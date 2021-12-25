using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSkillData : MonoBehaviour
{
    [SerializeField] Skill[] _skillList = new Skill[4];

    public Skill[] GetSkillList()
    {
        return _skillList;
    }
    /// <summary>
    /// プレイヤーのスキルリストをセット
    /// </summary>
    public void SetSkills(List<Skill> skillList)
    {
        //空の場合20000番のスキルをセット
        if (skillList ==null)
        {
            Skill[] skill = new Skill[] {
                (Skill)Enum.ToObject(typeof(Skill), 20000) ,
                (Skill)Enum.ToObject(typeof(Skill), 20000) ,
                (Skill)Enum.ToObject(typeof(Skill), 20000) ,
                (Skill)Enum.ToObject(typeof(Skill), 20000) ,
            };
            _skillList =skill;
        }
        else
        {
            _skillList = skillList.ToArray();
        }
    }

    /// <summary>
    /// 新規でスキルを覚えるときに使う。
    /// </summary>
    public void SetPlayerSkill(AbstractSkill newSkill, Skill _forgettingSkill)
    {
        //新しく覚えるスキルのデータを取得
        var skillData = SkillManager.Instance.GetSkillData(newSkill);
        //忘れるスキルを一致するものを新しく覚えるスキルに変える
        if(_skillList[0] == _forgettingSkill)
        {
            _skillList[0] = skillData;
        }
        else if (_skillList[1] == _forgettingSkill)
        {
            _skillList[1] = skillData;
        }
        else if (_skillList[2] == _forgettingSkill)
        {
            _skillList[2] = skillData;
        }
        else if (_skillList[3] == _forgettingSkill)
        {
            _skillList[3] = skillData;
        }
    }
}
