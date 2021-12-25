using Cysharp.Threading.Tasks;
using System;

public class SkillNull : AbstractSkill
{
    public override Func<UniTask> GetSkill()
    {
        return null;
    }

}
