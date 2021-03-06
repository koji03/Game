using Cysharp.Threading.Tasks;

public class SaveSetting
{
    public class SettingData
    {
        public float BGMVolume;
        public float SEVolume;
    }

    async public static UniTask Serialize(float bgm, float se)
    {
        //BGMとSEの音量設定を保存
        var setting = new SettingData();
        setting.BGMVolume = bgm;
        setting.SEVolume = se;
        await Save.Seialize(Save.setting, setting);
    }

    async public static UniTask<SettingData> Deserialize()
    {
        return await Save.Deserialize<SettingData>(Save.setting);
    }
}
