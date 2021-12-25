using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Ailment", menuName = "ScriptableObjects/ItemData/Ailment")]

public class AilmentModel : ItemModel
{
    public Ailment _ailment;
    public bool isAddAilment = true;
    /// <summary>
    /// アイテムを使用する。
    /// </summary>
    public override async UniTask<bool> UseItem()
    {
        ///状態異常を追加するのか、回復するのか
        if(isAddAilment)
        {
            GameManager.Instance._player.AddAilment(_ailment);
        }
        else
        {
            GameManager.Instance._player.RecoveryAilment(_ailment);
        }
        await GameManager.Instance._player.UseItem(_effect,_name);
        return true;
    }
}
