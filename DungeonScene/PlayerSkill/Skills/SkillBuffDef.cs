using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SkillBuffDef : AbstractSkill
{
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }
    async UniTask SkillAttack()
    {
        GameManager.Instance._player.AddBuff(SpAilment.DefBuff);
        await UniTask.Yield();
    }
}
