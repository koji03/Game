using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Extensions;
public class EquipPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _selectName, _selectDescription, _selectStrength,_selectWeight;
    [SerializeField] TextMeshProUGUI _currentName, _currentDescription, _currentStrength,_currentWeight;
    ItemModel _selectItem;


    public void ShowEquipPanel(EquipModel item)
    {
        //装備用のボタンに切り替える。
        ItemManager.Instance.ToggleButtons(false);
        //アイテム
        ItemManager.Instance._itemPopup.gameObject.SetActive(false);
        gameObject.SetActive(true);

        _selectItem = item;

        //選択した装備と、装備している装備の情報をそれぞれのテキストに書く。
        _selectName.text = item._name;
        _selectDescription.text = item._description;
        _selectStrength.text = item._strength.ToString();
        EquipModel current;
        if (item is WeaponModel)
        {
            current = GameManager.Instance._player._charactorWeapon.GetWeapon() as EquipModel;
            var w = item as WeaponModel;
            var c = current as WeaponModel;
            _selectWeight.text = w._times.GetWeight();
            _currentWeight.text = c._times.GetWeight();
        }
        else
        {
            current = GameManager.Instance._player._charactorWeapon.GetArmor() as EquipModel;
            _selectWeight.text = "-";
            _currentWeight.text = "-";
        }
        _currentName.text = current._name;
        _currentDescription.text = current._description;
        _currentStrength.text = current._strength.ToString();
    }

    /// <summary>
    /// 装備を変更する
    /// </summary>
    public void OnChangeEquip()
    {
        Player _player = GameManager.Instance._player;

        //バッグからアイテムを削除する。
        ItemManager.Instance.RemoveHaveItem(_selectItem);
        if (_selectItem.itemType == ItemType.Weapon)
        {
            //現在装備している武器をバッグに移動する。
            ItemModel weapon = _player._charactorWeapon.GetWeapon();
            if (weapon != null)
            {
                ItemManager.Instance.AddItem(weapon);
            }
            UpdateData(true).Forget();
        }
        else if (_selectItem.itemType == ItemType.Armor)
        {
            //現在装備している盾をバッグに移動する。
            var armor = _player._charactorWeapon.GetArmor();
            if (armor != null)
            {
                ItemManager.Instance.AddItem(armor);
            }
            UpdateData(false).Forget();
        }
        _selectItem = null;

        //装備パネルを非表示にする。
        ItemManager.Instance.OnClose();
    }
    /// <summary>
    /// ステータス画面などを更新する。
    /// </summary>
    async UniTask UpdateData(bool isweapon)
    {
        //装備を変更したらターンを終了する。
        await GameManager.Instance.PlayerAction(() => ChangeEquip(isweapon), GameManager.Instance._player.GetActableTimes());
        ItemManager.Instance.UpdateStatusCtrl();
        GameManager.Instance._player.SetAttackMark();
    }
    async public UniTask<bool> ChangeEquip(bool isweapon)
    {
        if (isweapon)
        {
            GameManager.Instance._player.SetWeaponObject(_selectItem);
        }
        else
        {
            GameManager.Instance._player.SetArmorObject(_selectItem);
        }
        return true;
    }

    /// <summary>
    /// 選択中の装備を削除する
    /// </summary>
    public void OnDelete()
    {
        if (_selectItem == null) { return; }
        //パネルのテキストを消す。
        _selectName.text = "";
        _selectDescription.text = "";
        _selectStrength.text = "";

        ItemManager.Instance.RemoveHaveItem(_selectItem);

        //所持アイテムのリストを更新する。
        ItemManager.Instance.RefreshItemList();
        _selectItem = null;
        ItemManager.Instance.Hidebuttons();
    }
}
