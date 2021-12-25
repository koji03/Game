using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Text;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _endLevel;
    [SerializeField] RectTransform _BG;

    /// <summary>
    /// ゲームオーバーの情報を書く
    /// </summary>
    public void Write(int currentlevel)
    {
        var sb = new StringBuilder();
        sb.Append(currentlevel.ToString());
        sb.Append("層で力尽きた。");
        _endLevel.text = sb.ToString();
    }
    /// <summary>
    /// ゲームオーバー時にパネルを表示する。
    /// </summary>
    async public UniTask ShowPanel()
    {
        await GameManager.Instance.SaveGiveUp();
        gameObject.SetActive(true);
        _BG.anchoredPosition = Vector3.up * 2200;
        _BG.DOAnchorPos(Vector2.zero, 0.8f).ToUniTask().Forget();
    }

    /// <summary>
    /// タイトル画面に戻る
    /// </summary>
    public void OnButton()
    {
        Loading.Instance.LoadScene("TitleScene", 1).Forget();
    }
}
