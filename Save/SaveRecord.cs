using Cysharp.Threading.Tasks;

public class SaveRecordData
{
    public class StageRecord
    {
        public int currentLevel;
        public int maxLevel;
    }
    static StageRecord record = new StageRecord();
    async public static UniTask SaveRecord(int currentLevel)
    {
        //現在の階層を記録
        record.currentLevel = currentLevel;
        //過去のの階層を記録
        record.maxLevel = (record.maxLevel <= currentLevel)? currentLevel: record.maxLevel;
        await Save.Seialize(Save.progressDataPath, record);
    }

    async public static UniTask<StageRecord> Deserialize()
    {
        var data = await Save.Deserialize<StageRecord>(Save.progressDataPath);
        //データが存在しない場合は全て0
        if(data ==null)
        {
            record.currentLevel = 0;
            record.maxLevel = 0;
        }
        else
        {
            record.currentLevel = data.currentLevel;
            record.maxLevel = data.maxLevel;
        }
        return record;
    }
}
