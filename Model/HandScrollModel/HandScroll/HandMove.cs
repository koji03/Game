using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

public class HandMove
{
    public void Play()
    {
        //プレイヤーをマス先に移動させる。
        MovePlayer();
    }
    void MovePlayer()
    {
        //自分の目の前の8マスを取得
        //遠い順からブロックを調べ何もなかったらそこに移動する
        Player p = GameManager.Instance._player;
        var positions = AttackPositions.EightFoword(p.transform);

        Vector3 position = p.transform.position;
        for (int i = positions.Length - 1; i > 0; i--)
        {
            var block = GameManager.Instance._stagemanager.CheckBlock(GameManager.Instance._stagemanager._objectsData, Mathf.RoundToInt(positions[i].x), Mathf.RoundToInt(positions[i].z));
            if (!GameManager.Instance._stagemanager.IsIn(positions[i]))
            {
                continue;
            }
            //アイテム以外が含まれていたらtrue
            if (block != null && block.Any(x => x._type != objectType.Item))
            {
                continue;
            }
            position = positions[i];
            break;
        }
        p.transform.position = position;
        GameManager.Instance._stagemanager._playerNextPosition = position;
    }
}
