using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject _menu;
    /// <summary>
    /// ƒƒjƒ…[‰æ–Ê‚ğ•\¦‚·‚é
    /// </summary>
    public void OpenMenu()
    {
        _menu.SetActive(true);
    }
}
