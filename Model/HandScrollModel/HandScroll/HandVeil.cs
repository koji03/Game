using System;
using UniRx;

public class HandVeil
{
    IDisposable sub;
    public void Play()
    {
        GameManager.Instance._player.AddBuff(SpAilment.Safeguard);
        //���̊K�w�ɐi�񂾂�I���
        sub = GameManager.Instance.ObserveEveryValueChanged(x => x._currentStage).Skip(1).Subscribe(_ => Subscribe());
    }
    void Subscribe()
    {
        GameManager.Instance._player._spAilment.RemoveSpAilment(SpAilment.Safeguard);
        sub.Dispose();
    }
}
