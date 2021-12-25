using System;
using UniRx;

public class HandSaving
{
    IDisposable sub;
    public void Play()
    {
        GameManager.Instance._player.AddBuff(SpAilment.MPHalf);
        //次の階層に進んだら終わる
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
    }
    void Subscribe()
    {
        GameManager.Instance._player._spAilment.RemoveSpAilment(SpAilment.MPHalf);
        sub.Dispose();
    }
}
