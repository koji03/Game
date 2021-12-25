using Cysharp.Threading.Tasks;
using System;

public class SkillHeel :AbstractSkill
{


    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

    async UniTask SkillAttack()
    {
        Player p = GameManager.Instance._player;
        p.DownHp(-p.GetMaxHP()/2);
        await UniTask.Yield();
    }
 
}
