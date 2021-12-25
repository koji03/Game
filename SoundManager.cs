using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SoundManager : Singleton<SoundManager>
{
    [Range(0, 1f)]
    float _SEVolume;
    [Range(0, 1f)]
    float _BGMVolume;


    AudioSource _audioSource;
    public AudioClip OnButtonSouce;

    float _volume_magnification = 0.5f;
   async protected override UniTask Init()
    {
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        var sound = await SaveSetting.Deserialize();
        if(sound == null)
        {
            SetBGMVol(1);
            SetSEVol(1);
        }
        else
        {
            SetBGMVol(sound.BGMVolume);
            SetSEVol(sound.SEVolume);
        }

    }
    /// <summary>
    /// SEの再生
    /// </summary>
    /// <param name="SE"></param>
    public void PlaySE(AudioClip SE)
    {
        _audioSource.PlayOneShot(SE, GetSEVol());
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    /// <param name="BGM"></param>
    public void PlayBGM(AudioClip BGM)
    {
        _audioSource.clip = BGM;
        var sequence = DOTween.Sequence();
        //他のBGMが再生されている場合はフェードアウトしてから再生する
        if (_audioSource.volume != 0)
        {
            sequence.Append(_audioSource.DOFade(0, _volume_magnification));
        }
        sequence.Append(_audioSource.DOFade(GetBGMVol()* _volume_magnification, 1)).OnComplete(()=> _audioSource.Play());
    }

    public float GetSEVol()
    {
        return _SEVolume;
    }

    public float GetBGMVol()
    {
        return _BGMVolume;
    }

    public void SetSEVol(float vol)
    {
        _SEVolume = vol;
    }

    public void SetBGMVol(float vol)
    {
        _BGMVolume = vol;
        _audioSource.volume = vol* _volume_magnification;
    }
}