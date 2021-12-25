using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
public class MessageText : MonoBehaviour
{
    int _textLength = 0;
    int _visibleLength = 0;
    public TextMeshProUGUI _messageText;
    public RectTransform _textRect;
    [System.NonSerialized]public float _textHeight;

    /// <summary>
    /// 文字数などのデータをリセットする。
    /// </summary>
    public void ResetMessageTextData()
    {
        _textLength = 0;
        _visibleLength = 0;
        _messageText.maxVisibleCharacters = 0;
        _textRect.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    //テキストに文字をセットして、文字数からサイズを調整する。
    /// </summary>
    public void SetText(string text)
    {
        _messageText.text = text;
        //テキストの文字数とテキストの高さを保存する。
        _textLength = text.Length;
        _textHeight = Mathf.Ceil(_messageText.preferredHeight);

        _textRect.sizeDelta = new Vector2(_textRect.sizeDelta.x, _textHeight);
    }

    /// <summary>
    /// 1文字ずつテキストを表示する
    /// </summary>
    async public UniTask ShowText(MessageWindow window)
    {
        while (_visibleLength < _textLength)
        {
            _visibleLength++;
            _messageText.maxVisibleCharacters = _visibleLength;
            await UniTask.Delay(window._writeWaittime);
        }
    }
}
