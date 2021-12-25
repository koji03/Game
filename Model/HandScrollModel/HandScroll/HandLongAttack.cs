using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandLongAttack
{
    int strength = 20;
    async public UniTask Play()
    {
        //前方マスを取得
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.EightFoword, GameManager.Instance._player.transform);
        foreach (var atkPos in attackPosition)
        {
            await Attacked(atkPos);
        }
    }
    async UniTask Attacked(Vector3Int _attackPosition)
    {
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
