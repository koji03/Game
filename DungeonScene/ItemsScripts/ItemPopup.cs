using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopup : MonoBehaviour
{
    public Text _nameText; 
    public Text _effectText;
    Func<UniTask<bool>> _itemFunc;
    ItemModel _selectItem;

    /// <summary>
    /// ポップアップの表示
    /// </summary>
    public void ShowItemPopUp(ItemModel item)
    {
        ItemManager.Instance.ToggleButtons(true);
        gameObject.SetActive(true);
        ItemManager.Instance._equipPopup.gameObject.SetActive(false);

        //テキストのパネルのデータを書く。
        _nameText.text = item._name;
        _effectText.text = item._description;

        //アイテムを使用したときに実行する内容を格納。
        _itemFunc = () => item.UseItem();
        _selectItem = item;
    }

    /// <summary>
    /// アイテムを使用する。
    /// </summary>
    public async void OnSelect()
    {
        ItemManager.Instance.OnClose();
        await UniTask.Delay(400);
        //アイテムの使用をなんらかの理由で中断した場合は何もしない
        bool result = await ItemManager.Instance.ExecuteItem(_itemFunc);
        if (!result) { return; }
        //アイテムを使用したらバッグから削除する。
        ItemManager.Instance.RemoveHaveItem(_selectItem);
        _selectItem = null;
        _itemFunc = null;
    }

    /// <summary>
    /// アイテムをバッグから削除する
    /// </summary>
    public void OnDelete()
    {
        if (_selectItem == null) { return; }
        _nameText.text = "";
        _effectText.text = "";

        //バッグから削除
        ItemManager.Instance.RemoveHaveItem(_selectItem);

        //アイテムリストを作成しなおす。
        ItemManager.Instance.RefreshItemList();
        _selectItem = null;
        _itemFunc = null;
        ItemManager.Instance.Hidebuttons();
    }

}
