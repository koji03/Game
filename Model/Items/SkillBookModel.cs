using Cysharp.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "SkillBook", menuName = "ScriptableObjects/ItemData/SkillBook")]
public class SkillBookModel : ItemModel
{
    public Skill _skill;
    //スキルを獲得する。
    async public override UniTask<bool> UseItem()
    {
        //スキルのデータを取得
        var skill =  SkillManager.Instance.GetSkillData(_skill);
        var result = await SkillManager.Instance.LearnNewSkill(skill,GameManager.Instance._player.GetSkillList());
        //「スキルを新しく覚える＝アイテムを使用」した場合
        if (result)
        {
            //アイテム使用後のエフェクト等を出す。
            GameManager.Instance._player.UseItem(_effect,_name).Forget();
        }
        return result;
    }

}
