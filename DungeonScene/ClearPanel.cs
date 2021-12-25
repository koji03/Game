using Cysharp.Threading.Tasks;
using UnityEngine;

public class ClearPanel : MonoBehaviour
{
    [SerializeField] RectTransform _BG;

    /// <summary>
    /// クリア情報が書かれているパネルを表示
    /// </summary>
    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }
    public void OnButton()
    {
        Loading.Instance.LoadScene("TitleScene", 1).Forget();
    }
}
