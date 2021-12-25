using System;
using UniRx;
public class HandInsomnia
{
    IDisposable sub;
    public void Play()
    {
        GameManager.Instance._player.AddBuff(SpAilment.Insomnia);
        //次の階層に進んだら終了
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
    }

    void Subscribe()
    {
        GameManager.Instance._player._spAilment.RemoveSpAilment(SpAilment.Insomnia);
        sub.Dispose();
    }
}
