using Cysharp.Threading.Tasks;
using UnityEngine;


public enum StageType
{
    Forest = 0,
    Deseart = 1,
    Winter = 2,
    Dungeon = 3,
    Last = 4
}
public class StageParts : MonoBehaviour
{
    [SerializeField] GameObject[] _forestTiles;

    [SerializeField] GameObject[] _desertTiles;


    [SerializeField] GameObject[] _winterTiles;

    [SerializeField] GameObject[] _dungeonTiles;
    [SerializeField] GameObject[] _lastTiles;


    [SerializeField] GameObject[] _forestOutWall;
    [SerializeField] GameObject[] _desertOutWall;
    [SerializeField] GameObject[] _winterOutWall;
    [SerializeField] GameObject[] _dungeonOutWall;
    [SerializeField] GameObject[] _lastOutWall;

    /// <summary>
    /// 床のブロックを取得
    /// </summary>
    public GameObject[] GetTiles(StageType type)
    {
        switch(type)
        {
            case StageType.Forest:
                return _forestTiles;
            case StageType.Deseart:
                return _desertTiles;
            case StageType.Winter:
                return _winterTiles;
            case StageType.Dungeon:
                return _dungeonTiles;
            case StageType.Last:
                return _lastTiles;
            default:
                return null;
        }
    }

    /// <summary>
    /// 外壁を取得
    /// </summary>
    public GameObject[] GetOutWall(StageType type)
    {
        switch (type)
        {
            case StageType.Forest:
                return _forestOutWall;
            case StageType.Deseart:
                return _desertOutWall;
            case StageType.Winter:
                return _winterOutWall;
            case StageType.Dungeon:
                return _dungeonOutWall;
            case StageType.Last:
                return _lastOutWall;
            default:
                return null;
        }
    }
    /// <summary>
    /// 使用するブロックの情報をロード
    /// </summary>
    async public UniTask<DropItemData>LoadBlocks(int stageID)
    {
        DropItemData data = await MasterStage.Load(stageID, MasterStage.BlockPath);
        return data;
    }
    /// <summary>
    /// 使用するエネミーの情報をロード
    /// </summary>
    async public UniTask<DropItemData> LoadEnemies(int stageID)
    {
        DropItemData data = await MasterStage.Load(stageID, MasterStage.EnemyPath);
        return data;
    }
    /// <summary>
    /// 使用する外壁の情報をロード
    /// </summary>
    async public UniTask<DropItemData> LoadWalls(int stageID)
    {
        DropItemData data = await MasterStage.Load(stageID, MasterStage.WallDataPath);
        return data;
    }
    /// <summary>
    /// 使用するアイテムの情報をロード
    /// </summary>
    async public UniTask<DropItemData> LoadItems(int stageID)
    {
        DropItemData data = await MasterStage.Load(stageID, MasterStage.ItemDataPath);
        return data;
    }
}
