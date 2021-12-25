using UnityEngine;
using UnityEngine.UI;

public class HeldItemListButton : MonoBehaviour
{
    public Text _name;
    [SerializeField] Button _button;
    /// <summary>
    /// ボタンに表示するテキストを変更
    /// </summary>
    public void SetText(ItemModel item)
    {
        _name.text = item._name;
    }
    /// <summary>
    /// ボタンをクリックしたときの挙動を変更
    /// </summary>
    /// <param name="item"></param>
    public void AddButtonListener(ItemModel item)
    {
        _button.onClick.RemoveAllListeners();
        //装備かアイテムで表示するものを変えている。
        if(item is EquipModel)
        {
            //装備の詳細を表示
            _button.onClick.AddListener(() => ItemManager.Instance._equipPopup.ShowEquipPanel(item as EquipModel));
        }
        else
        {
            //アイテムの詳細を表示
            _button.onClick.AddListener(() => ItemManager.Instance._itemPopup.ShowItemPopUp(item));
        }
    }

}
