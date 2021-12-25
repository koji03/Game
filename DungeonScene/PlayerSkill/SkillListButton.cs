using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillListButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _skillName,_MPText;
    [SerializeField] Button button;

    /// <summary>
    /// ボタンにスキルのデータを入れる。
    /// </summary>
    public void SetSkillData(AbstractSkill skillData)
    {
        if(skillData == null) { _skillName.text = "-";return; }

        var MP = (GameManager.Instance._player._spAilment.HaveAilment(SpAilment.MPHalf)) ? skillData.GetMP() / 2 : skillData.GetMP();

        _skillName.text = skillData.GetSkillName();
        _MPText.text = MP.NumToString();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(()=> OnSkillButton(skillData));
    }

    /// <summary>
    /// ボタンに新規で覚えるスキルのデータを入れる。
    /// </summary>
    public void SetLearnSkillData(AbstractSkill forgetskillData)
    {
        if (forgetskillData == null) 
        { 
            _skillName.text = "-"; 
            _MPText.text = "-"; 
        } 
        else 
        { 
            _skillName.text = forgetskillData.GetSkillName(); 
            _MPText.text = forgetskillData.GetMP().NumToString(); 
        }
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnLearnButton(forgetskillData));
    }
    /// <summary>
    /// スキルの詳細を表示する
    /// </summary>
    public void OnSkillButton(AbstractSkill skillData)
    {
        SkillManager.Instance.ShowDescriptionPanel(skillData,true);
    }
    /// <summary>
    /// 詳細を表示する。（スキルを新規獲得するときに使う
    /// </summary>
    public void OnLearnButton(AbstractSkill forgetskillData)
    {
       SkillManager.Instance.ShowDescriptionPanel(forgetskillData, false);
    }
}
