using Cysharp.Threading.Tasks;
using UnityEngine;

public class MasterStage
{
    public const string ItemDataPath = "StageItemData/";
    public const string WallDataPath = "StageWallData/";
    public const string BlockPath = "StageBlockData/";
    public const string EnemyPath = "StageEnemyData/";
    public const string BossPath = "StageBossData/";

    /// <summary>
    /// ステージのデータをロード
    /// </summary>
    async public static UniTask<DropItemData> Load(int ID,string path)
    {
        DropItemData model = await Resources.LoadAsync(path + ID) as DropItemData;
        if (model == null)
        {
            //例外処理
            Debug.Log("Modelが見つかりません" + typeof(DropItemData) + ID);
        }
        return model;
    }
}
