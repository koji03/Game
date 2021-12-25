using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Extensions;
public class SelectedSkillPanel : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _name, _description,_strengthLabel,_strength,_buttonText;
    [SerializeField] Button _useButton,_yesButton,_noButton; 

    /// <summary>
    /// スキルの詳細を表示
    /// </summary>
    public void ShowDescriptionPanel(AbstractSkill skillData,bool _isUse)
    {
        gameObject.SetActive(true);
        if (skillData == null)
        {
            _name.text = "-";
            _strength.gameObject.SetActive(false);
            _strengthLabel.gameObject.SetActive(false);
            _strength.text = "-";
            _description.text = "-";
            _buttonText.text = "覚える";
        }
        else
        {
            _name.text = skillData.GetSkillName();
            _strength.text = skillData.GetStrength().NumToString();
            _description.text = skillData.GetSkillDescription();
            _buttonText.text = "忘れさせる";
        }
        //スキルを使う場合と習得する場合で分ける。
        if (_isUse)
        {
            var data = skillData.GetSkillModel();
            UseButtonAddLListener(skillData.GetSkill(), skillData.GetSkillName(), skillData.GetMP(), data._effect, data._direction);
        }else
        {
            AddLListener(SkillManager.Instance._newSkill, skillData);
        }
        _useButton.gameObject.SetActive(_isUse);
        _yesButton.gameObject.SetActive(!_isUse);
        _noButton.gameObject.SetActive(!_isUse);
    }
    /// <summary>
    /// スキルを使用するときのボタン
    /// </summary>
    void UseButtonAddLListener(Func<UniTask> skill,string name,int MP,EffectData effect,AttackDirection direction)
    {
        _useButton.onClick.RemoveAllListeners();
        _useButton.onClick.AddListener(
            () => GameManager.Instance.PlayerAction(()=> GameManager.Instance._player.SkillAttack(name, skill, effect, direction,MP), GameManager.Instance._player.GetActableTimes()).Forget());
    }
    /// <summary>
    /// スキルを覚えるときのボタン
    /// </summary>
    public void AddLListener(AbstractSkill skill, AbstractSkill _forgettingSkill)
    {
        _yesButton.onClick.RemoveAllListeners();
        _yesButton.onClick.AddListener(() => SkillManager.Instance.GetNewSkill(skill, _forgettingSkill).Forget());
    }
    /// <summary>
    /// 詳細情報を隠す。
    /// </summary>
    public void HideDescriptionPanel()
    {
        gameObject.SetActive(false);
    }
}
