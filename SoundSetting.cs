using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField]
    Slider _SESlider, _BGMSlider;

    [SerializeField]
    AudioClip SE;
    PanelFade _fade;
    public void Start()
    {
        _SESlider.SetValueWithoutNotify(SoundManager.Instance.GetSEVol()*10);
        _BGMSlider.SetValueWithoutNotify(SoundManager.Instance.GetBGMVol()*10);
        _fade = GetComponent<PanelFade>();
    }
    /// <summary>
    /// スライダーを動かしたときにSEを鳴らす。
    /// </summary>
    public void SESlider()
    {
        SoundManager.Instance.SetSEVol(_SESlider.value/10);
        SoundManager.Instance.PlaySE(SE);
    }
    public void BGMSlider()
    {
        SoundManager.Instance.SetBGMVol(_BGMSlider.value/10);
    }
    public void ClosePanel()
    {
        var bgm = SoundManager.Instance.GetBGMVol();
        var se = SoundManager.Instance.GetSEVol();
        SaveSetting.Serialize(bgm,se).Forget();
        _fade.FadeOut();
    }
}
