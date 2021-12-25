using UnityEngine;

public class AilmentsEffectData : MonoBehaviour
{
    //エフェクトデータ
    public EffectData poisonEffect;
    public EffectData curseEffect;
    public EffectData burnEffect;
    public EffectData sleepEffect;
    public EffectData sunnyEffect;
    public EffectData hailEffect;
    public EffectData nullEffect;

    //エフェクトのデータを返す。
    public EffectData GetAilmentEffect(Ailment ailment)
    {
        switch(ailment)
        {
            case Ailment.Poison:
                return poisonEffect;
            case Ailment.BadPoison:
                return poisonEffect;
            case Ailment.Curse:
                return curseEffect;
            case Ailment.Burn:
                return burnEffect;
            case Ailment.Sleep:
                return sleepEffect;
        }
        return nullEffect;
    }

}
