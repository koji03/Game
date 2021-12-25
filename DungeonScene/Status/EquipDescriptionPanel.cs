using TMPro;
using UnityEngine;

public class EquipDescriptionPanel : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _name, _strength, _description,_weight;

    string armorName, armorStrength, weaponName, weaponStrength;
    [SerializeField]public bool _isWeapon;
    /// <summary>
    /// 装備の情報を更新
    /// </summary>
    public void UpdateWeaponDescriptionText(string name, int strength,string description)
    {
        weaponName = name;
        weaponStrength = strength.ToString();
        _description.text = description;
    }
    /// <summary>
    /// 盾の情報を更新
    /// </summary>
    public void UpdateArmorDescriptionText(string name, int strength)
    {
        armorName = name;
        armorStrength = strength.ToString();
    }
    /// <summary>
    /// 装備の詳細を表示
    /// </summary>
    /// <param name="isweapon"></param>
    public void ShowDescriptionPanel(bool isweapon)
    {
        _isWeapon = isweapon;
        _name.text = (isweapon) ? weaponName : armorName;
        _strength.text = (isweapon) ? weaponStrength : armorStrength;

        _name.gameObject.SetActive(isweapon);
        _strength.gameObject.SetActive(isweapon);
        _description.gameObject.SetActive(isweapon);
        gameObject.SetActive(!gameObject.activeSelf);
    }
    /// <summary>
    /// 詳細情報を隠す
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
