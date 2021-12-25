using Cysharp.Threading.Tasks;
using System;


public class SkillBadPoison : AbstractSkill
{
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

    async UniTask SkillAttack()
    {
        GameManager.Instance._player.AddAilment(Ailment.BadPoison);
        var enemies = GameManager.Instance.GetEnemies();
        foreach (var e in enemies)
        {
            e.AddAilment(Ailment.BadPoison).Forget();
        }
    }
}