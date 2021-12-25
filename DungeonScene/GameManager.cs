using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using DG.Tweening;
using TMPro;
using System.Text;

public class GameManager : Singleton<GameManager>
{
    public ushort _turnNum { get;private set; }//現在のターン数。

    [NonSerialized]public StageManager _stagemanager;
    [NonSerialized]public EffectManager _effect;

    bool _playerTurn = true;
    public bool _enemyTurn = false;

    [NonSerialized]public bool _playerSelect = false;

    public byte _clearLevel = 10;
    public int _currentStage = 0;
    [NonSerialized]public bool doingSetup = true;
    public Text _levelText;
    public GameObject _levelImage;
    public Player _player;
    public TopUI _TopUI;

    [SerializeField] ActSlider _actSlider;
    [SerializeField] StageLevelData _stageLevelData;
    [SerializeField] ClearPanel _clearPanel;
    [SerializeField] GameOver _gameOverPanel;
    [SerializeField] CameraMove _cameraMove;
    [SerializeField] AilmentIconManager _ailmentIconManager;
    [SerializeField] TextMeshProUGUI _rewordButtonCount;
    [SerializeField] GameObject _rewordCountBG;
    [SerializeField] TutorialManager _tutorialManager;

    List<Enemy> _enemies;

    [Tooltip("攻撃するたびに加算する倍率")]
    public float _damageIncrement;
    /// <summary>
    /// x,zの変化量
    /// </summary>
    public Dictionary<Direction, int[]> _positions = new Dictionary<Direction, int[]>()
    {
        {Direction.Up, new int[]{0,1 } },
        {Direction.UpperRigjt, new int[]{1,1 } },
        {Direction.Right, new int[]{1,0} },
        {Direction.LowerRight, new int[]{1,-1 } },
        {Direction.Down, new int[]{0,-1 } },
        {Direction.LowerLeft, new int[]{-1,-1 } },
        {Direction.Left, new int[]{-1,0 } },
        {Direction.UpperLeft, new int[]{-1,1 } },
        {Direction.Null, new int[]{0,0 } },
    };


    async protected override UniTask Init()
    {
        if(SoundManager.Instance ==null)
        {
            SceneManager.LoadScene("TitleScene");
            return;
        }

        DOTween.Init();
        //ローディング画面を出す。
        Loading.Instance.ShowLoading();
        SaveStage.StageData data = null;

        //チュートリアルではない場合はセーブデータをロードする
        if (!Setting.IsTutorial)
        {
            data = await SaveStage.Deserialize();
        }
        //前回の続きからかどうか
        if (data !=null)
        {
            _isFirst = data._isClear;
        }
        else
        {
            _isFirst = true;
        }
        //チュートリアルのときは最初から
        if(Setting.IsTutorial)
        {
            _isFirst = true;
        }
        _isSave = false;
        _stagemanager = GetComponent<StageManager>();
        //前回の続きからの場合、セーブデータからそれぞれ設定する。
        if (!_isFirst)
        {
            int currentLevel = data._currentStage;
            _currentStage = currentLevel-1;
            Level._Level = data._stageLevel;
            _turnNum = data._turnNum;
            _stagemanager._row = data._row;
            _stagemanager._colum = data._column;
            _stagemanager.SetGoalPos(data._exitPosition);
            _stagemanager._forcedBattle = data._forceBattle;
            RewardAds.SetRewardCount(data._canGetRewordNum);
            SetRewordCount(RewardAds._rewardViewableCount);
        }
        else
        {
            //続きからではなくチュートリアルでないなら、過去の所持アイテムのデータ等を削除する。
            if(!Setting.IsTutorial)
            {
                Save.Delete("");
                //初期装備をつける。
                await SavePlayerEquip.SavePlayerEquips(1002, 2000);
            }
            RewardAds.SetRewardCount(1);
        }
        _levelImage.SetActive(true);
        //ロード画面を隠す。
        Loading.Instance.HideLoading(0).Forget();

        //ステージの難易度を計算してステージのタイプを設定する。
        int num = (_currentStage+1) / _levelStep;
        var type = (StageType)Enum.ToObject(typeof(StageType), num);
        _stagemanager.Init(type);

        var stageData = _stageLevelData.GetData(Level._Level);
        _clearLevel = stageData._clearLevel;
        
        _enemies = new List<Enemy>();

        //装備のデータをロード
        var equipData = await LoadData();
        var (skillModels, equip) = equipData;
        var (weapon, armor) = equip;

        //プレイヤーのオブジェクトを作成して初期化
        await _player.PlayerInit(weapon, armor, skillModels);

        //ステータスの情報を更新
        ItemManager.Instance.UpdateStatusCtrl();

        //前回の続きからの場合、プレイヤーのデータをロードして設定する。
        if (!_isFirst)
        {
            var pdata = await SavePlayerParams.Deserialize();
            _player.AddAilments(pdata._playerAilment);
            _player.SetTotalExp(pdata._playerTotalExp);
            _player.SetHP(pdata._playerHP);
            _player.SetMP(pdata._playerMP);
            _player._life =pdata._playerLife;
        }
        await InitGame();

        _TopUI.Init(_player.GetHP(), _player.GetMP(), _player.GetMaxHP(), _player.GetMaxMP(), _currentStage, _player.GetLv());
        _ailmentIconManager.Init();
        if(Setting.IsTutorial)
        {
            _tutorialManager.StartTutorial();
        }
    }
    byte _levelStep = 20;

