using Cysharp.Threading.Tasks;
public class SavePlayerParams
{
    public class PlayerParamData
    {
        public int _playerHP;
        public int _playerMP;
        public int _playerLife;
        public int[] _playerAilment;
        public int _playerTotalExp;
    }

    async public static UniTask SaveStageData(int exp,int hp,int mp ,int life,int[] ailment)
    {
        PlayerParamData pdata = new PlayerParamData();
        pdata._playerTotalExp = exp;
        pdata._playerHP = hp;
        pdata._playerMP = mp;
        pdata._playerLife = life;
        pdata._playerAilment = ailment;
        await Save.Seialize(Save.playerStagePath, pdata);
    }

    async public static UniTask<PlayerParamData> Deserialize()
    {
        return await Save.Deserialize<PlayerParamData>(Save.playerStagePath);
    }
}
