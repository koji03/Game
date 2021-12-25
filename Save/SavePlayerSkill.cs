using Cysharp.Threading.Tasks;
using System.Collections.Generic;


public class SavePlayerSkill
{
    public class SkillData
    {
        public List<int> IDs;
    }

    async public static UniTask SavePlayerSkills(List<int> ids)
    {
        //idが0以下は削除
        ids.RemoveAll(x => x <= 0);
        var SkillData = new SkillData();
        SkillData.IDs = ids;
        await Save.Seialize(Save.playerSkillDataPath, SkillData);
    }

    async public static UniTask<SkillData> Deserialize()
    {
        return await Save.Deserialize<SkillData>(Save.playerSkillDataPath);
    }
}
