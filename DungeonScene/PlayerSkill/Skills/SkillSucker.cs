using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
public class SkillSucker : AbstractSkill
{
    async UniTask SkillAttack()
    {
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.TweFoword, GameManager.Instance._player.transform);
        foreach (var atkPos in attackPosition)
        {
            await Attacked(atkPos);
        }
    }
    async UniTask Attacked(Vector3Int _attackPosition)
    {
        var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            var b = block.Find(x => x._type != objectType.Item);
            if (b != null)
            {
                await b._object.GetComponent<Damage>().AttackDamage(GameManager.Instance._player.GetAtk(), _model._strength+ weaponS, b._model);
            }
        }
    }

    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

}
