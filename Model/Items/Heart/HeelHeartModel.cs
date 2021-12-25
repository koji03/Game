using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Recovery", menuName = "ScriptableObjects/ItemData/Heart/HeelHeart")]

public class HeelHeartModel : ItemModel, IHeart
{
    [SerializeField] AudioClip _revivedSE;
    async public override UniTask<bool> UseItem()
    {
        //プレイヤーの最大HPを取得して1/3回復する。
        var hp = GameManager.Instance._player.GetMaxHP();
        var heel = hp / 3;
        GameManager.Instance._player.DownHp(-heel);

        //エフェクトを再生
        await GameManager.Instance._player.UseItem(_effect, _name);
        return true;
    }

    public void UseAuto()
    {
        //SEを再生
        SoundManager.Instance.PlaySE(_revivedSE);
        MessageManager.Instance.ShowMessage(_name, "の効果で体力を回復した。");

        //プレイヤーの最大HPを取得して1/3回復する。
        var hp = GameManager.Instance._player.GetMaxHP();
        var heel = hp / 3;
        GameManager.Instance._player.DownHp(-heel);

        //バッグからアイテムを削除
        ItemManager.Instance.RemoveHaveItem(this);

        //エフェクトを再生
        Locator<IEffectManager>.I.PlayEffect(GameManager.Instance._player.transform.position, _effect, GameManager.Instance._player.transform.rotation).Forget();
    }

}
