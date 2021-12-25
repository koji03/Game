using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Recovery", menuName = "ScriptableObjects/ItemData/Heart/Heart")]

public class HeartModel : ItemModel, IHeart
{
    [SerializeField] AudioClip _revivedSE;
    /// <summary>
    /// アイテムを直接使用
    /// </summary>
    async public override UniTask<bool> UseItem()
    {
        GameManager.Instance._player.DownHp(-999);
        GameManager.Instance._player.DownMp(-999);
        await GameManager.Instance._player.UseItem(_effect, _name);
        return true;
    }

    /// <summary>
    /// アイテム自動発動
    /// </summary>
    public void UseAuto()
    {
        //SEを再生
        SoundManager.Instance.PlaySE(_revivedSE);

        MessageManager.Instance.ShowMessage(_name, "の効果で復活した。");
        GameManager.Instance._player.DownHp(-999);
        GameManager.Instance._player.DownMp(-999);

        //バッグからアイテムを削除
        ItemManager.Instance.RemoveHaveItem(this);

        Locator<IEffectManager>.I.PlayEffect(GameManager.Instance._player.transform.position, _effect, GameManager.Instance._player.transform.rotation).Forget();
    }
}
