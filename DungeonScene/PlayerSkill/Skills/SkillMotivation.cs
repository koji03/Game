using Cysharp.Threading.Tasks;
using System;
using UniRx;
public class SkillMotivation : AbstractSkill
{
    IDisposable sub;
    async UniTask SkillAttack()
    {
        GameManager.Instance.AddHalf();
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
    }
    void Subscribe()
    {
        GameManager.Instance.RemoveHalf();
        sub.Dispose();
    }
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

}
