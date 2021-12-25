using UnityEngine;

public class StageBGM : MonoBehaviour
{
    /// <summary>
    /// ステージの進行度でBGMを変える。
    /// </summary>
    public void PlayBGM(StageType type)
    {
        switch(type)
        {
            case StageType.Forest:
                var fb = Resources.Load<AudioClip>("BGM/0");
                SoundManager.Instance.PlayBGM(fb);
                break;
            case StageType.Deseart:
                var db = Resources.Load<AudioClip>("BGM/1") ;
                SoundManager.Instance.PlayBGM(db);
                break;
            case StageType.Winter:
                var wb = Resources.Load<AudioClip>("BGM/2") ;
                SoundManager.Instance.PlayBGM(wb);
                break;
            case StageType.Dungeon:
                var dub = Resources.Load<AudioClip>("BGM/3") ;
                SoundManager.Instance.PlayBGM(dub);
                break;
            case StageType.Last:
                var lb = Resources.Load<AudioClip>("BGM/4") ;
                SoundManager.Instance.PlayBGM(lb);
                break;
        }
    }
}
