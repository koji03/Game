using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SkillReincarnation : AbstractSkill
{
    public override Func<UniTask> GetSkill()
    {
        return SkillAttack;
    }

    async UniTask SkillAttack()
    {
        //自分の目の前の8マスを取得
        //遠い順からブロックを調べ何もなかったらそこに移動する
        Player p = GameManager.Instance._player;
        var positions = AttackPositions.Foword(p.transform);

        var stage = GameManager.Instance._stagemanager;
        var block = stage.CheckBlock(stage._objectsData, Mathf.RoundToInt(positions[0].x), Mathf.RoundToInt(positions[0].z));
        bool _onItem = (block == null && stage.IsIn(positions[0]));
        if (_onItem)
        {
            var model = await MasterModel.Load<Model>(5002);
            var obj = stage.CreateObject(model, positions[0]);
            stage._objectsData.AddObjectData(objectType.Block, stage.xz2i(positions[0].x, positions[0].z), obj, model);
            //ここにアイテムドロップ宝箱出現させる
            Resources.UnloadUnusedAssets().ToUniTask().Forget();
        }

    }
}
