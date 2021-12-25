using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゲームの中断
/// </summary>
public class Suspend : MonoBehaviour
{
    public void OnSuspend()
    {
        Loading.Instance.LoadScene("TitleScene", 1).Forget();
    }
    public void OnShowSuspend()
    {
        gameObject.SetActive(true);
    }
}
