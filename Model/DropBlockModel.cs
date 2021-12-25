using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/BlockData/DropBlock")]
public class DropBlockModel : BlockModel
{
    [Tooltip("Itemがついているのだけ")]
    public DropItem[] _dropItem;

    Model DropItem;

    //ブロック破壊後、アイテムを落とす。
    override async public UniTask ActionAfterDestroyed(Vector3 position)
    {
        //ドロップするアイテムを取得。
        DropItem = GameManager.Instance._stagemanager.RandomItemDrop(_dropItem, GameManager.Instance._currentStage);
        //アイテムがドロップするのが確定
        StageManager stage = GameManager.Instance._stagemanager;
        var obj = stage.CreateObject(DropItem, position);
        GameManager.Instance._stagemanager._objectsData.AddObjectData(objectType.Item,GameManager.Instance._stagemanager.xz2i((int)position.x, (int)position.z) , obj, DropItem);
        await UniTask.Delay(1);
    }
}
