using Cysharp.Threading.Tasks;
using System;

public class SkillGluttony : AbstractSkill
{

    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

    async UniTask SkillAttack()
    {
        Player p = GameManager.Instance._player;
        p.DownLife(100);
    }

}
