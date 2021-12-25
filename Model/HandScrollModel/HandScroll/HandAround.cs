using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandAround : MonoBehaviour
{
    int strength = 20;

�@�@//���͂ɍU��
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
        //�U������}�X���擾
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.TwoSquaresAround, GameManager.Instance._player.transform);
        return attackPosition;

    }
    async UniTask Attack(Vector3Int _attackPosition)
    {
        //�G�l�~�[�ł���΃_���[�W��^����B
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
