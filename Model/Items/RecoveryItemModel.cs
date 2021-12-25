using Cysharp.Threading.Tasks;
using UnityEngine;



[CreateAssetMenu(fileName = "Recovery", menuName = "ScriptableObjects/ItemData/Recovery")]
public class RecoveryItemModel : ItemModel
{
    public enum Type
    {
        HP, MP, LIFE, NONE,SPLIFE
    }
    [SerializeField] RecoevryData[] _recoevryDatas;
    public override  async UniTask<bool> UseItem()
    {
        //催眠状態の時は使用できない
        if (GameManager.Instance._player._statusAilment.HaveAilment(Ailment.Sleep))
        {
            return false;
        }
        //選択したタイプの回復をする。
        foreach (var data in _recoevryDatas)
        {
            switch (data._recoveryType)
            {
                case Type.HP:
                    GameManager.Instance._player.DownHp(-data.recoveryNum);
                    break;
                case Type.MP:
                    GameManager.Instance._player.DownMp(-data.recoveryNum);
                    break;
                case Type.LIFE:
                    GameManager.Instance._player.DownLife(data.recoveryNum);
                    break;
                //空腹度に応じてHPとMPの回復をする。
                case Type.SPLIFE:
                    if (GameManager.Instance._player._life >= 79)
                    {
                        GameManager.Instance._player.DownHp(-20);
                        GameManager.Instance._player.DownMp(-20);
                    }
                    GameManager.Instance._player.DownLife(data.recoveryNum);
                    break;
            }
        }
        await GameManager.Instance._player.UseItem(_effect,_name);
        return true;
    }
    [System.Serializable]
    public class RecoevryData
    {
        public Type _recoveryType;
        public int recoveryNum = 20;
    }
}
