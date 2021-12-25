using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "StageData")]
public class DropItemData : ScriptableObject
{
    //一定の階層ごとに出現アイテムを設定する
    public byte _stageID;
    [ListDrawerSettings(HideAddButton =true,HideRemoveButton =true)]
    public Data[] _Data = new Data[5];

    //現在の階層に合わせてアイテムを変える。
    public DropItem[] GetDropItems(int level)
    {
        return _Data[level]._dropItem;
    }

    //現在の階層に合わせてアイテムを変える。
    public Model[] GetModels(int level)
    {
        return _Data[level]._dropItem.Select(x => x._model).ToArray();
    }

    [System.Serializable]
    public class Data
    {
        public DropItem[] _dropItem;

    }
}

[System.Serializable]
public class DropItem
{
    public Model _model;
    [Range(1, 5)]
    public byte _dropRate;
    [Range(1, 100)]
    [ListDrawerSettings(HideRemoveButton = true,HideAddButton = true, DraggableItems =false)]
    public int[] _dropLevel = new int[2] {0,100 };
}