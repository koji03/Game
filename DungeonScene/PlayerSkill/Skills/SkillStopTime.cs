using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class SkillStopTime : AbstractSkill
{

    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }
    IDisposable[] stopTime = new IDisposable[2];

    async UniTask SkillAttack()
    {
        GameManager.Instance.AddStop();
        var endturn = GameManager.Instance._turnNum + 5;
        stopTime[0] = GameManager.Instance.ObserveEveryValueChanged(x => x._turnNum >= endturn).Skip(1).Subscribe(_ => Subscribe());
        stopTime[1] = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
        GameManager.Instance.StopAllEnemyAnimation();
    }
    void Subscribe()
    {
        GameManager.Instance.RemoveStop();
        GameManager.Instance.StartAllEnemyAnimation();
        foreach (var i in stopTime)
        {
            i.Dispose();
        }
    }
}