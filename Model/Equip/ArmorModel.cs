using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/ItemData/Armor")]
public class ArmorModel : EquipModel
{
    public override UniTask<bool> UseItem()
    {
        throw new System.NotImplementedException();
    }

}