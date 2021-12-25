using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "ScriptableObjects/BlockData/WallBlock")]
public class WallModel :BlockModel
{
    [Tooltip("Itemがついているのだけ")]
    public ItemModel[] _dropItems;
    [Tooltip("0-100の間、アイテムドロップ確立")]
    public int _probability = 100;
    override async public UniTask ActionAfterDestroyed(Vector3 position){}
}
