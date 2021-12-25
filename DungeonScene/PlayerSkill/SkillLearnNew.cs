using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillLearnNew : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name, _description, _strengthLabel, _strength,_MPText;
    [SerializeField] Button _button;

    /// <summary>
    /// 新しく覚えるスキルの詳細パネルを出す。
    /// </summary>
    public void ShowDescriptionPanel(AbstractSkill skillData, Skill[] _playerSkills)
    {
        //パネルをアクティブにする。
        gameObject.SetActive(true);

        //スキルの情報をテキストに書いていく
        _name.text = skillData.GetSkillName();
        int strength = skillData.GetStrength();
        _strength.text = skillData.GetStrength().NumToString();
        _description.text = skillData.GetSkillDescription();
        _MPText.text = skillData.GetMP().ToString();

        SkillManager.Instance._newSkill = skillData;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => ButtonListener(_playerSkills));
    }

    /// <summary>
    /// 決定のボタンを押したときのメソッド
    /// </summary>
    /// <param name="_playerSkills"></param>
    void ButtonListener(Skill[] _playerSkills)
    {
        SkillManager.Instance.ShowLearnSkills(_playerSkills);
    }
}
