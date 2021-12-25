using Cysharp.Threading.Tasks;
using UnityEngine;

public class GiveUp : MonoBehaviour
{
    public void ShopGiveUp()
    {
        gameObject.SetActive(true);
    }
    public void OnGiveUp()
    {
        Play().Forget();
    }

    async public UniTask Play()
    {
        Loading.Instance.ShowLoading();
        await GameManager.Instance.SaveGiveUp();

        await Loading.Instance.LoadScene("TitleScene");
    }
}
