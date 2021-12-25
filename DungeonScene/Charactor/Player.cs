using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;

//方向と角度
public enum Direction
{
    Up=0,
    UpperRigjt = 45,
    Right = 90,
    LowerRight = 135,
    Down = 180, 
    LowerLeft = 225,
    Left = 270,
    UpperLeft =315,
    Null = -1
}
[RequireComponent(typeof(CharactorStatus))]
[RequireComponent(typeof(PlayerSkillData))]
[RequireComponent(typeof(StatusAilment))]
[RequireComponent(typeof(StatusSpAilment))]
[RequireComponent(typeof(CharactorEquip))]
public class Player : MonoBehaviour
{
    public int _life = 100;
    Animator _animator;

    [NonSerialized]
    public Direction _playerDirection =Direction.Null;
    CharactorStatus _status;

    [NonSerialized]public CharactorEquip _charactorWeapon;

    [SerializeField] AudioClip _damageSE,_attackSE;
    PlayerSkillData _playerSkill;
    [NonSerialized]public StatusAilment _statusAilment;
    [NonSerialized] public StatusSpAilment _spAilment;

    public bool _isDefendMode { get;private set; }

    int DefendAnim;
    int AttackAnim;
    int WalkAnim ;
    int HitAnim;
    int DieAnim;
    int ItemAnim;
    int DefendHitAnim;

    int IsDefendMode;
    /// <summary>
    /// 初期化
    /// </summary>
    async public UniTask PlayerInit(ItemModel weaponModel,ItemModel armorModel,List<SkillModel> skillModels)
    {
        //アニメーションの設定
        DefendAnim = Animator.StringToHash("DefendHit");
        AttackAnim = Animator.StringToHash("Attack");
        WalkAnim = Animator.StringToHash("Walk");
        HitAnim = Animator.StringToHash("Hit");
        DieAnim = Animator.StringToHash("Die");
        ItemAnim = Animator.StringToHash("Item");
        DefendHitAnim = Animator.StringToHash("DefendHit");
        IsDefendMode = Animator.StringToHash("IsDefend");

        _canMove = true;

        //プレイヤーの基礎ステータスをロードして設定
        var model = await MasterModel.Load<CharactorModel>(0);
        _status = GetComponent<CharactorStatus>();
        _status.Init(model);

        _spAilment = GetComponent<StatusSpAilment>();
        _spAilment.Init();

        _statusAilment = GetComponent<StatusAilment>();
        _playerSkill = GetComponent<PlayerSkillData>();
        _animator = GetComponent<Animator>();
        _charactorWeapon = GetComponent<CharactorEquip>();

        //プレイヤーの位置
        transform.position = new Vector3(0, 0, 0);
        GameManager.Instance._stagemanager._playerNextPosition = transform.position;
        
        //装備の設定
        SetWeaponObject(weaponModel);
        SetArmorObject(armorModel);
        
        //スキルの設定
        var skills =(skillModels != null)? skillModels.Select(x => (Skill)Enum.ToObject(typeof(Skill), x._ID)).ToList(): null;
        _playerSkill.SetSkills(skills);
    }

    /// <summary>
    /// 防御モード
    /// </summary>
    public void SetIsDefend(bool defend)
    {
        _isDefendMode = defend;
        _animator.SetBool(IsDefendMode, defend);
    }

