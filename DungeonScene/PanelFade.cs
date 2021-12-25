using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;


/// <summary>
/// オブジェクトを移動して表示させる
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PanelFade : MonoBehaviour
{

    Vector2? _initialPosition = null;
    [SerializeField] Vector3 _hidePosition = new Vector3(0, -1500, 0);
    [SerializeField] float duration = 0.3f;
    CanvasGroup canvas;
    private void Awake()
    {
        if(_initialPosition == null)
        {
            _initialPosition = GetComponent<RectTransform>().anchoredPosition;
        }
        canvas = GetComponent<CanvasGroup>();
    }
    private void OnEnable()
    {
        //隠すポジションから表示位置まで移動させてフェードインする。
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = _hidePosition;

        rectTransform.DOAnchorPos((Vector2)_initialPosition, .5f).SetEase(Ease.OutQuart);
        FadeIn(canvas, 0.1f);
    }
    public void OnActive()
    {
        gameObject.SetActive(true);
    }

    void FadeIn(CanvasGroup canvasGroup, float duration = 0.1f)
    {
        //フェードイン中はクリックを無効にする。
        ButtonGuard.Instance.Guard();
        DOTween.Sequence()
            .Append(canvasGroup.DOFade(1.0F, duration)).OnComplete(ButtonGuard.Instance.CancelGuard);
    }
    /// <summary>
    /// フェードアウト
    /// </summary>
    public void FadeOut()
    {
        //フェードアウト中はクリックを無効にする。
        ButtonGuard.Instance.Guard();
        if (_initialPosition == null)
        {
            _initialPosition = GetComponent<RectTransform>().anchoredPosition;
        }
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        var rectTransform = GetComponent<RectTransform>();
        //移動とフェードアウト
        rectTransform.DOAnchorPos(_hidePosition, duration).SetRelative(true).SetEase(Ease.InQuart).OnComplete(()=>gameObject.SetActive(false));
        canvas.DOFade(0.0F, duration).SetEase(Ease.InQuart).OnComplete(ButtonGuard.Instance.CancelGuard);
    }
    /// <summary>
    /// 非同期
    /// </summary>
    async public UniTask AsyncFadeOut()
    {
        ButtonGuard.Instance.Guard();
        if (_initialPosition == null)
        {
            _initialPosition = GetComponent<RectTransform>().anchoredPosition;
        }
        CanvasGroup canvas = GetComponent<CanvasGroup>();
        var rectTransform = GetComponent<RectTransform>();
       var task1 = rectTransform.DOAnchorPos(_hidePosition, duration).SetRelative(true).SetEase(Ease.InQuart).OnComplete(() => gameObject.SetActive(false)).ToUniTask();
       var task2 =  canvas.DOFade(0.0F, duration).SetEase(Ease.InQuart).OnComplete(ButtonGuard.Instance.CancelGuard).ToUniTask();

        await UniTask.WhenAll(task1, task2);
    }
}
