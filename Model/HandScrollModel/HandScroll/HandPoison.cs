using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandPoison
{
    public void Play()
    {
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.Foword, GameManager.Instance._player.transform);
        foreach (var atkPos in attackPosition)
        {
            AddAilment(atkPos);
        }
    }
     void AddAilment(Vector3Int _attackPosition)
    {
        //周囲16ます。
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            var b = block.Find(x => x._type == objectType.Enemy);
            if (b != null)
            {
                var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
                b._object.GetComponent<Enemy>().AddAilment(Ailment.Poison).Forget();
            }
        }
    }
}
