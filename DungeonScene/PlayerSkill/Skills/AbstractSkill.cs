using Cysharp.Threading.Tasks;
using System;

public abstract class AbstractSkill
{
    protected SkillModel _model;

    public void SetModel(int id)
    {
        _model = SkillManager.Instance.GetSkillModel(id);
    }

    public string GetSkillName()
    {
        return _model._name;
    }
    public SkillModel GetSkillModel()
    {
        return _model;
    }
    public  int GetID()
    {
        return _model._ID;
    }
    public string GetSkillDescription()
    {
        return _model._description;
    }

    public abstract Func<UniTask> GetSkill();
    public int GetMP()
    {
        return _model._MP;
    }

    public int GetStrength()
    {
        return _model._strength;
    }
}
