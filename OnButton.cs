using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンのSEを再生
/// </summary>
public class OnButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioClip _clip;
    void Start()
    {
        AudioClip clip = SoundManager.Instance.OnButtonSouce;
        if (_clip !=null)
        {
            clip = _clip;
        }
        Button b = GetComponent<Button>();
        if(b !=null)
        {
            b.onClick.AddListener(() => SoundManager.Instance.PlaySE(clip));
        }
    }
}
