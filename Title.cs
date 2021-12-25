using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] Sprite[] BGS;
    [SerializeField] Image _BG;

    [SerializeField] RecordPanel _recordPanel;
    private void Awake()
    {
        Application.targetFrameRate = 60; //フレームレートを60に設定
        //ランダムに背景を設定
        _BG.sprite = BGS[Random.Range(0, BGS.Length)];

        var bgm = Resources.Load<AudioClip>("BGM/5");
        SoundManager.Instance.PlayBGM(bgm);
        _clockFlag = false;
    }

    bool _clockFlag=false;

    public void OnTutorial()
    {
        Setting.IsTutorial = true;
        Level._Level = 255;
        Loading.Instance.LoadScene("StageScene", 1).Forget();
    }
    public void OnStart()
    {
        Setting.IsTutorial = false;
        Level._Level = 100;
        Loading.Instance.LoadScene("StageScene", 1).Forget();
    }
    public void TouchScreen()
    {
        Debug.Log(_clockFlag);
        if(!_clockFlag)
        {
            _clockFlag = true;
            ShowRecordScreen().Forget();
        }
    }
    bool firstAd = true;
    async UniTask ShowRecordScreen()
    {
        Loading.Instance.ShowLoading();
        //広告を1回表示したら表示しないようにする。
        if (firstAd)
        {
            firstAd = false;
            //広告表示
            await UnityAds.ShowUnityInterstitialAds();
        }
        var data = await SaveRecordData.Deserialize();
        await Loading.Instance.HideLoading(1);
        _recordPanel.ShowPanel(data.currentLevel, data.maxLevel);
        _clockFlag = false;
    }

}
