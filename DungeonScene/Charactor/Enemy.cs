using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UniRx;

[RequireComponent(typeof(Damage))]
[RequireComponent(typeof(CharactorStatus))]
[RequireComponent(typeof(StatusAilment))]
public class Enemy : MonoBehaviour
{
    bool _isMoving = false;//現在動いているかの判定

    Animator _animator;
    [NonSerialized]public EnemyModel _model;
    [NonSerialized]public Damage _damage;
    [NonSerialized] public CharactorStatus _charactorStatus;
    [NonSerialized] public StatusAilment _ailment;

    string WalkAnimKey = "Walk";
    string AttackAnimKey = "Attack";
    string DieAnimKey = "Die";
    string HitAnimKey = "Hit";


    /// <summary>
    /// エネミーを倒したときに貰える経験値
    /// </summary>
    public int GetExp()
    {
        //プレイヤーのレベルが現在の階層+7以上になったらもらえる経験値が少なるなる。
        int diff =Mathf.Max(0,GameManager.Instance._currentStage+7-GameManager.Instance._player.GetLv());
        var exp = Mathf.Floor(_charactorStatus._baseExp * GameManager.Instance._currentStage*0.8f);
        float num =GameManager.Instance._player.GetLv() - diff;
        num = (num <= 0) ? 1 : num; 
        var exp2 = Mathf.FloorToInt(exp * (GameManager.Instance._currentStage / num));
        return exp2;
    }
    IDisposable _damagescaleSub;

