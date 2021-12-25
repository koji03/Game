using Cysharp.Threading.Tasks;
using System;
using System.Linq;

public class SkillMeteorShower : AbstractSkill
{
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

    async UniTask SkillAttack()
    {
        var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
        StageManager stage = GameManager.Instance._stagemanager;
        foreach(var block in stage._objectsData._objects)
        {
            if(block == null) { continue; }
            var obj = block.Find(x => x._type != objectType.Item);
            if (obj !=null)
            {
                await obj._object.GetComponent<Damage>().AttackDamage(GameManager.Instance._player.GetAtk(),_model._strength + weaponS, obj._model);
                await UniTask.Delay(1000);
            }
        }
    }
}
