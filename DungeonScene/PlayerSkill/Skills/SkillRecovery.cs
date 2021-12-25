using Cysharp.Threading.Tasks;
using System;


public class SkillRecovery : AbstractSkill
{
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

    async UniTask SkillAttack()
    {
        GameManager.Instance._player._statusAilment.AllRecovery();
    }
}