    /// <summary>
    /// 20区切りで難易度を変えている。
    /// </summary>
    public int StageLevel()
    {
        return _currentStage / _levelStep;
    }

    /// <summary>
    /// プレイヤーが行動をするときに使う。
    /// /summary>
    public async UniTask<bool> PlayerAction(Func<UniTask<bool>> task, ConsecutiveTimes times)
    {
        //敵のターンのときは何もしない
        if (_playerSelect || !_playerTurn)
        {
            return false;
        }
        _playerSelect = true;
        var success = await task();

        //行動ゲージを減らす。
        if (success && !doingSetup)
        {
            _actSlider.CountDown(times);
        }
        _playerSelect = false;

        //行動ゲージが0なら敵のターンにする。
        if (_actSlider._intervalcount <= 0)
        {
            _playerTurn = false;
            await PlayerTurnEnd();
            //エネミー関数
            EnemyTurn().Forget();
        }
        return success;
    }
    /// <summary>
    /// プレイヤーが行動をするときに使う。
    /// /summary>
    async public UniTask PlayerAction(ConsecutiveTimes times)
    {
        if (_playerSelect || !_playerTurn)
        {
            return;
        }

        _playerSelect = true;
        if (!doingSetup)
        {
            _actSlider.CountDown(times);
        }
        _playerSelect = false;

        if (_actSlider._intervalcount <= 0)
        {
            _playerTurn = false;
            await PlayerTurnEnd();
            //エネミー関数
            EnemyTurn().Forget();
        }
    }
    bool _clear = false;

    /// <summary>
    /// ゲームクリアの処理
    /// </summary>
    void GameClear()
    {
        if (_gameover || _clear)
        {
            return;
        }
        _clear = true;
        _clearPanel.ShowPanel();
        if(!Setting.IsTutorial)
        {
            SaveRecordData.SaveRecord(_clearLevel).Forget();
        }
        SaveData(true).Forget();
    }
    /// <summary>
    /// 広告視聴回数をセットする。
    /// </summary>
    /// <param name="num"></param>
    public void SetRewordCount(int num)
    {
        _rewordButtonCount.text = num.ToString();
        _rewordCountBG.SetActive(num != 0);
    }

    bool _isFirst = true;
    bool _isSave = true;
    /// <summary>
    /// 次の階層に進むたびに設定する。
    /// </summary>
    async public UniTask InitGame()
    {
        _player._canMove = false;
        doingSetup = true;
        _playerTurn = true;
        _playerSelect = false;
        if (_clearLevel <= _currentStage)
        {
            Debug.Log("クリア");
            GameClear();
            return;
        }

        _currentStage++;
        if(!Setting.IsTutorial)
        {
            SaveRecordData.SaveRecord(_currentStage).Forget();
        }
        //現在の階層を表示
        _levelImage.SetActive(true);
        var sb = new StringBuilder();
        sb.Append(Instance._currentStage);
        sb.Append("層");
        _levelText.text = sb.ToString();
        _enemies.Clear();
        sb = null;

        //初回、クリアしている、現在進行中のどれかの場合
        if (_isSave || _isFirst)
        {
            //広告視聴可能回数を増やす。
            var add =  (_currentStage % RewardAds._getLevel == 0) ? 1 : 0;
            RewardAds.AddRewardCount(add);
            SetRewordCount(RewardAds._rewardViewableCount);
            //次の階層を作成
            await _stagemanager.CreateStage(_currentStage);
            //現在の進行をセーブする。
            SaveData(false).Forget();
        }
        //前回の続きからの場合
        else
        {
            //前のデータをロードしてステージを作成
            await _stagemanager.CreateStageLoadedObjectData();
        }

        _isSave = true;
        _actSlider.ResetCount();
        //カメラの初期化
        _cameraMove.Init(_player.gameObject);
        await UniTask.Delay(1000);
        HideLevelImage();
        doingSetup = false;
        _player._canMove = true;
        if(_enemies.Count > 10)
        {
            MessageManager.Instance.ShowMessage(1,"モンスターが大量発生している！");
        }
        GC.Collect();
        Resources.UnloadUnusedAssets().ToUniTask().Forget();
    }

