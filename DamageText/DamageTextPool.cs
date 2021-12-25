using System;
using UniRx;
using UniRx.Toolkit;
public class DamageTextPool : AsyncObjectPool<DamageText>
{
    private readonly DamageText _original;


    public DamageTextPool(DamageText original)
    {
        _original = original;
        _original.gameObject.SetActive(false);
    }

    protected override IObservable<DamageText> CreateInstanceAsync()
    {
        var text = DamageText.Instantiate(_original, _original.transform.parent);

        return Observable.Return(text);
    }


}
