using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "StageLevelData", menuName = "StageLevelData")]
public class StageLevelData : ScriptableObject
{
    public List<StageLevel> _stage;

    /// <summary>
    /// ステージのデータを取得
    /// </summary>
    public StageLevel GetData(int level)
    {
        return _stage.FirstOrDefault(x => x._stageLevel == level);
    }
}
[System.Serializable]
public class StageLevel
{
    public byte _stageLevel;
    public byte _clearLevel;
}