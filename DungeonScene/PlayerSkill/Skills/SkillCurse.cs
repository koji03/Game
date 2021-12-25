using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SkillCurse : AbstractSkill
{
    async UniTask SkillAttack()
    {
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.Foword, GameManager.Instance._player.transform);
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
            var b = block.Find(x => x._type == objectType.Enemy);
            if (b != null)
            {
                await b._object.GetComponent<Enemy>().AddAilment(Ailment.Curse);
            }
        }
    }
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

}