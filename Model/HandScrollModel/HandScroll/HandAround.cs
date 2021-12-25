using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandAround : MonoBehaviour
{
    int strength = 20;

　　//周囲に攻撃
    async public UniTask Play()
    {
        var attackPosition = GetAttackPosition();
        foreach (var atkPos in attackPosition)
        {
            await Attack(atkPos);
        }
    }
    Vector3Int[] GetAttackPosition()
    {
        //攻撃するマスを取得
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.TwoSquaresAround, GameManager.Instance._player.transform);
        return attackPosition;

    }
    async UniTask Attack(Vector3Int _attackPosition)
    {
        //エネミーであればダメージを与える。
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            var b = block.Find(x => x._type != objectType.Item);
            if (b != null)
            {
                await b._object.GetComponent<Damage>().AttackDamage(GameManager.Instance._player.GetAtk(), strength, b._model);
            }
        }
    }

}
