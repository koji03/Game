using UnityEngine;

/// <summary>
/// プレイヤーが使えるスキルのリスト
/// </summary>
public class PlayerSkills : MonoBehaviour
{
    [SerializeField] Transform skillList;
    [SerializeField] SkillListButton skillListButtonPrefab;
    [SerializeField] GameObject _closeButton;

    /// <summary>
    /// プレイヤーが使えるスキルのリストを表示
    /// </summary>
    public void ShowPanel(bool isShowCloseBtn)
    {
        gameObject.SetActive(true);
        _closeButton.SetActive(isShowCloseBtn);
    }
    /// <summary>
    /// スキルのリストを隠す
    /// </summary>
    public void HidePanel()
    {
        GetComponent<PanelFade>().FadeOut();
    }

    /// <summary>
    /// リストのアイテムを作成
    /// </summary>
    public void CreateSkillList(AbstractSkill skillData)
    {
        SkillListButton button = Instantiate(skillListButtonPrefab,skillList);
        button.SetSkillData(skillData);
    }
    /// <summary>
    /// スキルを新規獲得するときのリストのアイテムを作成
    /// </summary>
    /// <param name="forgetskillData"></param>
    public void CreateLearnSkillButton(AbstractSkill forgetskillData)
    {
        SkillListButton button = Instantiate(skillListButtonPrefab, skillList);
        button.SetLearnSkillData(forgetskillData);
    }

    /// <summary>
    /// スキルリストのアイテムをすべて削除
    /// </summary>
    public void DeleteSkillList()
    {
        foreach(Transform child in skillList)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }
}
