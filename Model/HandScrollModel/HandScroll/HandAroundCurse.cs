using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandAroundCurse
{
    async public UniTask Play()
    {
        //���͂Q�}�X���擾
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.TwoSquaresAround, GameManager.Instance._player.transform);
        //�̂낢��Ԃ�t�^
        foreach (var atkPos in attackPosition)
        {
            await AddAilment(atkPos);
        }
    }
    async UniTask AddAilment(Vector3Int _attackPosition)
    {
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            var b = block.Find(x => x._type == objectType.Enemy);
            if (b != null)
            {
                var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
                await b._object.GetComponent<Enemy>().AddAilment(Ailment.Curse);
            }
        }
    }
}
