using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SkillLongAttack :AbstractSkill
{
    async UniTask SkillAttack()
    {
        float damageMag = 1;
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.EightFoword, GameManager.Instance._player.transform);
        foreach (var atkPos in attackPosition)
        {
            await Attacked(atkPos, damageMag);
            damageMag -= 0.1f;
        }
    }
    async UniTask Attacked(Vector3Int _attackPosition , float damageMag)
    {
        var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
        //周囲16ます。
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            var b = block.Find(x => x._type != objectType.Item);
            if (b != null)
            {
                await b._object.GetComponent<Damage>().AttackDamage(GameManager.Instance._player.GetAtk(), Mathf.CeilToInt((_model._strength + weaponS) * damageMag), b._model);
            }
        }
    }

    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

}
