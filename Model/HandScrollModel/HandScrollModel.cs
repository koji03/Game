using Cysharp.Threading.Tasks;
using UnityEngine;

public enum ScrollType
{
    Curse = 1,
    LongAttack = 2,
    Move = 3,
    LvUp = 4,
    Poison = 5,
    MPHalf = 6,
    AllSleep = 7,
    Insomnia = 8,
    Motivation = 9,
    FullHeal = 10,
    VEIL = 11,
    Around = 12,
    AroundSP = 13,
    LongSP = 14,
    AroundCurse =15,
    AroundSleep = 16
}
[CreateAssetMenu(fileName = "HandScroll", menuName = "ScriptableObjects/ItemData/HandScroll")]
public class HandScrollModel : ItemModel
{
    [SerializeField] ScrollType _scrollType;
    async public override UniTask<bool> UseItem()
    {
        await GameManager.Instance._player.UseItem(_effect, _name);
        switch (_scrollType)
        {
            case ScrollType.Curse:
                await new HandCurse().Play();
                break;
            case ScrollType.LongAttack:
                HandLongAttack HandLongAttack = new HandLongAttack();
                await HandLongAttack.Play();
                break;
            case ScrollType.Move:
                HandMove HandMove = new HandMove();
                HandMove.Play();
                break;
            case ScrollType.LvUp:
                HandLvUp HandLvUp = new HandLvUp();
                await HandLvUp.Play();
                break;
            case ScrollType.Poison:
                HandPoison HandPoison = new HandPoison();
                HandPoison.Play();
                break;
            case ScrollType.MPHalf:
                HandSaving HandSaving = new HandSaving();
                HandSaving.Play();
                break;
            case ScrollType.AllSleep:
                HandAllSleep HandAllSleep = new HandAllSleep();
                await HandAllSleep.Play();
                break;
            case ScrollType.Insomnia:
                HandInsomnia HandInsomnia = new HandInsomnia();
                HandInsomnia.Play();
                break;
            case ScrollType.Motivation:
                HandMotivation HandMotivation = new HandMotivation();
                HandMotivation.Play();
                break;
            case ScrollType.FullHeal:
                HandFullHeal HandFullHeal = new HandFullHeal();
                HandFullHeal.Play();
                break;
            case ScrollType.VEIL:
                HandVeil HandVeil = new HandVeil();
                HandVeil.Play();
                break;
            case ScrollType.Around:
                HandAround HandAround = new HandAround();
                await HandAround.Play();
                break;
            case ScrollType.AroundSP:
                HandAroundSP HandAroundSP = new HandAroundSP();
                await HandAroundSP.Play();
                break;
            case ScrollType.LongSP:
                HandLongSP HandLongSP = new HandLongSP();
                await HandLongSP.Play();
                break;
            case ScrollType.AroundCurse:
                HandAroundCurse HandAroundCurse = new HandAroundCurse();
                await HandAroundCurse.Play();
                break;
            case ScrollType.AroundSleep:
                HandAroundSeep HandAroundSeep = new HandAroundSeep();
                await HandAroundSeep.Play();
                break;
        }
        return true;
    }
}
