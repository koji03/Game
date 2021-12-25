using System;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class Damage : MonoBehaviour
{
    [NonSerialized]public Enemy _enemy;
    [NonSerialized] StatusAilment _statusAilment;
    public void Init()
    {
        _damageScale = 1;
        _enemy = GetComponent<Enemy>();
        _statusAilment = GetComponent<StatusAilment>();
    }
    [NonSerialized]public float _damageScale = 1;

    /// <summary>
    /// 通常の計算のダメージ
    /// </summary>
    public async UniTask<int> AttackDamage(int playerAtk, int strength,Model _model)
    {
        //エネミーの場合
        if (gameObject.CompareTag("Enemy"))
        {
            var eM = _model as EnemyModel;
            GameManager.Instance._player.PlayAttackSE();
            int enemyDef = _enemy._charactorStatus.GetDef();
            //ダメージを計算
            int damage = CalcDamage(playerAtk, strength, enemyDef,eM._fixedDef);
            //HPを減らす
            _enemy._charactorStatus.DownHP(damage);

            //ダメージが0より高いならメッセージを出す
            if(damage > 0)
            {
                MessageManager.Instance.ShowMessage(_model._name, "に", damage.ToString(),"ダメージを与えた!");
            }

            //アニメーションを再生
            await _enemy.HitAnimation(_enemy._charactorStatus.GetHP());
            //HPが0以下なら
            if (_enemy._charactorStatus.GetHP() <= 0)
            {
                //プレイヤーに経験値を与える。
                await GameManager.Instance._player.AddExp(_enemy.GetExp());

                //エネミーのオブジェクトとデータを削除
                GameManager.Instance._stagemanager.RemoveObjectData(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), _enemy._model._ID, objectType.Enemy);
                GameManager.Instance.DestoryEnemyToList(_enemy);
                _enemy.DisposeSub();
                gameObject.SetActive(false);
            }
            return damage;
        }
        else if(gameObject.CompareTag("Block"))
        {
            //ブロックの場合はダメージ計算せずに破壊する。
            GameManager.Instance._player.PlayDamageSE();
            var blovkModel =  (BlockModel)_model;
            await blovkModel.ActionAfterDestroyed(transform.position);
            gameObject.SetActive(false);
            GameManager.Instance._stagemanager.RemoveObjectData(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), _model._ID, objectType.Block);
            return 0;
        }
        return 0;
    }
    /// <summary>
    /// 固定ダメージ
    /// </summary>
    async public UniTask FixedDamage(int damage)
    {
        if (gameObject.CompareTag("Enemy"))
        {
            //固定ダメージなので計算せずに直接減らす。
            _enemy._charactorStatus.DownHP(damage);
            //アニメーションの再生
            await _enemy.HitAnimation(_enemy._charactorStatus.GetHP());

            //hpが0以下ならオブジェクトとデータを削除
            if (_enemy._charactorStatus.GetHP() <= 0)
            {
                await GameManager.Instance._player.AddExp(_enemy.GetExp());
                GameManager.Instance._stagemanager.RemoveObjectData(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z), _enemy._model._ID, objectType.Enemy);
                GameManager.Instance.DestoryEnemyToList(_enemy);
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ダメージの計算
    /// プレイヤーの攻撃力、武器の威力（スキルの威力）レベル、防御力
    /// </summary>
    int CalcDamage(int playerAtk,int strength,int enemyDef,int fixedDef)
    {
        float damage = 
            (playerAtk * strength) / (enemyDef + fixedDef);
        if(_statusAilment.HaveAilment(Ailment.Burn))
        {
            damage *= 1.1f;
        }
        int d = Mathf.CeilToInt(Mathf.Max(1,damage * _damageScale));
        _damageScale += GameManager.Instance._damageIncrement;
        return d;
    }
}