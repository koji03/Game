using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandLvUp
{
    async public UniTask Play()
    {
        //次のレベルに上がるために必要な経験値を取得してプレイヤーに与える
        var exp = GameManager.Instance._player.RequiredExp();
        await GameManager.Instance._player.AddExp(exp + 1);
    }
}
