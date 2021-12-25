using UnityEngine;


[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/SkillData/Skill")]
public class SkillModel : ScriptableObject
{
    public int _ID = 20000;
    public string _description;
    public string _name;
    public int _MP = 10;
    public int _strength = 10;
    public AttackDirection _direction;
    public EffectData _effect;
}