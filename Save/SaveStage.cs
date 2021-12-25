using Cysharp.Threading.Tasks;

public class SaveStage
{
    public class StageData
    {
        public bool _isClear;
        public int _currentStage;
        public int _stageLevel;
        public ushort _turnNum;
        public int _row;
        public int _column;
        public int _exitPosition;
        public int _canGetRewordNum;
        public bool _forceBattle;
    }
    async public static UniTask SaveStageData(bool isclear, int currentStage,ushort turnNum,int row,int column, int _exitPosition,int _canGetRewordNum,bool _forceBattle)
    {
        StageData stage = new StageData();
        stage._isClear = isclear; //クリアしたか
        stage._currentStage = currentStage;
        stage._stageLevel = Level._Level; 
        stage._turnNum = turnNum;
        stage._row = row;
        stage._column = column;
        stage._exitPosition = _exitPosition;
        stage._canGetRewordNum = _canGetRewordNum;
        stage._forceBattle = _forceBattle;
        await Save.Seialize(Save.stagePath, stage);
    }
    async public static UniTask<StageData> Deserialize()
    {
        return await Save.Deserialize<StageData>(Save.stagePath);
    }
}
