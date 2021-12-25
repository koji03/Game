using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;

public enum objectType
{
    Block = 0,
    Item = 1,
    Enemy =2,
}
[Serializable]
public struct FloorData
{
    public int _colum, _row;
    public int _defaultEnemy;

}
public class StageManager : MonoBehaviour
{
    //生成したオブジェクトはすべてここに入れる。
    [NonSerialized] public List<GameObject> _objects = new List<GameObject>();

    [SerializeField] FloorData[] _floorData;
    [NonSerialized]public int _colum = 5, _row = 5;
    public int
        _itemMinimun = 1,
        _itemMaximum = 2, _weaponMinmum = 0, _weaponMaxmum = 1,
        _blockMinNum = 1, _blockMaxNum = 3,
        _wallMinimun = 1, _wallMaximum = 4,
        _wallMinLength =0 , _wallMaxLength =3,
        _decorationMinNum =0, _decorationMaxNum = 3;

    [SerializeField] StageParts _stageParts;

    public StageType _stageType {private get; set; }

    public GameObject _goal;

    [SerializeField] GameObject _attackArea;
    List<Vector3> _objectGridPositions = new List<Vector3>();//オブジェクトを配置可能なグリッド

    [NonSerialized]public ObjectsData _objectsData;

    [NonSerialized]public Vector3 _playerNextPosition;
    GameObject[,] _attackAreaMarks;

    [SerializeField] StageBGM _stageBGM;

    public int _goalPosition { get; private set; }

    public void SetGoalPos(int pos)
    {
        _goalPosition = pos;
    }

    public void Init(StageType StageType)
    {
        _stageType = StageType;
        //BGMを再生する。
        _stageBGM.PlayBGM(StageType);
        _objectsData = new ObjectsData();

        //ステージのタイプによって所持できるアイテムの数を変える。
        Setting.UpdateNumMaxItemsFromType(StageType);
        ItemManager.Instance.UpdateMaxItemNumText(Setting.NumCanHaveHeldItems);
    }

    //強制戦闘 enemyの数が0になると先に進める
    [NonSerialized] public bool _forcedBattle = false;


    /// <summary>
    /// ステージのブロックやエネミーを作成する。
    /// </summary>
    async public UniTask CreateStage(int currentLevel)
    {
        //フロアのサイズを設定する。
        SetupFloorData();

        //ステージにあるブロックを削除する。
        DestroyBlocks();

        //マスのデータの初期化
        _objectsData.Init(xz2i(_row, _colum));

        //壁や床のブロックを生成
        CreateBlocks();

        //ゴールの位置を設定
        _goal.transform.position = MakeGoalPositionAtRandom();

        //エネミーやアイテムなどのオブジェクトを削除する。
        DestroyAllObject();

        //ブロックの配列を初期化
        _objectsData.InitObjectDataArray();

        //グリッドのポジションのリストを作成
        MakeGridPositionList(_row,_colum);


        int stageLevel = Level._Level;
        //チュートリアルとそうでない場合で分ける。
        //ブロック、エネミー、その他オブジェクトの作成
        if (Setting.IsTutorial)
        {
            //チュートリアルのモンスターのレベルとアイテムの数を設定
            var monsternum = currentLevel - 1;
            var itemnum = currentLevel;
            //エネミーの作成
            CreateObject(await _stageParts.LoadEnemies(stageLevel), monsternum, 0, objectType.Enemy, Quaternion.Euler(0, 0, 0));
            //アイテムの作成
            CreateObject(await _stageParts.LoadItems(stageLevel), itemnum, 0, objectType.Item, Quaternion.Euler(0, 0, 0));
        }
        else
        {
            //エネミーの数を計算する。
            int enemyCount = CalculateEnemyCount(currentLevel);
            //フィールド上のブロックの作成
            CreateObject(await _stageParts.LoadBlocks(stageLevel), _blockMinNum, _blockMaxNum, objectType.Block, Quaternion.Euler(0, 0, 0));
            if(enemyCount !=0)
            {
                //エネミーの作成
                CreateObject(await _stageParts.LoadEnemies(stageLevel), enemyCount, enemyCount, objectType.Enemy, Quaternion.Euler(0, 0, 0));
            }
            //アイテムの作成
            CreateObject(await _stageParts.LoadItems(stageLevel), _itemMinimun, _itemMaximum, objectType.Item, Quaternion.Euler(0, 0, 0));

            //強制戦闘にするか決める
            DecideForcedBattleAtRandom(enemyCount);
        }
    }

