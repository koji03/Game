using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SkillUTurn : AbstractSkill
{
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }
    async UniTask SkillAttack()
    {
        bool canmove = GameManager.Instance._player._canMove;
        GameManager.Instance._player._canMove = true;
        var attackPosition = AttackPositions.GetAttackPosition(AttackDirection.Foword, GameManager.Instance._player.transform);
        foreach (var atkPos in attackPosition)
        {
            await Attacked(atkPos);
        }
        var pD = GameManager.Instance._player._playerDirection;
        await MoveDirection(pD);
        GameManager.Instance._player._canMove = canmove;
    }
    async UniTask Attacked(Vector3Int _attackPosition)
    {
        var weaponS = GameManager.Instance._player._charactorWeapon.GetWeaponStrength();
        //周囲16ます。
        var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(_attackPosition.x), Mathf.RoundToInt(_attackPosition.z));
        if (block != null)
        {
            var b = block.Find(x => x._type != objectType.Item);
            if (b != null)
            {
                await b._object.GetComponent<Damage>().AttackDamage(GameManager.Instance._player.GetAtk(), _model._strength + weaponS, b._model);
            }
        }
    }

    async UniTask MoveDirection(Direction pD)
    {
        switch (pD)
        {
            case Direction.Up:
                await GameManager.Instance._player.Move(Direction.Down);
                break;
            case Direction.UpperLeft:
                await GameManager.Instance._player.Move(Direction.LowerRight);
                break;
            case Direction.UpperRigjt:
                await GameManager.Instance._player.Move(Direction.LowerLeft);
                break;
            case Direction.Right:
                await GameManager.Instance._player.Move(Direction.Left);
                break;
            case Direction.Left:
                await GameManager.Instance._player.Move(Direction.Right);
                break;
            case Direction.LowerLeft:
                await GameManager.Instance._player.Move(Direction.UpperRigjt);
                break;
            case Direction.LowerRight:
                await GameManager.Instance._player.Move(Direction.LowerLeft);
                break;
            case Direction.Down:
                await GameManager.Instance._player.Move(Direction.Up);
                break;
            default:
                await GameManager.Instance._player.Move(Direction.Down);
                break;
        }
    }
}
