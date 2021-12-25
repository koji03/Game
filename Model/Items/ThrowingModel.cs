using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System.Linq;

[CreateAssetMenu(fileName = "Throwing", menuName = "ScriptableObjects/ItemData/Throwing")]
public class ThrowingModel : ItemModel
{
    [SerializeField] AttackDirection _attackDirection;
    [Tooltip("trueなら貫通する")]
    [SerializeField] bool _isPenetrating = true;
    [SerializeField] int _strength = 1;
    [SerializeField] int hitrate = 100;
    [SerializeField] GameObject _throwObject;

    //投擲アイテム（矢）を使う。
    async public override UniTask<bool> UseItem()
    {
        float moveSpeed = 16f;

        Player player = GameManager.Instance._player;

        //エフェクトを再生。
        Locator<IEffectManager>.I.PlayEffect(GameManager.Instance._player.transform.position, _effect, GameManager.Instance._player.transform.rotation).Forget();
        
        //アイテムの攻撃する範囲を取得
        var positions = AttackPositions.GetAttackPosition(_attackDirection, player.transform);

        //投擲アイテムの一番遠い場所を取得
        Vector3 endposition = positions[0];
        for (int i = 0; i < positions.Length; i++)
        {
            var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(positions[i].x), Mathf.RoundToInt(positions[i].z));
            //敵がいたら終わらせる。
            //壁の外に出たら終わらせる
           　if (block != null)
            {
                if(block.Any(x => x._type != objectType.Item))
                {
                    endposition = positions[i];
                    break;
                }
            }
            endposition = positions[i];
            if (!GameManager.Instance._stagemanager.IsIn(endposition))
            {
                break;
            }
        }
        //投擲アイテムを作成
        var throwItem = Instantiate(_throwObject);

        //投擲アイテムの初期位置をプレイヤーの場所にする。
        throwItem.transform.position = GameManager.Instance._player.transform.position;

        //投擲アイテムの終点の距離を取得し、移動時間を計算する。
        float distance = Vector3.Distance(GameManager.Instance._player.transform.position, endposition);
        float moveTime = distance / moveSpeed;

        //投擲アイテムの初期位置と終点の位置を少し上に動かす。
        throwItem.transform.position += Vector3.up * 0.2f;
        endposition = endposition + Vector3.up * 0.2f;

        //矢の向きを正しい方向に向けさせる。
        var quaternion = Quaternion.Euler(90, 0, 0);
        throwItem.transform.LookAt(endposition);
        throwItem.transform.rotation *= quaternion;
        
        //矢の移動をする。
        await throwItem.transform.DOMove(endposition, moveTime).SetEase(Ease.Linear).ToUniTask();

        //移動が終われば消す。
        throwItem.SetActive(false);
        var result = await player.OnAttack(_attackDirection, _isPenetrating, _strength , hitrate,false);
        Destroy(throwItem);
        return result;
    }
}