    /// <summary>
    /// ステージのデータをロードしてブロックを作成する。
    /// 途中から始める場合に使う。
    /// </summary>
    async public UniTask CreateStageLoadedObjectData()
    {
        //マスのデータの初期化
        _objectsData.Init(xz2i(_row, _colum));

        //壁や床のブロックを生成
        CreateBlocks();

        //ゴールの位置を設定
        _goal.transform.position = new Vector3(i2xz(_goalPosition)[0], 0.1f, i2xz(_goalPosition)[1]);

        //ブロックのデータ(エネミーやアイテム)をロードする。
        var blocks = await _objectsData.Deserialize();

        //ロードしたデータからアイテムやエネミーを作成
        foreach (var data in blocks)
        {
            var model = data._model;
            //オブジェクトを作成する。
            var obj = CreateObject(model, new Vector3(i2xz(data._position)[0], height, i2xz(data._position)[1]));
            if (model is EnemyModel)
            {
                var enemy = obj.GetComponent<Enemy>();
                GameManager.Instance.AddEnemy(enemy);
                enemy.EnemyInit(GameManager.Instance._currentStage, (EnemyModel)model);
                var pos = obj.transform.position;
                obj.transform.position = new Vector3(pos.x, 0, pos.z);
            }
            _objectsData.AddObjectData(data._type, data._position, obj, model);
        }
        MakeForcedBattle(_forcedBattle);
    }

    IDisposable _forcedBattleSubscribe;
    /// <summary>
    /// 強制戦闘部屋にするかを決める。
    /// </summary>
    void DecideForcedBattleAtRandom(int enemycount)
    {
        //10%の確立
        var num = UnityEngine.Random.Range(0, 101);
        //エネミーが0または確率でゴールを表示している状態にする。
        //強制戦闘でない場合
        if (enemycount == 0 || num > 11)
        {
            _forcedBattle = false;
            _goal.SetActive(true);
        }
        else
        {
            _forcedBattle = true;
            
            //ゴールを隠す
            _goal.SetActive(false);

            var enemies = GameManager.Instance.GetEnemies();
            //エネミーの数が0になったらゴールを出現させる。
            _forcedBattleSubscribe = enemies.ObserveEveryValueChanged(x => x.Count).Where(x => x == 0).Subscribe(_ => ShowGoal());
            MessageManager.Instance.ShowMessage(3, "モンスターをすべて倒してワープゾーンを出現させよう。");
        }
    }
    /// <summary>
    /// ロードしたデータから設定する。
    /// </summary>
    void MakeForcedBattle(bool forcedCombat)
    {
        //強制戦闘の場合
        if (forcedCombat)
        {
            _forcedBattle = forcedCombat;
            //ゴールを隠す
            _goal.SetActive(false);
            var enemies = GameManager.Instance.GetEnemies();

            //エネミーの数が0になったらゴールを出現させる。
            _forcedBattleSubscribe = enemies.ObserveEveryValueChanged(x => x.Count).Where(x => x == 0).Subscribe(_ => ShowGoal());
            MessageManager.Instance.ShowMessage(3, "モンスターをすべて倒してワープゾーンを出現させよう。");
        }
        else
        {
            _forcedBattle = forcedCombat;
            _goal.SetActive(true);
        }
    }
    void ShowGoal()
    {
        _goal.SetActive(true);
        MessageManager.Instance.ShowMessage(3, "ワープゾーンが出現した。");
        _forcedBattleSubscribe.Dispose();
    }

