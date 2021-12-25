using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 所持アイテムリストの画面
/// </summary>
public class PossessionItemScreen : MonoBehaviour
{

    [SerializeField] ItemList _itemList;
    [SerializeField] Text _heading;

    private void OnEnable()
    {
        ItemManager.Instance.Hidebuttons();
        ItemManager.Instance._itemPopup.gameObject.SetActive(false);
        ItemManager.Instance._equipPopup.gameObject.SetActive(false);

        //アイテムリストを作成しなおす。
        RefreshItems();
    }
    public void Init(int _maxItemNum)
    {
        _itemList.Init(_maxItemNum);
    }

    /// <summary>
    /// 所持上限のテキストを更新
    /// </summary>
    /// <param name="maxnum"></param>
    public void UpdateMaxItemNumText(int maxnum)
    {
        _itemList.UpdateMaxItemNumText(maxnum);
    }
    /// <summary>
    /// アイテムリストのボタンを作成する
    /// </summary>
    HeldItemListButton CreateListButton(ItemModel item)
    {
        var itemprefab = _itemList.CreateButton();
        itemprefab.SetText(item);
        itemprefab.AddButtonListener(item);
        _itemList._buttons.Add(itemprefab);
        return itemprefab;
    }
    /// <summary>
    /// ボタンを全て作成しなおす。
    /// </summary>
    public void RefreshItems()
    {
        _itemList.DeleteItemButtons();
        OnOpenItemList();
    }
    /// <summary>
    /// 装備のリストを表示する。
    /// </summary>
    /// <param name="type"></param>
    public void OnOpenWeaponList(ItemType type)
    {
        var items = ItemManager.Instance._items;
        foreach (var item in items)
        {
            if (item.itemType == type)
            {
                CreateListButton(item);
            }
        }
        gameObject.SetActive(true);
        _heading.text = "武器";

    }
    /// <summary>
    /// アイテムのリストを表示する。
    /// </summary>
    void OnOpenItemList()
    {
        var items = ItemManager.Instance._items;
        foreach (var item in items)
        {
            CreateListButton(item);
        }
        gameObject.SetActive(true);
        _heading.text = "アイテム";
    }

}
