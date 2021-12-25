using Extensions;
using TMPro;
using UnityEngine;


public class StatPanel : MonoBehaviour
{
    //ステータス画面
    //ステータス、装備、スキルを表示する。
    [SerializeField]
    TextMeshProUGUI
        _statHP, _statMP, _statAtk, _statDef,
        _weaponName, _weaponStrength, _armorName, _armorStrength,
        _skillName1, _skill1MP, _skillName2, _skill2MP, _skillName3, _skill3MP, _skillname4, _skill4MP,
        _exp,_currentLv,_weight;


    //それぞれのテキストに書いていく
    public void SetupStatuSpanel(int hp,int mp,int atk,int def,
        WeaponModel _wModel, ArmorModel _aModel,
        string skillName1, int skill1MP, string skillName2, int skill2MP, string skillName3, int skill3MP, string skillname4, int skill4MP,
        int lv,int exp)
    {
        _statHP.text = hp.ToString();
        _statMP.text = mp.ToString();
        _statAtk.text = atk.ToString();
        _statDef.text = def.ToString();
        _currentLv.text = lv.ToString();
        _exp.text = exp.ToString();

        //武器のモデルがある場合
        if (_wModel)
        {
            _weaponName.text = _wModel._name;
            _weaponStrength.text = _wModel._strength.ToString();
            _weight.text = _wModel._times.GetWeight();
        }
        else
        {
            _weaponName.text = "";
            _weaponStrength.text = "0";
            _weight.text = "";
        }
        //盾のモデルがある場合
        if (_aModel)
        {
            _armorName.text = _aModel._name;
            _armorStrength.text = _aModel._strength.ToString();
        }
        else
        {
            _armorName.text = "";
            _armorStrength.text = "0";
        }
        _skillName1.text = skillName1;
        _skill1MP.text = skill1MP.ToString();
        _skillName2.text = skillName2;
        _skill2MP.text = skill2MP.ToString();
        _skillName3.text = skillName3;
        _skill3MP.text = skill3MP.ToString();
        _skillname4.text = skillname4;
        _skill4MP.text = skill4MP.ToString();

    }

    public void RefreshEquip(string weaponName, int weaponStrength, string armorName, int armorStrength)
    {
        _weaponName.text = weaponName;
        _weaponStrength.text = weaponStrength.ToString();
        _armorName.text = armorName;
        _armorStrength.text = armorStrength.ToString();
    }
}
