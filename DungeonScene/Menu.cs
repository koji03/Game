using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject _menu;
    /// <summary>
    /// ���j���[��ʂ�\������
    /// </summary>
    public void OpenMenu()
    {
        _menu.SetActive(true);
    }
}
