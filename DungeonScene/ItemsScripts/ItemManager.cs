using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemType
{
    Recovery, Weapon, Throwing,Buff,Learn,Ailment,Armor,HandScroll,Heart
}
public class ItemManager : Singleton<ItemManager>
{
    //所持アイテムのリスト
    public List<ItemModel> _items = new List<ItemModel>(Setting.NumCanHaveHeldItems);
    public ItemPopup _itemPopup;
    public EquipPopup _equipPopup;
    [SerializeField] StatusController _statusCtrl;

    public PossessionItemScreen _itemScreen;

    public GameObject[] panels;
    [SerializeField] Menu _menu;


    [SerializeField] GameObject itemButtons, equipButtons;

    async protected override UniTask Init()
    {
        _itemScreen.Init(Setting.NumCanHaveHeldItems);
    }

    /// <summary>
    /// アイテム用のボタンと装備用のボタンを切り替える。
    /// </summary>
    /// <param name="isItembuttons"></param>
    public void ToggleButtons(bool isItembuttons)
    {
        itemButtons.SetActive(isItembuttons);
        equipButtons.SetActive(!isItembuttons);
    }
    /// <summary>
    /// ボタンをすべて非表示にする
    /// </summary>
    public void Hidebuttons()
    {
        itemButtons.SetActive(false);
        equipButtons.SetActive(false);
    }

    /// <summary>
    /// 自動で発動するアイテムがあるかを確認。
    /// ある場合は使用する。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool CheckHeartItem<T>()
    {
        var model = _items.FirstOrDefault(x => x is T);
        if (model != null)
        {
            var heart = model as IHeart;
            heart.UseAuto();
            return true;
        }
        return false;
    }
    /// <summary>
    /// ステータスの情報を更新
    /// </summary>
    public void UpdateStatusCtrl()
    {
        _statusCtrl.Init();
    }

    /// <summary>
    /// アイテムの所持上限のテキストを更新
    /// </summary>
    /// <param name="maxnum"></param>
    public void UpdateMaxItemNumText(int maxnum)
    {
        _itemScreen.UpdateMaxItemNumText(maxnum);
    }
    /// <summary>
    /// アイテムを取得できるかどうか（アイテムの所持上限ではないか）
    /// </summary>
    /// <returns></returns>
    public bool CanGetItem()
    {
        if (_items.Count >= Setting.NumCanHaveHeldItems) { return false; } else { return true; }
    }

    /// <summary>
    /// 所持アイテムを削除する。
    /// </summary>
    /// <param name="_item"></param>
    public void RemoveHaveItem(ItemModel _item)
    {
        _items.Remove(_item);
    }
    /// <summary>
    /// 所持アイテムに追加する
    /// </summary>
    /// <param name="_item"></param>
    public void AddItem(ItemModel _item)
    {
        _items.Add(_item);
    }

    /// <summary>
    /// 子要素もすべて隠す。
    /// </summary>
    public void OnClose()
    {
        foreach(Transform child in transform)
        {
            if(child.gameObject.activeInHierarchy)
            {
                child.GetComponent<PanelFade>().FadeOut();
            }
        }
    }

    /// <summary>
    /// アイテムを実行する。
    /// </summary>
    public async UniTask<bool> ExecuteItem(Func<UniTask<bool>> _itemFunc)
    {
        var result = await GameManager.Instance.PlayerAction(() => _itemFunc(),GameManager.Instance._player.GetActableTimes());
        return result;
    }

    /// <summary>
    /// 選択した番号のパネルを表示する。
    /// </summary>
    /// <param name="num"></param>
    public void ShowPanel(int num)
    {
        if (GameManager.Instance._playerSelect || GameManager.Instance._enemyTurn) { return; }
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.GetComponent<PanelFade>().FadeOut();
            }
        }
        panels[num].SetActive(true);
    }
    public void BackButton()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.GetComponent<PanelFade>().FadeOut();
            }
        }
        _menu.OpenMenu();
    }
    public void RefreshItemList()
    {
        _itemScreen.RefreshItems();
    }
}
