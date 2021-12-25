using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBurnAttack : AbstractSkill
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
        //周囲16ます。
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            var b = block.Find(x => x._type != objectType.Item);
            if (b != null)
            {
                var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
                await b._object.GetComponent<Damage>().AttackDamage(GameManager.Instance._player.GetAtk(), _model._strength + weaponS, b._model);
                if(b._model is EnemyModel)
                {
                    int random = UnityEngine.Random.Range(0, 5);
                    if (random != 0) { return; }
                    var e = b._object.GetComponent<Enemy>();
                    await e.AddAilment(Ailment.Burn);
                }
            }
        }
    }
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

}
