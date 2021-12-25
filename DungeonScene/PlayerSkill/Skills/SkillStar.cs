using Cysharp.Threading.Tasks;
using System;
using UniRx;

public class SkillStar :AbstractSkill
{
    IDisposable sub;
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

    async UniTask SkillAttack()
    {
        GameManager.Instance._player.SetStar(5);
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => PlayerReset());
    }
    public void PlayerReset()
    {
        GameManager.Instance._player.ResetStar();
        sub.Dispose();
    }
}
