using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandCurse 
{
    async public UniTask Play()
    {
        //前方のマスを取得
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.Foword, GameManager.Instance._player.transform);
        foreach (var atkPos in attackPosition)
        {
            //状態異常を付与
            await AddAilment(atkPos);
        }
    }
    async UniTask AddAilment(Vector3Int _attackPosition)
    {
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            //目の前のマスにエネミーがいれば状態異常を付与
            var b = block.Find(x => x._type == objectType.Enemy);
            if (b != null)
            {
                await b._object.GetComponent<Enemy>().AddAilment(Ailment.Curse);
            }
        }
    }
}
