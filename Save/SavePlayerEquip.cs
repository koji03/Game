using Cysharp.Threading.Tasks;

public class SavePlayerEquip
{
    public class EquipData
    {
        public int weaponID;
        public int armorID;
    }

    async public static UniTask SavePlayerEquips(int weaponID, int armorID)
    {
        var SkillData = new EquipData();
        SkillData.weaponID = weaponID;
        SkillData.armorID = armorID;
       await  Save.Seialize(Save.playerEquipDataPath, SkillData);
    }
    async public static UniTask SavePlayerEquips(EquipData data)
    {
        await Save.Seialize(Save.playerEquipDataPath, data);
    }

    async public static UniTask<EquipData> Deserialize()
    {
        return await Save.Deserialize<EquipData>(Save.playerEquipDataPath);
    }
}