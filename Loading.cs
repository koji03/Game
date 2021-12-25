using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

/// <summary>
/// ロード中の画面
/// </summary>
public class Loading : Singleton<Loading>
{
    [SerializeField]Text _text;
    [SerializeField] float _delayTime,_textTime;
    [SerializeField] string text;
    [SerializeField] GameObject _canvas;
    [SerializeField] Sprite[] BGS;
    [SerializeField] Image _SceneLoadBG,_DataLoadBG;
    Sequence sequence;

    async protected override UniTask Init()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneUnloaded += SceneLoaded;
    }
    /// <summary>
    /// ロード中の画面を出す。
    /// </summary>
    public void ShowLoading()
    {
        if (sequence != null) { sequence.Kill(); }
        //背景画像をランダムで選択
        _SceneLoadBG.sprite = BGS[Random.Range(0, BGS.Length)];
        sequence = DOTween.Sequence();
        sequence.Append(_text.DOText(text, _textTime));
        sequence.SetDelay(_delayTime).SetLoops(-1);
        sequence.Play();
        _SceneLoadBG.gameObject.SetActive(true);
        _DataLoadBG.gameObject.SetActive(false);
        _canvas.SetActive(true);
    }
    /// <summary>
    /// ロード中の画面を隠す
    /// </summary>
    async public UniTask HideLoading(int waitTime)
    {
        await UniTask.Delay(waitTime * 1000);
        sequence = null;
        _canvas.SetActive(false);
    }


    // イベントハンドラー（イベント発生時に動かしたい処理）
    void SceneLoaded(Scene thisScene)
    {
        sequence = null;
        _canvas.SetActive(false);
    }
    /// <summary>
    /// シーンのロードしながらロード画面を出す。
    /// </summary>
    /// <param name="SceneName"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    async public UniTask LoadScene(string SceneName, int waitTime = 1)
    {
        ShowLoading();
        var async = SceneManager.LoadSceneAsync(SceneName);
        async.allowSceneActivation = false;
        while (true)
        {
            await UniTask.Yield();
            // 読み込み完了したら
            if (async.progress >= 0.9f)
            {
                break;
            }
        }
        // シーン読み込み
        await UniTask.Delay(waitTime * 1000);
        async.allowSceneActivation = true;

    }
}
