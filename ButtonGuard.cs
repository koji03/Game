using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// クリック防止
/// </summary>
public class ButtonGuard : Singleton<ButtonGuard>
{
    [SerializeField] GameObject _panel;

    void Start()
    {
        CancelGuard();
    }

    async protected override UniTask Init()
    {
        DontDestroyOnLoad(gameObject);
        _panel.SetActive(false);
    }
    public void Guard()
    {
        _panel.SetActive(true);
    }
    public void CancelGuard()
    {
        _panel.SetActive(false);
    }
}
