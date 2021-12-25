using UnityEngine;

//プレイヤーのレベルアップ時のスキルデータ
public class LevelSkills : MonoBehaviour
{
    [SerializeField] byte _learnSkillInterval = 5;
    [SerializeField]SkillModel[] _playerSkills;
    /// <summary>
    /// プレイヤーのレベルがスキル習得レベルになっていた場合はスキルを返す。
    /// </summary>
    public SkillModel GetSkillModels(int playerLv)
    {
        if(playerLv % _learnSkillInterval != 0) { return null; }
        var learningLV = (playerLv / _learnSkillInterval) - 1;
        if (_playerSkills.Length <= learningLV) { return null; }
        return _playerSkills[learningLV];
    }
}