    /// <summary>
    /// 状態異常の処理、空腹度を減らす
    /// </summary>
    async public UniTask PlayerTurnEnd()
    {
        //空腹度を減らす
        _player.DownLife(-1);

        //状態異常の処理
        if (_player.GetAilment().Contains(Ailment.Poison))
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(_player.transform.position, Ailment.Poison, _player.transform.rotation).Forget();
            _player.DownHp();
            _player.PlayHitAnimation();
            await UniTask.Delay(700);
        }
        if (_player.GetAilment().Contains(Ailment.BadPoison))
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(_player.transform.position, Ailment.Poison, _player.transform.rotation).Forget();
            var down = _player.GetMaxHP() / 12;
            _player.DownHp(down);
            _player.PlayHitAnimation();
            await UniTask.Delay(700);
        }
    }
    [NonSerialized] public bool canReword = false;
    /// <summary>
    /// 途中経過をセーブする
    /// </summary>
    async UniTask SaveData(bool isClear)
    {
        //チュートリアルならセーブしない
        if (Setting.IsTutorial) { return; }
        List<int> ids = new List<int>();
        List<UniTask> tasks = new List<UniTask>(10);

        //アイテムデータ
        if (ItemManager.Instance._items != null)
        {
            foreach (var item in ItemManager.Instance._items)
            {
                ids.Add(item._ID);
            }
            tasks.Add(SaveHeldItemData.SaveHeldItems(ids.ToArray()));
        }

        //プレイヤーの現在のスキル
        var skill = _player.GetSkillList();
        if (skill != null)
        {
            var skills = skill.Select(x => (int)x).ToList();
            tasks.Add(SavePlayerSkill.SavePlayerSkills(skills));
        }

        //プレイヤーの装備
        var weapon = _player._charactorWeapon.GetWeapon();
        var armor = _player._charactorWeapon.GetArmor();
        int weaponID = 0;
        int armorID = 0;
        if (weapon != null)
        {
            weaponID = weapon._ID;
        }
        if (armor != null) { armorID = armor._ID; }
        tasks.Add(SavePlayerEquip.SavePlayerEquips(weaponID, armorID));
        //ステージのオブジェクトのデータ
        tasks.Add(_stagemanager._objectsData.SaveObjects());
        //ステージの基本情報
        tasks.Add(SaveStage.SaveStageData(isClear, _currentStage,_turnNum, _stagemanager._row, _stagemanager._colum,
            _stagemanager._goalPosition, RewardAds._rewardViewableCount,_stagemanager._forcedBattle));

        //プレイヤーの基本情報
        var ailment = _player.GetAilment().Select(x => (int)x).ToArray();
        tasks.Add(SavePlayerParams.SaveStageData(_player.GetTotalExp(),_player.GetHP(), _player.GetMP(),_player._life, ailment));
        await UniTask.WhenAll(tasks);
    }
    /// <summary>
    /// セーブデータのロード
    /// </summary>
    async UniTask<(List<SkillModel>,(WeaponModel, ArmorModel))> LoadData()
    {
        //チュートリアルの場合は予め用意しているデータをロード
        if(Setting.IsTutorial)
        {
            var skills = _tutorialManager.GetSkills().ToList();
            var armor = _tutorialManager.GetArmor();
            var weapon = _tutorialManager.GetWeapon();
            return (skills, (weapon, armor));
        }
        //キャラクターのheldItemと装備情報
        var task1 = LoadHeldItemData();
        var task2 = LoadSkillData();
        var task3 = LoadEquip();
        var (_,skill,result) = await UniTask.WhenAll(task1.AsAsyncUnitUniTask(), task2, task3);
        return (skill, result);
    }
    /// <summary>
    /// プレイヤーのスキルをロード
    /// </summary>
    /// <returns></returns>
    async public UniTask<List<SkillModel>> LoadSkillData()
    {
        var skills = await SavePlayerSkill.Deserialize();
        if(skills !=null)
        {
            var skillData = await MasterModel.LoadAllModelsOfSelectedType<SkillModel>(skills.IDs.ToArray());
            return skillData;
        }
        return null;
    }
    /// <summary>
    /// プレイヤーの装備品をロード
    /// </summary>
    /// <returns></returns>
    async public UniTask<(WeaponModel,ArmorModel)> LoadEquip()
    {
        var skills = await SavePlayerEquip.Deserialize();
        if(skills !=null)
        {
            var armorData = MasterModel.Load<ArmorModel>(skills.armorID);
            var weaponData = MasterModel.Load<WeaponModel>(skills.weaponID);

            var (weapon, armor) = await UniTask.WhenAll(weaponData, armorData);
            return (weapon, armor);
        }
        return (null, null);

    }
    /// <summary>
    /// プレイヤーの所持アイテムをロード
    /// </summary>
    async public UniTask LoadHeldItemData()
    {
        var heldItem =await SaveHeldItemData.Deserialize();
        if(heldItem != null && heldItem.Count >0)
        {
            List<int> itemids = new List<int>();
            //countの分がない。
            foreach(var item in heldItem)
            {
                for(int i=0; i<item.Count; i++)
                {
                    itemids.Add(item.ID);
                }
            }
            var itemModels = await MasterModel.LoadAllModelsOfSelectedType<ItemModel>(itemids.ToArray());
            ItemManager.Instance._items = itemModels;
        }

    }
    void HideLevelImage()
    {
        _levelImage.SetActive(false);
    }
    /// <summary>
    /// エネミーの情報をリストに格納
    /// </summary>
    public void AddEnemy(Enemy script)
    {
        _enemies.Add(script);
    }
    /// <summary>
    /// エネミーの情報をリストから削除
    /// </summary>
    public void DestoryEnemyToList(Enemy enemy)
    {
        _enemies.Remove(enemy);
    }
    /// <summary>
    /// エネミーリストを返す。
    /// </summary>
    public List<Enemy> GetEnemies()
    {
        return _enemies;
    }
    /// <summary>
    /// エネミーのターンを始める。
    /// </summary>
    async UniTask EnemyTurn()
    {
        if(isStop)
        {
            AllEnemyTurnEnd();
            return;
        }
        _enemyTurn = true;
        _stagemanager._objectsData.RemoveAllObjectBlock(objectType.Enemy);
        //エネミーがいる場所を記録する
        for (int i = 0; i < _enemies.Count; i++)
        {
            _stagemanager._objectsData.AddObjectData(objectType.Enemy, _stagemanager.xz2i(Mathf.RoundToInt(_enemies[i].transform.position.x), Mathf.RoundToInt(_enemies[i].transform.position.z)), _enemies[i].gameObject, _enemies[i]._model);
        }
        //エネミーの行動
        for (int i = 0; i < _enemies.Count; i++)
        {
            await _enemies[i].MoveEnemy();
        }
        AllEnemyTurnEnd();
    }
    /// <summary>
    /// 全てのエネミーのターンが終了したら
    /// </summary>
    public void AllEnemyTurnEnd()
    {
        _player._canMove = true;
        _playerTurn = true;
        _enemyTurn = false;
        //プレイヤーの防御モードを解除
        _player.SetIsDefend(false);

        _turnNum++;
        _actSlider.ResetCount();
    }

    bool _gameover = false;
    /// <summary>
    /// ゲームオーバー
    /// </summary>
    public void GameOver()
    {
        if(_gameover || _clear)
        {
            return;
        }
        _gameover = true;
        _gameOverPanel.Write(_currentStage);
        _gameOverPanel.ShowPanel().Forget();
    }

    /// <summary>
    /// クリア失敗したときにセーブする。
    /// </summary>
   async public UniTask SaveGiveUp()
    {
        if (Setting.IsTutorial) { return; }
        await SaveRecordData.SaveRecord(0);
        await SaveHeldItemData.SaveHeldItems(new int[] { });
        await SaveStage.SaveStageData(true, 0, 0, 0, 0,0, RewardAds._rewardViewableCount, false);
        await SavePlayerEquip.SavePlayerEquips(-1, -1);
    }

    //スキルで使用 
    [NonSerialized]bool isStop = false; //時を止める
    [NonSerialized]bool isHalf = false; //行動半減
    public void AddStop()
    {
        isStop = true;
        _player._spAilment.AddStatusAilment(SpAilment.StopTime);
    }
    public void RemoveStop()
    {
        isStop = false;
        _player._spAilment.RemoveSpAilment(SpAilment.StopTime);
    }
    public bool GetIsStop()
    {
        return isStop;
    }
    public void AddHalf()
    {
        isHalf = true;
        _player._spAilment.AddStatusAilment(SpAilment.SpeedUp);
    }
    public void RemoveHalf()
    {
        isHalf = false;
        _player._spAilment.RemoveSpAilment(SpAilment.SpeedUp);
    }
    public bool GetIsHalf()
    {
        return isHalf;
    }
    /// <summary>
    /// 全てのエネミーのアニメーションを止める
    /// </summary>
    public void StopAllEnemyAnimation()
    {
        foreach(var e in _enemies)
        {
            e.StopAnimation();
        }
    }
    /// <summary>
    /// 全てのエネミーのアニメーションを再生する。
    /// </summary>
    public void StartAllEnemyAnimation()
    {
        foreach (var e in _enemies)
        {
            e.StartAnimation();
        }
    }
}
