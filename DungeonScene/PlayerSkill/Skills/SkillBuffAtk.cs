using Cysharp.Threading.Tasks;
using System;
public class SkillBuffAtk : AbstractSkill
{

    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }
    async UniTask SkillAttack()
    {
        GameManager.Instance._player.AddBuff(SpAilment.AtkBuff);
        GameManager.Instance._player.AddBuff(SpAilment.DefBuff);
        await UniTask.Yield();
    }
}
