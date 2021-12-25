using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

//スキルの種類
public enum Skill
{
    Null = 20000,
    PowerAttack = 20001,
    Heel = 20002,
    Long = 20003,
    Gluttony = 20004,
    Teleport = 20005,
    Star = 20006,
    DomeShiekd = 20007,
    Drop = 20008,
    MeteorShower = 20009,
    AllBuff = 20010,
    Safegurad = 20011,
    UTurn = 20012,
    StopTime = 20013,
    EnergyDrain = 20014,
    Curse = 20015,
    Hypnosis = 20016,
    Smash = 20017,
    Sucker = 20018,
    Explosion = 20019,
    Motivation = 20020,
    SpinningSlash = 20021,
    TakeDown = 20022,
    WideAttack = 20023,
    Recovery = 20024,
    BadPoison = 20025,
    BurnAttack = 20026
}
public class SkillManager : Singleton<SkillManager>
{
    [SerializeField] PlayerSkills _playerSkills;
    [SerializeField] SelectedSkillPanel _selectedSkillPanel;
    [SerializeField] PanelFade skillObjects;
    [SerializeField] SkillLearnNew _skillLearnNew;
    SkillData _skillData;
    [NonSerialized] public AbstractSkill _newSkill;
    [SerializeField] SkillModel[] _skills;

    async protected override UniTask Init()
    {
        _skillData = new SkillData();
    }

    /// <summary>
    /// スキルのデータを返す
    /// </summary>
    public SkillModel GetSkillModel(int id)
    {
        return _skills.FirstOrDefault(x => x._ID == id);
    }

    /// <summary>
    /// プレイヤーのスキルリストを表示する。
    /// </summary>
    public void ShowPlayerSkillsPanel(Skill[] skills)
    {
        skillObjects.gameObject.SetActive(true);
        _selectedSkillPanel.HideDescriptionPanel();
        _playerSkills.DeleteSkillList();
        _playerSkills.ShowPanel(true);

        //プレイヤーのスキルリストを作成
        for (int i=0; i< skills.Length; i++)
        {
            if(skills[i] == Skill.Null)
            {
                continue;
            }
            var skillData = GetSkillData(skills[i]);
            _playerSkills.CreateSkillList(skillData);
        }
    }


    Subject<Unit> _skillSelectWait;
    bool _isLearned;
    /// <summary>
    /// 新しく覚えるスキルときに使う。
    /// スキルを覚えた場合はtrue、覚えなかった場合はfalse
    /// </summary>
    public async UniTask<bool> LearnNewSkill(AbstractSkill skillData,Skill[] playerSkills)
    {
        skillObjects.gameObject.SetActive(true);
        _playerSkills.HidePanel();
        _selectedSkillPanel.HideDescriptionPanel();
        _skillLearnNew.ShowDescriptionPanel(skillData, playerSkills);
        _skillSelectWait = new Subject<Unit>();

        //スキルを覚えるか、覚えないかの選択が完了するまで待機する。
        await _skillSelectWait.ToUniTask(useFirstValue:true);
        return _isLearned;
    }

    public void OnCompeted(bool isLearn)
    {
        _isLearned = isLearn;
        if(_skillSelectWait != null)
        {
            _skillSelectWait.OnNext(Unit.Default);
            _skillSelectWait.OnCompleted();
        }

    }

    /// <summary>
    /// 新しくスキルを覚えるときに表示する。
    /// プレイヤーのスキルリストの表示
    /// </summary>
    public void ShowLearnSkills(Skill[] playerSkills)
    {
        _playerSkills.DeleteSkillList();
        for (int i = 0; i < playerSkills.Length; i++)
        {
            var _forgetskillData = GetSkillData(playerSkills[i]);
            _playerSkills.CreateLearnSkillButton(_forgetskillData);
            if(_forgetskillData.GetID() == 20000)
            {
                GetNewSkill(_newSkill, _forgetskillData).Forget();
                return;
            }
        }
        skillObjects.gameObject.SetActive(true);
        _playerSkills.ShowPanel(false);
    }

    /// <summary>
    /// スキル画面を隠す
    /// </summary>
    public void HideSkillObjectsPanel()
    {
        OnCompeted(false);
        skillObjects.FadeOut();
    }
    /// <summary>
    /// スキルの詳細を表示する
    /// </summary>
    public void ShowDescriptionPanel(AbstractSkill skillData,bool _isUse)
    {
        _selectedSkillPanel.ShowDescriptionPanel(skillData, _isUse);
    }

    public AbstractSkill GetSkillData(Skill skill)
    {
        return _skillData.GetSkillData(skill);
    }
    public Skill GetSkillData(AbstractSkill skill)
    {
        return _skillData.GetSkillType(skill);
    }

    /// <summary>
    /// スキルを新規で獲得する
    /// </summary>
    async public UniTask GetNewSkill(AbstractSkill skill, AbstractSkill _forgettingSkill)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance._player.SetSkill(skill, SkillManager.Instance.GetSkillData(_forgettingSkill));
        }
        else
        {
            var psData = await SavePlayerSkill.Deserialize();

            int learnID = skill.GetID();
            int forgetID = (_forgettingSkill != null) ? _forgettingSkill.GetID() : -1;
            int index = -1;
            if (psData != null)
            {
                for (int i = 0; i < psData.IDs.Count; i++)
                {
                    if (psData.IDs[i] == forgetID)
                    {
                        index = i;
                        break;
                    }
                }
                if (index < 0)
                {
                    for (int i = 0; i < psData.IDs.Count; i++)
                    {
                        if (psData.IDs[i] <= Setting.SkillID[0])
                        {
                            psData.IDs[i] = learnID;
                            break;
                        }
                    }
                }
                else
                {
                    psData.IDs[index] = learnID;
                }
                SavePlayerSkill.SavePlayerSkills(psData.IDs).Forget();
            }
            else
            {
                SavePlayerSkill.SavePlayerSkills(new List<int> { learnID, 20000, 20000, 20000 }).Forget();
            }
        }
        OnCompeted(true);
        MessageManager.Instance.ShowMessage(_newSkill.GetSkillName(), "のスキルを覚えた。");
        HideSkillObjectsPanel();
    }
}
