using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemList : MonoBehaviour
{
    [SerializeField] Transform _content;
    [SerializeField] HeldItemListButton itemPrefab;

    [NonSerialized] public List<HeldItemListButton> _buttons;
    [SerializeField] Text _itemMaxNumText, _currentItemNumText;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int _maxItemNum)
    {
        _buttons = new List<HeldItemListButton>(_maxItemNum);
    }
    /// <summary>
    /// 所持上限のテキストを更新
    /// </summary>
    /// <param name="maxItemNum"></param>
    public void UpdateMaxItemNumText(int maxItemNum)
    {
        _itemMaxNumText.text = maxItemNum.ToString();
    }
    /// <summary>
    /// リストのボタンを作成
    /// </summary>
    public HeldItemListButton CreateButton()
    {
        _currentItemNumText.text = (_buttons.Count+1).ToString();
        return Instantiate(itemPrefab, _content);
    }

    /// <summary>
    /// ボタンをリストに追加
    /// </summary>
    public void AddListButton(HeldItemListButton listButton)
    {
        _buttons.Add(listButton);
    }
    /// <summary>
    /// 全てのボタンを削除
    /// </summary>
    public void DeleteItemButtons()
    {
        foreach (var button in _buttons)
        {
            RemoveButton(button);
        }
        _buttons.Clear();
    }
    /// <summary>
    /// 指定したボタンを削除
    /// </summary>
    /// <param name="button"></param>
    void RemoveButton(HeldItemListButton button)
    {
        button.gameObject.SetActive(false);
        Destroy(button.gameObject);
    }
}