    float height = 0.5f;
    List<GameObject> _outwall = new List<GameObject>();
    List<GameObject> _tiles = new List<GameObject>();
    /// <summary>
    /// 壁や床のブロックを削除
    /// </summary>
    void DestroyBlocks()
    {
        foreach(var b in _outwall)
        {
            Destroy(b);
        }
        foreach (var b in _tiles)
        {
            Destroy(b);
        }
        _outwall.Clear();
        _tiles.Clear();
    }
    /// <summary>
    /// フロアのサイズ（row colomn)とデフォルトのエネミーの数を決める。
    /// </summary>
    void SetupFloorData()
    {
        //チュートリアルの場合
        if(Setting.IsTutorial)
        {
            //チュートリアル用のデータを取得し設定
            var floort = _floorData[_floorData.Length-1];
            _row = floort._row;
            _colum = floort._colum;
            _defaultEnemy = floort._defaultEnemy;
        }
        else
        {
            //ランダムにデータを取得して設定
            var floor = _floorData[UnityEngine.Random.Range(0, _floorData.Length-1)];
            _row = floor._row;
            _colum = floor._colum;
            _defaultEnemy = floor._defaultEnemy;
        }
    }
    /// <summary>
    /// 外壁の床のブロックを作成
    /// </summary>
    void CreateBlocks()
    {
        //ステージのタイプを変更
        ChangeStageType();
        
        //外壁を取得する。
        var outwall = _stageParts.GetOutWall(_stageType);
        //床のブロックを取得
        var tiles = _stageParts.GetTiles(_stageType);
        //攻撃マークの配列を作成
        _attackAreaMarks = new GameObject[_row,_colum];
        Quaternion quaternion = Quaternion.Euler(90f, 0f, 0f);

        //床と壁と攻撃マークの作成する。
        for (int x = -2; x < _row +2 ; x++)
        {
            for (int y = -2; y < _colum + 2 ; y++)
            {
                //外壁の時は床を作成しない。
                if (MakeOutWall(x, y, outwall[UnityEngine.Random.Range(0, outwall.Length)])) { continue; }
                GameObject tile;

                //タイルをランダムに取得
                tile = tiles[UnityEngine.Random.Range(0,tiles.Length)];
                var obj = Instantiate(tile, new Vector3(x, -0.5f, y), Quaternion.identity);

                //攻撃マークも作成する。
                var attackMark = Instantiate(_attackArea, new Vector3(x, 0.01f, y), quaternion);

                //リストに格納しておく。
                _tiles.Add(attackMark);
                _tiles.Add(obj);

                _attackAreaMarks[x, y] = attackMark;
                //攻撃マークは隠しておく。
                attackMark.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 外壁を作成する場合はtrueを返す。
    /// </summary>
    bool MakeOutWall(int x, int z,GameObject wall)
    {
        float height = 0f;
        GameObject obj;
        if(x < 0)
        {
            obj = Instantiate(wall, new Vector3(x, height, z), Quaternion.identity);
            _outwall.Add(obj);
            return true;
        }
        if(z < 0)
        {
            obj = Instantiate(wall, new Vector3(x, height, z), Quaternion.identity);
            _outwall.Add(obj);
            return true;
        }
        if(x >= _row)
        {
            obj = Instantiate(wall, new Vector3(x, height, z), Quaternion.identity);
            _outwall.Add(obj);
            return true;
        }
        if(z >= _colum)
        {
            obj = Instantiate(wall, new Vector3(x, height, z), Quaternion.identity);
            _outwall.Add(obj);
            return true;
        }
        return false;
    }

    //マス目から攻撃マークを表示する。
    public void ShowAttackArea(Vector3Int[] indexes)
    {
        for(int x =0; x<_row; x++)
        {
            for(int z=0; z<_colum; z++)
            {
                _attackAreaMarks[x, z].SetActive(indexes.Any(i => i.x == x && i.z == z));
            }
        }
    }

    int _defaultEnemy;
    /// <summary>
    /// 階層からモンスターの数を計算して返す
    /// </summary>
    int CalculateEnemyCount(int currentLevel)
    {
        if(UnityEngine.Random.Range(0,101) < 4) { return 0; }
        var max = UnityEngine.Random.Range(1,Mathf.FloorToInt(Mathf.Log(currentLevel, 3f))+1);
        return Mathf.Min(Mathf.Max(max, 1)+ DecideEnemyCountAtRandom(), 15) + _defaultEnemy;
    }

    /// <summary>
    /// エネミーの数をランダムに決める。
    /// </summary>
    int DecideEnemyCountAtRandom()
    {
        //モンスターの数に振れ幅を持たせる。
        int[] randomEnemyNum = new int[100]
        {
        0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,
        0,0,0,0,0,0,0,0,0,0,
        1,1,1,1,1,1,1,1,1,1,
        1,1,1,1,1,1,1,1,1,1,
        1,1,1,1,1,1,1,1,1,1,
        1,1,1,1,1,1,1,1,1,1,
        1,1,1,1,1,1,1,1,1,1,
        1,2,2,2,2,2,2,2,2,2,
        2,2,3,3,3,3,3,10,10,10,
        };
        return randomEnemyNum[UnityEngine.Random.Range(0, randomEnemyNum.Length)];
    }
    /// <summary>
    /// ゴールのポジションを決める。
    /// </summary>
    Vector3 MakeGoalPositionAtRandom()
    {
        if(Setting.IsTutorial)
        {
            return new Vector3(_row-1, 0.1f, _colum-1);
        }
        var r = UnityEngine.Random.Range(2, _row);
        var c = UnityEngine.Random.Range(_colum-3, _colum);
        _goalPosition = xz2i(r, c);
        return new Vector3(r, 0.1f, c);
    }

    /// <summary>
    /// ステージのタイプを変更する。
    /// 進行度によって変える。
    /// </summary>
    void ChangeStageType()
    {
        //現在の階層を元に計算して設定する。
        int num = GameManager.Instance.StageLevel();
        var tempType = _stageType;
        _stageType = (StageType)Enum.ToObject(typeof(StageType), num);

        //アイテム所持上限の計算
        Setting.UpdateNumMaxItemsFromType(_stageType);

        //ステージのタイプが変わったら
        if (tempType != _stageType)
        {
            //BGMを変える
            _stageBGM.PlayBGM(_stageType);
            MessageManager.Instance.ShowMessage(3, "アイテムの所持上限が上がった。");
            ItemManager.Instance.UpdateMaxItemNumText(Setting.NumCanHaveHeldItems);
        }
    }


    /// <summary>
    /// オブジェクトを配置可能なポジションのリストを作成する。
    /// </summary>
    void MakeGridPositionList(int row, int column)
    {
        _objectGridPositions.Clear();
        var goalposition = _goal.transform.position;

        for (int x = 1; x < row - 1 + 1; x++)
        {
            for (int z = 1; z < column - 1 + 1; z++)
            {
                //プレイヤーから周囲3マスとゴールのポジションにはオブジェクトを配置しない。
                if(_playerNextPosition.x+3 > x &&_playerNextPosition.z +3> z ||
                    goalposition.x == x && goalposition.z == z)
                {
                    continue;
                }
                _objectGridPositions.Add(new Vector3(x, 0, z));
            }
        }

    }

    /// <summary>
    /// ステージの難易度からアイテムやエネミーをランダムで生成する。
    /// </summary>
    /// 
    void CreateObject(DropItemData objects,int min , int max, objectType _type, Quaternion rotation)
    {
        //ステージの難易度に設定されているオブジェクトが無かったら終わる。
        if (objects.GetModels(GameManager.Instance.StageLevel()) == null)
        {
            return;
        }
        //オブジェクトを配置する数を決めて配置する。
        int objectNum = 0;
        if (max == 0)
        {
            objectNum = max;
        }
        else
        {
            objectNum = UnityEngine.Random.Range(min, max);
        }
        for (int i=0; i< objectNum; i++)
        {
            //オブジェクトを配置するポジションを決める
            Vector3 objectPosition = RandomPosition();
            if(objectPosition == Vector3.zero)
            {
                return;
            }
            //オブジェクトのモデルを取得。
            var objModel =RandomItemDrop(objects.GetDropItems(GameManager.Instance.StageLevel()), GameManager.Instance._currentStage);
            //そのモデルからオブジェクトを作成
            var obj = CreateObject(objModel, objectPosition);
            obj.transform.rotation = rotation;
            _objects.Add(obj);
            //オブジェクトのタイプによって処理を分岐
            switch(_type)
            {
                case objectType.Block:
                    _objectsData.AddObjectData(_type, xz2i(Mathf.RoundToInt(objectPosition.x), Mathf.RoundToInt(objectPosition.z)), obj, objModel);
                    break;
                case objectType.Enemy:
                    var enemy = obj.GetComponent<Enemy>();
                    //エネミーはY座標を0にする。
                    var pos = obj.transform.position;
                    obj.transform.position = new Vector3(pos.x, 0,pos.z);

                    //エネミーリストに追加する。
                    GameManager.Instance.AddEnemy(enemy);
                    enemy.EnemyInit(GameManager.Instance._currentStage, (EnemyModel)objModel);
                    _objectsData.AddObjectData(_type, xz2i(Mathf.RoundToInt(objectPosition.x), Mathf.RoundToInt(objectPosition.z)), obj, objModel);
                    break;
                case objectType.Item:
                    _objectsData.AddObjectData(_type, xz2i(Mathf.RoundToInt(objectPosition.x), Mathf.RoundToInt(objectPosition.z)),  obj, objModel);
                    break;
            }
        }
    }
    /// <summary>
    /// 配置可能なポジションを返す。
    /// </summary>
    Vector3 RandomPosition()
    {
        if (_objectGridPositions.Count == 0)
        {
            return Vector3.zero;
        }
        int randomIndex = UnityEngine.Random.Range(0, _objectGridPositions.Count);
        Vector3 randomPosition = _objectGridPositions[randomIndex];

        _objectGridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }
    /// <summary>
    /// 確率を計算して、そのレアリティのモデルを返す。
    /// </summary>
    public Model RandomItemDrop(DropItem[] data,int _currentLevel)
    {
        var num = UnityEngine.Random.Range(0, 101);
        Model model;
        if (num >=0 && num <=45)
        {
            model = GetModel(1);
        }
        else if(num>45 && num<=75)
        {
            model = GetModel(2);
        }
        else if(num > 75 && num <= 90)
        {
            model = GetModel(3);
        }
        else if (num > 90 && num <= 99)
        {
            model = GetModel(4);
        }
        else
        {
            model = GetModel(5);
        }
        return model;
        Model GetModel(int rarity)
        {
            //ﾚｱﾘﾃｨと同じモデルを探す。
            var itemmodels = data.Where(x => x._dropRate == rarity && x._dropLevel[0]<=_currentLevel && x._dropLevel[1] >=_currentLevel).ToArray();
            //該当のrarityがある場合はそれを返す。
            if(itemmodels.Length > 0)
            {
                var models = itemmodels.Select(x => x._model).ToArray();
                return models[UnityEngine.Random.Range(0, models.Length)];
            }
            //存在しない場合は低レアリティから順番に検索
            for (int i=0; i<rarity; i++)
            {
                itemmodels = data.Where(x => x._dropRate == i && x._dropLevel[0] <= _currentLevel && x._dropLevel[1] >= _currentLevel).ToArray();
                if (itemmodels.Length >0)
                {
                    break;
                }
            }
            if (itemmodels ==null || itemmodels.Length ==0)
            {
                itemmodels = data;
            }
            var models2 = itemmodels.Select(x => x._model).ToArray();
            return models2[UnityEngine.Random.Range(0, models2.Length)];
        }
    }

    /// <summary>
    /// オブジェクトを作成
    /// </summary>
    public GameObject CreateObject(Model model, Vector3 randomPosition)
    {
        randomPosition.y = height;
        GameObject obj = Instantiate(model._prefab, randomPosition, Quaternion.identity);
        _objects.Add(obj);
        return obj;
    }
    /// <summary>
    /// 作成しているオブジェクトをすべて削除する。
    /// </summary>
    void DestroyAllObject()
    {
        foreach (var obj in _objects)
        {
            Destroy(obj);
        }
    }
    /// <summary>
    /// オブジェクトのデータを削除する。
    /// </summary>
    public void RemoveObjectData(int x, int z,int _id, objectType type)
    {
        _objectsData.RemoveObjectData(xz2i(x,z), type, _id);
    }

    /// <summary>
    /// ポジションが壁の内側かどうか
    /// </summary>
    public bool IsIn(Vector3 _nextPosition)
    {
        return (_row > _nextPosition.x && _colum > _nextPosition.z) && (_nextPosition.x >= 0 && _nextPosition.z >= 0);
    }
    /// <summary>
    /// エネミーのデータを格納する
    /// </summary>
    public void AddEnemyObject(int  nextPositions,EnemyModel _enemyID ,GameObject obj)
    {
        var position = i2xz(nextPositions);
        _objectsData.AddObjectData(objectType.Enemy, xz2i(position[0], position[1]) ,  obj, _enemyID);
    }


    //インデックスからXポジションを計算
    public int i2x(int i)
    {
        return i % _row;
    }
    //インデックスからZポジションを計算
    public int i2z(int i)
    {
        return i / _row;
    }
    //インデックスからxとzを計算
    public int[] i2xz(int i)
    {
        return new int[] { i2x(i), i2z(i) };
    }
    //xとzの配列からインデックスを計算
    public int xz2i(int[] xz)
    {
        return xz2i(xz[0], xz[1]);
    }
    //xとzからインデックスを計算
    public int xz2i(int x, int z)
    {
        return z * _row + x;
    }

    public List<ObjectsData.Object> CheckBlock(ObjectsData blocks, int _pPosX, int pPosZ)
    {
        if(_pPosX <0 || pPosZ <0)
        {
            return null;
        }
        int pPos = xz2i(_pPosX, pPosZ);
        for (int x = 0; x < _row; x++)
        {
            for (int z = 0; z < _colum; z++)
            {
                if (blocks._objects[xz2i(x,z)] != null && blocks._objects[xz2i(x, z)].Count != 0)
                {
                    int ePos = xz2i(x, z);

                    if (pPos == ePos)
                    {
                        return blocks._objects[xz2i(x, z)];
                    }
                }

            }
        }
        return null;
    }
}
