using Cysharp.Threading.Tasks;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    [SerializeField] StatPanel _statsPanel;
    [SerializeField] EquipDescriptionPanel _equipDescriptionPanel;

    public void Init()
    {
        UpdateStatusPanel();
    }

    /// <summary>
    /// ステータスの情報を更新する。
    /// </summary>
    void UpdateStatusPanel()
    {
        Player player = GameManager.Instance._player;
        //プレイヤーが覚えているスキルを取得する。
        var skillList = player.GetSkillList();
        var skill1 = SkillManager.Instance.GetSkillData(skillList[0]);
        var skill2 = SkillManager.Instance.GetSkillData(skillList[1]);
        var skill3 = SkillManager.Instance.GetSkillData(skillList[2]);
        var skill4 = SkillManager.Instance.GetSkillData(skillList[3]);

        //プレイヤーが装備しているアイテムを取得
        CharactorEquip equip = GameManager.Instance._player._charactorWeapon;
        var weaponModel = equip.GetWeapon() as WeaponModel;
        var armorModel = equip.GetArmor() as ArmorModel;

        //ステータスのパネルを更新
        _statsPanel.SetupStatuSpanel(
            player.GetMaxHP(), player.GetMaxMP(), player.GetAtk(), player.GetDef(),
            weaponModel, armorModel,
            skill1.GetSkillName(),
            skill1.GetMP(),
            skill2.GetSkillName(),
            skill2.GetMP(),
            skill3.GetSkillName(),
            skill3.GetMP(),
            skill4.GetSkillName(),
            skill4.GetMP(),
            player.GetLv(),
            player.RequiredExp()
            ) ;

        //装備の情報を更新
        if(weaponModel != null)
        {
            _equipDescriptionPanel.UpdateWeaponDescriptionText(weaponModel._name, weaponModel._strength, weaponModel._description);
        }
        else
        {
            _equipDescriptionPanel.UpdateWeaponDescriptionText("", 0, "");
        }
        if (armorModel !=null)
        {
            _equipDescriptionPanel.UpdateArmorDescriptionText(armorModel._name, armorModel._strength);
        }
        else
        {
            _equipDescriptionPanel.UpdateArmorDescriptionText("", 0);
        }
    }
    private void OnEnable()
    {
        //装備の詳細情報を隠す
        _equipDescriptionPanel.Hide();
        UpdateStatusPanel();
    }

}
