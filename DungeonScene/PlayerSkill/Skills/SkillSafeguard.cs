using Cysharp.Threading.Tasks;
using System;
using UniRx;
public class SkillSafeguard : AbstractSkill
{
    IDisposable sub;
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }
    async UniTask SkillAttack()
    {
        GameManager.Instance._player.AddBuff(SpAilment.Safeguard);
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
        await UniTask.Yield();
    }
    void Subscribe()
    {
        GameManager.Instance._player._spAilment.RemoveSpAilment(SpAilment.Safeguard);
        sub.Dispose();
    }
}