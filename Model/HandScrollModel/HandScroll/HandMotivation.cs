using System;
using UniRx;
public class HandMotivation
{
    IDisposable sub;
    public void Play()
    {
        GameManager.Instance.AddHalf();
        //次の階層に進んだら終了
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
    }

    void Subscribe()
    {
        GameManager.Instance.RemoveHalf();
        sub.Dispose();
    }
}
