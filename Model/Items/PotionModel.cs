using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Position", menuName = "ScriptableObjects/ItemData/Position")]

public class PotionModel : ItemModel
{
    public enum Buff
    {
        Atk,
        Def,
        SPAtk,
        SPDef
    }
    [SerializeField] BuffData[] _buffDatas;
    public override async UniTask<bool> UseItem()
    {
        //指定されたバフをプレイヤーに与える。
        foreach(var data in _buffDatas)
        {
            switch (data._buttType)
            {
                case Buff.Atk:
                    GameManager.Instance._player.AddBuff(SpAilment.AtkBuff);
                    break;
                case Buff.Def:
                    GameManager.Instance._player.AddBuff(SpAilment.DefBuff);
                    break;
                case Buff.SPAtk:
                    GameManager.Instance._player.AddBuff(SpAilment.AtkBuff);
                    GameManager.Instance._player.DownHp(-40);
                    break;
                case Buff.SPDef:
                    GameManager.Instance._player.AddBuff(SpAilment.DefBuff);
                    GameManager.Instance._player.DownMp(-40);
                    break;

            }
        }
        await GameManager.Instance._player.UseItem(_effect,_name);
        return true;
    }
    [System.Serializable]
    public class BuffData
    {
        public Buff _buttType;
    }
}
