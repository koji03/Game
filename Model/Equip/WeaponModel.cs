using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/ItemData/Weapon")]
public class WeaponModel : EquipModel
{
    public int _hitrate = 100;
    public AttackDirection _attackDirection = AttackDirection.ThreeHorizon;
    public bool isPenetrating = true;
    public ConsecutiveTimes _times;
    public override UniTask<bool> UseItem()
    {
        throw new System.NotImplementedException();
    }

}
