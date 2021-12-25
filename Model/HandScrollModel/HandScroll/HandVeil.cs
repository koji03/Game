using System;
using UniRx;

public class HandVeil
{
    IDisposable sub;
    public void Play()
    {
        GameManager.Instance._player.AddBuff(SpAilment.Safeguard);
        //ŽŸ‚ÌŠK‘w‚Éi‚ñ‚¾‚çI‚í‚é
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
    }
    void Subscribe()
    {
        GameManager.Instance._player._spAilment.RemoveSpAilment(SpAilment.Safeguard);
        sub.Dispose();
    }
}
