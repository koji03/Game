using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject _menu;
    /// <summary>
    /// メニュー画面を表示する
    /// </summary>
    public void OpenMenu()
    {
        _menu.SetActive(true);
    }
}