    public bool _canMove { get; set; }
    /// <summary>
    /// 移動
    /// </summary>
    public async UniTask<bool> ATMove(Direction direction)
    {
        //催眠状態のときは動かない
        if (_statusAilment.HaveAilment(Ailment.Sleep))
        {
            return false;
        }

        bool canMove = await Move(direction);

        //動けた場合
        if (canMove)
        {
            //プレイヤーの攻撃範囲を表示
            SetAttackMark();
            CheckGoalPosition(Vector3Int.RoundToInt(transform.position));
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// プレイヤーの行動可能回数を取得する。
    /// </summary>
    public ConsecutiveTimes GetActableTimes()
    {
        //装備している武器によって回数は変わる。　装備していない場合は1回のみ
        var w = _charactorWeapon.GetWeapon() as WeaponModel;
        if (w ==null)
        {
            return ConsecutiveTimes.One;
        }
        else
        {
            return w._times;
        }
    }
    /// <summary>
    /// 移動
    /// </summary>
    async public UniTask<bool> Move(Direction direction)
    {
        Vector3 start = transform.position;
        //directionからプレイヤーの移動場所を計算
        Vector3Int end = Vector3Int.RoundToInt(start + new Vector3(GameManager.Instance._positions[direction][0], 0, GameManager.Instance._positions[direction][1]));
        //回転
        Rotate(direction);
        //のろい状態の場合は動けない（回転はできる）
        if (_statusAilment.HaveAilment(Ailment.Curse))
        {
            MessageManager.Instance.ShowMessage("呪いで動くことが出来ない。");
            Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Curse, transform.rotation).Forget();
            return false;
        }

        //移動先にアイテム以外の物があるときや壁の場合は移動しない
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.z));
        bool _onItem =(block != null)? !block.Any(x => x._type != objectType.Item) : false;
        if ((_onItem || block ==null) && GameManager.Instance._stagemanager.IsIn(end))
        {
            if (!_canMove)
            {
                MessageManager.Instance.ShowMessage("このターンは移動をすることができない。");
                return false;
            }
            GameManager.Instance._stagemanager._playerNextPosition =end;
            //アイテムを拾う。
            if(_onItem)
            {
                var b = block.Find(x => x._type == objectType.Item);
                await Movement(end, ()=>GetItem(b._object,(ItemModel)b._model, Vector3Int.RoundToInt(end)));
                
            }else
            {
                //動く
                await Movement(end);
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// プレイヤーの位置にゴールがあれば次の階層に進ませる。
    /// </summary>
    void CheckGoalPosition(Vector3Int end)
    {
        StageManager stage = GameManager.Instance._stagemanager;
        //ゴールが非アクティブの場合は何もしない
        if (!stage._goal.activeSelf)
        {
            return;
        }
        Vector3 goal = stage._goal.transform.position;
        goal.y = end.y;
        if (goal == end)
        {
            transform.position = Vector3.zero;
            GameManager.Instance.doingSetup = true;
            stage._playerNextPosition = transform.position;
            Restart().Forget();

        }
    }
    /// <summary>
    /// 移動処理、
    /// </summary>
    async UniTask Movement(Vector3Int end, Action _action = null)
    {
        //移動距離を計算して移動する。
        float remainingDistance = (transform.position - end).sqrMagnitude;
        _animator.SetTrigger(WalkAnim);
        _animator.speed = 10;
        while (remainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, end, 5f * Time.deltaTime);
            remainingDistance = (transform.position - end).sqrMagnitude;
            await UniTask.Yield();
        }
        _animator.speed = 1;
        transform.position = end;

        //移動後に何かすることがあれば実行
        if(_action != null){ _action(); }
    }
    /// <summary>
    /// 回転
    /// </summary>
    public bool Rotate(Direction direction)
    {
        if (_playerDirection == direction)
        {
            return false;
        }
        _playerDirection = direction;
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(GameManager.Instance._positions[direction][0], 0, GameManager.Instance._positions[direction][1]);

        StartCoroutine(Rotation(end));
        return true;
    }
    /// <summary>
    /// ターゲットがある方向に向く
    /// Rotateを使う
    /// <returns></returns>
    IEnumerator Rotation(Vector3 _target)
    {
        var direction = _target - transform.position;
        direction.y = 0;

        var lookRotation = Quaternion.LookRotation(direction, Vector3.up);

        for (int i=0; i<3; i++)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, i/3);
            yield return null;
        }
        transform.LookAt(_target);
        SetAttackMark();
        yield break;
    }

    /// <summary>
    /// ガード状態にする。
    /// </summary>
    async public UniTask<bool> OnDefend()
    {
        _canMove = false;
        MessageManager.Instance.ShowMessage("防御態勢になった！");
        SetIsDefend(true);
        _animator.SetTrigger(DefendAnim);

        //ガードアニメーションの半分の時間待機する。
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(TimeSpan.FromSeconds(state.length / 2));
        return true;
    }

    /// <summary>
    /// プレイヤーの攻撃範囲に赤い□を表示する。
    /// </summary>
    public void SetAttackMark()
    {
        var d = AttackPositions.GetAttackPosition(_charactorWeapon.GetWeaponDirection(),transform);
        GameManager.Instance._stagemanager.ShowAttackArea(d);
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <returns></returns>
    async public UniTask<bool> OnAttack(AttackDirection attackDirection, bool isPenetrating, int strength, int hitrate, bool shouldAnim = true)
    {
        _canMove = false;
        if(shouldAnim)
        {
            _animator.SetTrigger(AttackAnim);
        }
        //アニメーションの半分の時間待機する。
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(TimeSpan.FromSeconds(state.length/2));

        //攻撃範囲を取得して攻撃をする。
        var attackPosition = AttackPositions.GetAttackPosition(attackDirection,transform);
        foreach (var atkPos in attackPosition)
        {
           if( await Attacked(atkPos, isPenetrating, strength, hitrate))
            {
                break;
            }
        }
        return true;
    }
    /// <summary>
    /// スキル攻撃
    /// </summary>
    public async UniTask<bool> SkillAttack(string name,Func<UniTask> _action, EffectData effect,AttackDirection direction, int MP)
    {
        MP = (_spAilment.HaveAilment(SpAilment.MPHalf)) ? MP / 2 : MP;
        if (!DownMP(MP))
        {
            return false;
        }
        _canMove = false;

        MessageManager.Instance.ShowMessage(name, "を使った。");

        //攻撃モーションの1/3だけ待機する。
        _animator.SetTrigger(AttackAnim);
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(TimeSpan.FromSeconds(state.length / 3));

        //スキルの攻撃範囲を取得してエフェクトを再生する。
        var positions = AttackPositions.GetAttackPosition(direction,transform);
        await Locator<IEffectManager>.I.PlayEffect(positions[0], effect,transform.rotation);
        //ここにスキル処理
        await _action();
        return true;
    }


    /// <summary>
    /// アイテムを使用したときのメッセージとアニメーションの処理。
    /// </summary>
    async public UniTask UseItem(EffectData effect, string name)
    {
        _canMove = false;
        MessageManager.Instance.ShowMessage(name, "を使った。");
        _animator.SetTrigger(ItemAnim);
        await Locator<IEffectManager>.I.PlayEffect(transform.position, effect, transform.rotation);
    }

    /// <summary>
    /// 通常攻撃　攻撃をした場合はtrueを返す。
    /// </summary>
    async UniTask<bool> Attacked(Vector3Int _attackPosition, bool isPenetrating, int strength, int hitrate)
    {
        int random = UnityEngine.Random.Range(0, 101);
        //失敗
        if (random >hitrate)
        {
            Debug.Log("技外した");
            if (!isPenetrating) { return true; }else{return false;}
        }
        //攻撃範囲に攻撃対象があるかを調べる。
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, _attackPosition.x, _attackPosition.z);
        if (block != null)
        {
            //ブロックかエネミーだった場合は攻撃をする。
            var b = block.Find(x => x._type != objectType.Item);
            if (b !=null)
            {
                await b._object.GetComponent<Damage>().AttackDamage(_status.GetAtk(), strength,b._model);
                if (!isPenetrating) { return true; }
            }
        }
        return false;
    }

    /// <summary>
    /// MPが0以下になる場合はfalse
    /// </summary>
    public bool DownMP(int MP)
    {
        if (_status.GetMP() -MP < 0)
        {
            return false;
        }
        _status.DownMP(MP);
        return true;
    }

    /// <summary>
    /// スター状態になる。
    /// </summary>
    public void SetStar(byte starCount)
    {
        _spAilment._starCount = starCount;
        _spAilment.AddStatusAilment(SpAilment.Star);
    }
    /// <summary>
    /// スター状態を解除
    /// </summary>
    public void ResetStar()
    {
        _spAilment._starCount = 0;
        _spAilment.RemoveSpAilment(SpAilment.Star);
    }

    /// <summary>
    /// HPを減らす。
    /// </summary>
    /// <param name="_hp"></param>
    public void DownHp(int _hp = 1)
    {
        _status.DownHP(_hp);
        //HPが最大値を超えていた場合は最大値にする。
        if (_status.GetHP() > _status.GetMaxHP())
        {
            _status.SetHP(_status.GetMaxHP());
        }
        //プレイヤーのHPが0以下か調べる
        if (!CheckHp())
        {
            return;
        }
        //プレイヤーのHPが半分以下なら自動発動アイテムを使用する。（所持していた場合）
        if(GetHP() <= GetMaxHP()/2)
        {
            ItemManager.Instance.CheckHeartItem<HeelHeartModel>();
        }
    }
    /// <summary>
    /// 空腹度を減らす
    /// </summary>
    public void DownLife(int life = -1)
    {
        //最大値は100
        _life += life;
        if(_life > 100)
        {
            _life = 100;
        }
        //0以下の場合はHPを減らす。
        if(_life <0)
        {
            _life = 0;
            DownHp();
        }
    }
    /// <summary>
    /// mpを減らす
    /// </summary>
    public void DownMp(int _mp = 1)
    {
        _status.DownMP(_mp);
        //最大MPを超えていた場合は最大MPにする。
        if (_status.GetMP() > _status.GetMaxMP())
        {
            _status.SetMP(_status.GetMaxMP());
        }
        //HPが0以下の場合はゲームオーバー
        CheckHp();
    }
    /// <summary>
    /// HPが0以下ならfalse
    /// </summary>
    /// <returns></returns>
    bool CheckHp()
    {
        if(_status.GetHP() <= 0)
        {
            //復活系アイテムを所持していない場合はfalse、所持していたら使用する。
            if(!ItemManager.Instance.CheckHeartItem<HeartModel>())
            {
                GameManager.Instance.GameOver();
                return false;

            }
        }
        return true;
    }

    /// <summary>
    /// アイテム入手
    /// </summary>
    void GetItem(GameObject obj,ItemModel item,Vector3Int _position)
    {
        //バッグに空きがあるかどうかを調べる
        bool getItem = ItemManager.Instance.CanGetItem();
        if (!getItem) {
                MessageManager.Instance.ShowMessage(item._name, "を拾えなかった。");
            return
                ; }
        //アイテムを拾えた場合
        ItemManager.Instance.AddItem(item);
        MessageManager.Instance.ShowMessage(item._name, "を拾った。");
        int id = item._ID;

        //拾ったアイテムはブロックデータから削除する。
        GameManager.Instance._stagemanager.RemoveObjectData(_position.x, _position.z, id, objectType.Item);

        //拾ったアイテムはフィールドから削除する。
        obj.gameObject.SetActive(false);
        Destroy(obj);
    }
    /// <summary>
    /// ゴールに到達したときに使う
    /// </summary>
    async UniTask Restart()
    {
        await GameManager.Instance.InitGame();
    }

    /// <summary>
    /// エネミーに攻撃されたとき
    /// </summary>
    /// <returns></returns>
    async public UniTask EnemyAttack(int enemyAtk,int enemyFixed,Ailment ailment,string _ename)
    {
        //10%の確立で回避する。
        if(UnityEngine.Random.Range(0,11) ==1)
        {
            MessageManager.Instance.ShowMessage(_ename, "の攻撃を回避した！");
            return;
        }
        MessageManager.Instance.ShowMessage(_ename, "の攻撃！");

        //スター状態出ない場合、攻撃を受ける。
        if (!_spAilment.HaveAilment(SpAilment.Star))
        {
            DownHp(CalcDamage(enemyAtk, enemyFixed, _status.GetDef()));
            CheckHp();
            //HPが0以下なら死亡アニメーション
            if (_status.GetHP() <= 0)
            {
                _animator.SetTrigger(DieAnim);
            }
            else
            {
                PlayDamageSE();
                PlayHitAnimation();
            }
            //状態異常付与
            AddAilment(ailment);
            //アニメーションの再生時間の半分を待機する。
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            await UniTask.Delay(TimeSpan.FromSeconds(state.length / 2));
        }
        //スター状態ならスターのカウントを減らす。（スターは一定回数攻撃を受けると効果が切れる）
        else
        {
            _spAilment.StarCountDown();
        }
    }
    /// <summary>
    /// 攻撃がヒットしたときのアニメーション
    /// </summary>
    public void PlayHitAnimation()
    {
        if(_isDefendMode)
        {
            _animator.SetTrigger(DefendHitAnim);
        }
        else
        {
            _animator.SetTrigger(HitAnim);
        }
    }
    /// <summary>
    /// ダメージを受けた時のSE
    /// </summary>
    public void PlayDamageSE()
    {
        SoundManager.Instance.PlaySE(_damageSE);
    }
    /// <summary>
    /// 通常攻撃時のSE
    /// </summary>
    public void PlayAttackSE()
    {
        SoundManager.Instance.PlaySE(_attackSE);
    }
    /// <summary>
    /// エネミーがプレイヤーに与えるダメージの計算
    /// </summary>
    /// <returns></returns>
    int CalcDamage(int enemyAtk,int enemyFixed, int playerDef)
    {
        float guardDamage = 0.3f;
        float damage = (enemyAtk * enemyFixed) / (playerDef + _charactorWeapon.GetArmorStrength()) + 1;
        if(_statusAilment.HaveAilment(Ailment.Burn))
        {
            damage *= 1.1f;
        }
        if (_isDefendMode) { damage *= guardDamage; }
        return Mathf.CeilToInt(damage);
    }


    /// <summary>
    /// 状態異常の追加とエフェクトの再生
    /// </summary>
    /// <param name="ailment"></param>
    public void AddAilment(Ailment ailment)
    {
        if(ailment == Ailment.Sleep && _spAilment.HaveAilment(SpAilment.Insomnia)) { return; }
        if(_spAilment.HaveAilment(SpAilment.Safeguard)) { return; }

        if(ailment == Ailment.Poison || Ailment.BadPoison == ailment)
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Poison, transform.rotation);
        }else if(ailment == Ailment.Curse)
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Curse, transform.rotation);
        }
        else if (ailment == Ailment.Burn)
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Burn, transform.rotation);
        }
        else if (ailment == Ailment.Sleep)
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Sleep, transform.rotation);
        }
        _statusAilment.AddStatusAilment(ailment,"プレイヤー");
    }
    /// <summary>
    /// 状態異常の削除
    /// </summary>
    public void RecoveryAilment(Ailment ailment)
    {
        _statusAilment.RecoveryAIlment(ailment);
    }
    /// <summary>
    /// 状態異常の取得
    /// </summary>
    public HashSet<Ailment> GetAilment()
    {
        return _statusAilment.GetStatusAilment();
    }
    public CharactorStatus GetStat()
    {
        return _status;
    }

    /// <summary>
    /// プレイヤーが装備している武器のモデルをセットして表示する
    /// </summary>
    public void SetWeaponObject(ItemModel item)
    {
        if(item == null)
        {
            foreach (var weapon in _charactorWeapon._weaponR)
            {
                weapon.gameObject.SetActive(false);
            }
            return;
        }
        foreach (var weapon in _charactorWeapon._weaponR)
        {
            if (item._name == weapon.name)
            {
                weapon.gameObject.SetActive(true);
                _charactorWeapon.SetWeapon(item);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// プレイヤーが装備している盾のモデルをセットして表示する
    /// </summary>
    public void SetArmorObject(ItemModel item)
    {
        if (item == null)
        {
            foreach (var weapon in _charactorWeapon._armorL)
            {
                weapon.gameObject.SetActive(false);
            }
            return;
        }
        foreach (GameObject armor in _charactorWeapon._armorL)
        {
            if (item._name == armor.name)
            {
                armor.gameObject.SetActive(true);
                _charactorWeapon.SetArmor(item);
            }
            else
            {
                armor.gameObject.SetActive(false);
            }
        }
    }

    async public UniTask AddExp(int exp)
    {
        await _status.AddExp(exp);
    }
    public void AddBuff(SpAilment spailment)
    {
        _spAilment.AddStatusAilment(spailment);
    }

    public void SetHP(int hp)
    {
        _status.SetHP(hp);
    }
    public void SetMP(int mp)
    {
        _status.SetMP(mp);
    }
    public void AddAilments(int[] ailments)
    {
        _statusAilment.AddAilments(ailments);
    }
    public void SetTotalExp(int totalexp)
    {
        _status.SetTotalExp(totalexp);
    }
    public int GetTotalExp()
    {
        return _status._totalExp;
    }
    public int RequiredExp()
    {
        return _status.RequiredExp();
    }
    public int GetLv()
    {
        return _status.GetLv();
    }
    public int GetMaxHP()
    {
        return _status.GetMaxHP();
    }
    public int GetHP()
    {
        return _status.GetHP();
    }
    public int GetMaxMP()
    {
        return _status.GetMaxMP();
    }
    public int GetMP()
    {
        return _status.GetMP();
    }
    public int GetAtk()
    {
        return _status.GetAtk();
    }
    public int GetDef()
    {
        return _status.GetDef();
    }
    public Skill[] GetSkillList()
    {
        return _playerSkill.GetSkillList();
    }
    public void SetSkill(AbstractSkill skill, Skill _forgettingSkill)
    {
        _playerSkill.SetPlayerSkill(skill, _forgettingSkill);
    }
}