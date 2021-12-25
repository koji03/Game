using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SkillDomeShield : AbstractSkill
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
        var positions = AttackPositions.TwoSquaresAround(p.transform);


        var blockmodel = await MasterModel.Load<Model>(30001);

        var stage = GameManager.Instance._stagemanager;
        Vector3 position = p.transform.position;
        for (int i = positions.Length - 1; i > 0; i--)
        {
            if(position == positions[i]) { break; }
            var block = stage.CheckBlock(stage._objectsData, Mathf.RoundToInt(positions[i].x), Mathf.RoundToInt(positions[i].z));
            bool _onItem = (block == null && stage.IsIn(positions[i]));
            if (_onItem)
            {
                var obj = stage.CreateObject(blockmodel, positions[i]);
                stage._objectsData.AddObjectData(objectType.Block, stage.xz2i(positions[i].x, positions[i].z), obj, blockmodel);
            }
        }

        Resources.UnloadUnusedAssets().ToUniTask().Forget();
    }
}