    public void DisposeSub()
    {
        _damagescaleSub.Dispose();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public void EnemyInit(int stageLevel,EnemyModel _model)
    {
        this._model = _model;
        _animator = GetComponent<Animator>();
        _damage = GetComponent<Damage>();
        _charactorStatus = GetComponent<CharactorStatus>();
        _ailment = GetComponent<StatusAilment>();
        gameObject.AddComponent<StatusSpAilment>();
        _damage.Init();
        _charactorStatus.Init(_model,stageLevel);

        //エネミーは同一ターン内で攻撃受けるたびにダメージ倍率が少しずつ上がる。
        //次のターンになったらその倍率を戻すための処理。
        _damagescaleSub = GameManager.Instance.ObserveEveryValueChanged(x => x._turnNum).Subscribe(_=> ResetDamagescale());
    }
    /// <summary>
    /// ダメージ倍率を戻す
    /// </summary>
    void ResetDamagescale()
    {
        _damage._damageScale = 1;
    }

    /// <summary>
    /// エネミーの移動
    /// </summary>
    async public UniTask MoveEnemy()
    {
        if(gameObject != null)
        {
            //状態異常の処理
            await AilmentAction();
            //エネミーの移動
            await ATMove();
        }
    }
    /// <summary>
    /// 状態異常の処理
    /// </summary>
    async UniTask AilmentAction()
    {
        //毒の場合はダメージを受ける
        if(_ailment.HaveAilment(Ailment.Poison))
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Poison, transform.rotation).Forget();
            await _damage.FixedDamage(1);
        }
        //猛毒の場合は大きなダメージを受ける
        if(_ailment.HaveAilment(Ailment.BadPoison))
        {
            Locator<IEffectManager>.I.PlayAilmentEffect(transform.position,Ailment.Poison, transform.rotation).Forget();
            var damage = _charactorStatus.GetMaxHP() / 8;
            await _damage.FixedDamage(damage);
        }
    }
    /// <summary>
    /// 移動と攻撃の処理
    /// </summary>
    async UniTask ATMove()
    {
        //眠り状態の時は移動しない
        if (_ailment.HaveAilment(Ailment.Sleep))
        {
            return;
        }
        //プレイヤーの方向を向く。
        Rotation(GameManager.Instance._stagemanager._playerNextPosition);

        //プレイヤーのポジションを取得
        var _pPosition = GameManager.Instance._stagemanager._playerNextPosition;
        bool isAttack = false;
        
        //プレイヤーがエネミーの攻撃範囲にいたら攻撃をする。
        var attackPosition = AttackPositions.GetAttackPosition(_model._attackDirection, transform);
        isAttack = attackPosition.Any(x => x == _pPosition);

        //攻撃をする場合
        if (isAttack)
        {
            //攻撃モーションを行う。
            await AttackMotion();
            //全ての攻撃範囲を調べる。エネミー側からチェックをする。
            foreach (var atP in attackPosition)
            {
                //プレイヤーとエネミーの間にブロックがあるかを調べる。
                var check = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, atP.x, atP.z);
                if(check !=null)
                {
                    var block = check.Find(x => x._type == objectType.Block);
                    if (block != null)
                    {
                        //ブロックがある場合はブロックに攻撃をする。
                        await block._object.GetComponent<Damage>().AttackDamage(_charactorStatus.GetAtk(), 0, block._model);
                        //貫通攻撃でない場合は処理を終わる。
                        if(!_model._isPenetrating)
                        {
                            break;
                        }
                    }
                }
                //プレイヤーに攻撃する。
                if (_pPosition == atP)
                {
                    var positions = AttackPositions.GetAttackPosition(AttackDirection.Foword, transform);
                    Locator<IEffectManager>.I.PlayEffect(positions[0], _model._effectData, transform.rotation).Forget();

                    //エネミーの攻撃で状態異常を付与するかどうか
                    int chance = UnityEngine.Random.Range(0, 101);
                    Ailment ailment = (_model._ailmentData._chance > chance) ? _model._ailmentData._ailment : Ailment.Null;

                    //攻撃処理.
                    await GameManager.Instance._player.EnemyAttack(_charactorStatus.GetAtk(), _model._fixedAtk, ailment,_model._name);
                }
            }
        }
        //攻撃をしない倍
        else
        {
            //のろい状態なら動けないのでエフェクトを再生して終わる。
            if (_ailment.HaveAilment(Ailment.Curse))
            {
                Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Curse, transform.rotation).Forget();
                return;
            }
            //動く
            await Move();
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    /// <returns>エネミーが動いたかどうか</returns>
    async UniTask<bool> Move()
    {
        //エネミーが移動するマスの番号を取得。
        var index = GetRoute();

        //番号からxとzのポジションを計算
        var x =GameManager.Instance._stagemanager.i2x(index);
        var z =GameManager.Instance._stagemanager.i2z(index);

        Vector3 start = transform.position;
        Vector3 moveEnd = new Vector3(x, 0, z);

        //プレイヤーの方向を向く
        if (moveEnd - transform.position == Vector3.zero)
        {
            Rotation(GameManager.Instance._stagemanager._playerNextPosition);
        }
        else
        {
            //移動先を向く
            Rotation(moveEnd);
        }
        start.y = 0;

        Vector3 _playerPosition = GameManager.Instance._stagemanager._playerNextPosition;
        //自分の周囲のマスにプレイヤーがいるかを確認し、いない場合は移動する。
        foreach(var direction in GameManager.Instance._positions)
        {
            if(_playerPosition != transform.position + new Vector3(direction.Value[0], _playerPosition.y, direction.Value[1]))
            {
                if (!_isMoving)
                {
                    //動く前にいたマスのデータからエネミーを削除
                    GameManager.Instance._stagemanager.RemoveObjectData(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), _model._ID, objectType.Enemy);
                    //新しい移動先のマスにエネミーを追加
                    GameManager.Instance._stagemanager.AddEnemyObject(index, _model, gameObject);
                    //移動
                    await Movement(moveEnd);
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 動く
    /// </summary>
    async UniTask Movement(Vector3 end)
    {
        _isMoving = true;

        //移動先との距離を計算
        float remainingDistance = (transform.position - end).sqrMagnitude;

        PlayAnimation(WalkAnimKey);
        _animator.speed = 10;

        //移動する。
        while (remainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, end, 5f * Time.deltaTime);
            remainingDistance = (transform.position - end).sqrMagnitude;
            await UniTask.Yield();
        }
        _animator.speed = 1;
        //ずれを正す。
        transform.position = end;
        _isMoving = false;
    }

    /// <summary>
    /// ターゲットがある方向に向く
    /// </summary>
    void Rotation(Vector3 _target)
    {
        var direction = _target - transform.position;
        direction.y = 0;
       // Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

        //x軸がおかしくなって下を向いたりするときがある→初期位置が浮いているから
        transform.LookAt(_target);
    }

    /// <summary>
    /// 攻撃のアニメーション
    /// </summary>
    async UniTask AttackMotion()
    {
        Rotation(GameManager.Instance._stagemanager._playerNextPosition);
        PlayAnimation(AttackAnimKey);
        await UniTask.Yield();
        //アニメの再生時間の半分の時間待機。
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(TimeSpan.FromSeconds(state.length/2));
    }

    /// <summary>
    /// 移動ルートを調べ、移動するマスの番号を返す。
    /// </summary>
    int GetRoute()
    {
        //エネミーとプレイヤーのマスを取得
        var enemyIndex = GameManager.Instance._stagemanager.xz2i(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        var playerIndex = GameManager.Instance._stagemanager.xz2i(Mathf.RoundToInt(GameManager.Instance._stagemanager._playerNextPosition.x), Mathf.RoundToInt(GameManager.Instance._stagemanager._playerNextPosition.z));

        //エネミーからプレイヤーまでの最短ルートのマスが入っている配列を取得
        var root =  Slover.Solve(
            GameManager.Instance._stagemanager._row, GameManager.Instance._stagemanager._colum, 
            enemyIndex, playerIndex);
        int rootInt = 0;
        //移動しない場合はエネミーの現在のポジションからマスの番号を計算する。
        if(root.Count <3)
        {
            rootInt = GameManager.Instance._stagemanager.xz2i(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        }
        else
        {
            rootInt = root[1];
        }
        return rootInt;
    }
    /// <summary>
    /// ダメージを受けた時のアニメーション
    /// </summary>
    public async UniTask HitAnimation(int _hp)
    {
        if (_hp <= 0)
        {
            PlayAnimation(DieAnimKey);
        }
        else
        {
            PlayAnimation(HitAnimKey);
        }
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(1000);
    }
    /// <summary>
    /// アニメーションの再生。
    /// </summary>
    /// <param name="anim"></param>
    void PlayAnimation(string anim)
    {
        //プレイヤーのスキルでエネミーが動けない場合は再生しない
        if(GameManager.Instance.GetIsStop())
        {
            return;
        }
        _animator.SetTrigger(anim);
    }
    /// <summary>
    /// アニメーションを止める。
    /// </summary>
    public void StopAnimation()
    {
        _animator.enabled =false;
    }
    /// <summary>
    /// アニメーションを再開する。
    /// </summary>
    public void StartAnimation()
    {
        _animator.enabled = true;
    }
    /// <summary>
    /// エネミーに状態異常を付与とそのエフェクトの再生をする。
    /// </summary>
    async public UniTask AddAilment(Ailment ailment)
    {
        if(_charactorStatus.GetHP() <= 0) { return; }
        if (ailment == Ailment.Poison || Ailment.BadPoison == ailment)
        {
            await Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Poison, transform.rotation);
        }
        else if (ailment == Ailment.Curse)
        {
           await Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Curse, transform.rotation);
        }
        else if (ailment == Ailment.Burn)
        {
            await Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Burn, transform.rotation);
        }
        else if (ailment == Ailment.Sleep)
        {
            await Locator<IEffectManager>.I.PlayAilmentEffect(transform.position, Ailment.Sleep, transform.rotation);
        }
        _ailment.AddStatusAilment(ailment, _model._name);
    }
}
