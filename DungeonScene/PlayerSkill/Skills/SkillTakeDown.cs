using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

public class SkillTakeDown : AbstractSkill
{
    async UniTask SkillAttack()
    {
        float moveSpeed = 6f;
        //自分の目の前の8マスを取得
        //遠い順からブロックを調べ何もなかったらそこに移動する
        Player p = GameManager.Instance._player;
        var positions = AttackPositions.FourFoword(p.transform);

        Vector3 position = p.transform.position;
        for (int i = 0;  i< positions.Length; i++)
        {
            var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(positions[i].x), Mathf.RoundToInt(positions[i].z));
            bool _onItem = (block != null) ? !block.Any(x => x._type != objectType.Item) : GameManager.Instance._stagemanager.IsIn(positions[i]);
            if (_onItem)
            {
                position = positions[i];
            }
            else
            {
                break;
            }
        }
        float distance = Vector3.Distance(p.transform.position, position);
        float moveTime = distance / moveSpeed;
        await p.transform.DOMove(position, moveTime).SetEase(Ease.InBack);
        p.transform.position = position;
        GameManager.Instance._stagemanager._playerNextPosition = position;
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.Foword, GameManager.Instance._player.transform);

        float damageMag = 1;
        foreach (var atkPos in attackPosition)
        {
            await Attacked(atkPos, damageMag);
            damageMag += 0.1f;
        }
        async UniTask Attacked(Vector3Int _attackPosition, float mag)
        {
            //周囲16ます。
            var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
            if (block != null)
            {
                var b = block.Find(x => x._type != objectType.Item);
                if (b != null)
                {
                    var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
                    await b._object.GetComponent<Damage>().AttackDamage(GameManager.Instance._player.GetAtk(), Mathf.CeilToInt((_model._strength + weaponS) * mag), b._model);
                }
            }
        }
    }


    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

}
