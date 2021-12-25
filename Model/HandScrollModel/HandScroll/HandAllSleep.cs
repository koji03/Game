using Cysharp.Threading.Tasks;

public class HandAllSleep
{
    async public UniTask Play()
    {
        //全ての敵を取得して眠らせる
        var enemies = GameManager.Instance.GetEnemies();
        foreach(var e in enemies)
        {
            await e.AddAilment(Ailment.Sleep);
        }
    }

}
