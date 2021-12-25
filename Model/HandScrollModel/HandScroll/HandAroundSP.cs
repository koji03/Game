using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandAroundSP : MonoBehaviour
{
    int strength = 60;

    async public UniTask Play()
    {
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.TwoSquaresAround, GameManager.Instance._player.transform);
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
