
using Cysharp.Threading.Tasks;
using System;

public class SkillData
{
    public AbstractSkill GetSkillData(Skill skill)
    {
        switch (skill)
        {
            case Skill.PowerAttack:
                SkillPowerAttack pA = new SkillPowerAttack();
                pA.SetModel(((int)skill));
                return pA;
            case Skill.Heel:
                SkillHeel heel = new SkillHeel();
                heel.SetModel(((int)skill));
                return heel;
            case Skill.Long:
                SkillLongAttack longAttack = new SkillLongAttack();
                longAttack.SetModel(((int)skill));
                return longAttack;
            case Skill.Gluttony:
                SkillGluttony skillGluttony = new SkillGluttony();
                skillGluttony.SetModel(((int)skill));
                return skillGluttony;
            case Skill.Teleport:
                SkillTeleport skillTeleport = new SkillTeleport();
                skillTeleport.SetModel(((int)skill));
                return skillTeleport;
            case Skill.Star:
                SkillStar skillStar = new SkillStar();
                skillStar.SetModel(((int)skill));
                return skillStar;
            case Skill.DomeShiekd:
                SkillDomeShield skillDomeShield = new SkillDomeShield();
                skillDomeShield.SetModel(((int)skill));
                return skillDomeShield;
            case Skill.Drop:
                SkillDrop skillDrop = new SkillDrop();
                skillDrop.SetModel(((int)skill));
                return skillDrop;
            case Skill.MeteorShower:
                SkillMeteorShower skillMeteorShower = new SkillMeteorShower();
                skillMeteorShower.SetModel(((int)skill));
                return skillMeteorShower;
            case Skill.AllBuff:
                SkillBuffAtk skillAtkBuff = new SkillBuffAtk();
                skillAtkBuff.SetModel(((int)skill));
                return skillAtkBuff;
            case Skill.UTurn:
                SkillUTurn UTurn = new SkillUTurn();
                UTurn.SetModel(((int)skill));
                return UTurn;
            case Skill.StopTime:
                SkillStopTime Stop = new SkillStopTime();
                Stop.SetModel(((int)skill));
                return Stop;
            case Skill.EnergyDrain:
                SkillEnergydrain drain = new SkillEnergydrain();
                drain.SetModel(((int)skill));
                return drain;
            case Skill.Curse:
                SkillCurse SkillCurse = new SkillCurse();
                SkillCurse.SetModel(((int)skill));
                return SkillCurse;
            case Skill.Hypnosis:
                SkillHyponosis SkillHyponosis = new SkillHyponosis();
                SkillHyponosis.SetModel(((int)skill));
                return SkillHyponosis;
            case Skill.Smash:
                SkillSmashAttack SkillSmashAttack = new SkillSmashAttack();
                SkillSmashAttack.SetModel(((int)skill));
                return SkillSmashAttack;
            case Skill.Sucker:
                SkillSucker SkillSucker = new SkillSucker();
                SkillSucker.SetModel(((int)skill));
                return SkillSucker;
            case Skill.Explosion:
                SkillExplosion SkillExplosion = new SkillExplosion();
                SkillExplosion.SetModel(((int)skill));
                return SkillExplosion;
            case Skill.Motivation:
                SkillMotivation SkillMotivation = new SkillMotivation();
                SkillMotivation.SetModel(((int)skill));
                return SkillMotivation;
            case Skill.SpinningSlash:
                SkillSpinningSlash SkillSpinningSlash = new SkillSpinningSlash();
                SkillSpinningSlash.SetModel(((int)skill));
                return SkillSpinningSlash;
            case Skill.TakeDown:
                SkillTakeDown SkillTakeDown = new SkillTakeDown();
                SkillTakeDown.SetModel(((int)skill));
                return SkillTakeDown;
            case Skill.WideAttack:
                SkillWideAttack SkillWideAttack = new SkillWideAttack();
                SkillWideAttack.SetModel(((int)skill));
                return SkillWideAttack;
            case Skill.Recovery:
                SkillRecovery SkillRecovery = new SkillRecovery();
                SkillRecovery.SetModel(((int)skill));
                return SkillRecovery;
            case Skill.BadPoison:
                SkillBadPoison SkillBadPoison = new SkillBadPoison();
                SkillBadPoison.SetModel(((int)skill));
                return SkillBadPoison;
            case Skill.Safegurad:
                SkillSafeguard SkillSafeguard = new SkillSafeguard();
                SkillSafeguard.SetModel(((int)skill));
                return SkillSafeguard;
            case Skill.BurnAttack:
                SkillBurnAttack SkillBurnAttack = new SkillBurnAttack();
                SkillBurnAttack.SetModel(((int)skill));
                return SkillBurnAttack;
            default:
                SkillNull skillNull = new SkillNull();
                skillNull.SetModel(((int)skill));
                return skillNull;
        }
    }

    public Skill GetSkillType(AbstractSkill skill)
    {
        if(skill == null) { return Skill.Null; }
        var model = skill.GetSkillModel();
        return (Skill)Enum.ToObject(typeof(Skill), model._ID);
    }
}
