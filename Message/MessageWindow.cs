using TMPro;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
[RequireComponent(typeof(PanelFade))]
public class MessageWindow : MonoBehaviour
{
    [SerializeField] MessageText _messageText;
    [SerializeField] RectTransform _mastRect;

    Queue<MessageText> _unusedTextQueue = new Queue<MessageText>();
    Queue<MessageText> _usingTextQueue = new Queue<MessageText>();
    PanelFade _panelFade;


    [NonSerialized] public int _writeWaittime = 40;
    [NonSerialized] public float _textWaitTime = 0.9f;
    public const float constwaitTime = 0.9f;
    const int constWriteWaittime = 40;

    bool _isTextWaiting = false;
    bool _canMove = true;
    private void Awake()
    {
        _panelFade = GetComponent<PanelFade>();
    }
    int onjectname = 0;
    async public UniTask WriteText(string message,sbyte addWaittime)
    {
        gameObject.SetActive(true);

        //未使用のテキストが無い場合は作成して_unusedTextQueueに追加する。
        if (_unusedTextQueue.Count == 0)
        {
            var mT = Instantiate(_messageText, _mastRect);
            _unusedTextQueue.Enqueue(mT);
            mT.gameObject.name = onjectname.ToString();
            onjectname++;
        }

        //未使用のテキストを_unusedTextQueueから取り出して使用する。
        var messageT = _unusedTextQueue.Dequeue();
        messageT._textRect.anchoredPosition = new Vector2(0, _mastRect.sizeDelta.y);
        messageT.ResetMessageTextData();
        messageT.SetText(message);

        //テキストを表示する時間を引く。待機しているテキストが多い場合に回転を速くするため。
        _textWaitTime -= 0.1f;
        if(_textWaitTime <= 0) { _textWaitTime = 0; }

        //メッセージウィンドウの高さを表示している全てのテキストが高さを超えている場合は待機する。
        while (_mastRect.sizeDelta.y <= _usingTextQueue.Sum(x => x._textHeight) + messageT._textHeight || !_canMove)
        {
            _isTextWaiting = true;
            await UniTask.Yield();
        }
        _isTextWaiting = false;
        _textWaitTime += 0.1f;
        if (_textWaitTime >= constwaitTime) { _textWaitTime = constwaitTime; }
        _usingTextQueue.Enqueue(messageT);

        //テキストを表示するポジションを計算する。
        var pos = CalcTextPosition(messageT._textHeight);
        messageT._textRect.anchoredPosition = pos;
        _writing = true;

        //テキストを１文字ずつ書く
        await messageT.ShowText(this);
        _writing = false;

        //テキストの移動
        MoveText(messageT, addWaittime).Forget();
    }
    bool _writing = false;

    //メッセージの表示速度を速くして待機時間を短くする。
    public void OnCancelMessageTask()
    {
        if (_usingTextQueue.Count == 0) { return; }
        _writeWaittime = 20;
        _textWaitTime = .2f;
    }

    //メッセージの表示速度と待機時間を戻す。
    public void UpResetSpeed()
    {
        if (_usingTextQueue.Count == 0) { return; }
        _writeWaittime = constWriteWaittime;
        _textWaitTime = constwaitTime;
    }



    async UniTask MoveText(MessageText mT,sbyte addWaittime)
    {
        while (_usingTextQueue.Peek() != mT || !_canMove || _writing)
        {
            await UniTask.Yield();
        }

        //テキストを待機する時間を計算して待機する。
        var secound = CalcTextLatency(mT,addWaittime);
        await UniTask.Delay(TimeSpan.FromSeconds(secound));
        _canMove = false;


        //テキストを上に移動させる。
        List<UniTask> tasks = new List<UniTask>();
        var height = _usingTextQueue.Peek()._textHeight;
        foreach (var t in _usingTextQueue)
        {
            //一番上に表示されているテキストと他のテキストで移動方法が異なる。
            if (_usingTextQueue.Peek() == t)
            {
                var task = TopTextMove(height, t);
                tasks.Add(task);
            }
            else
            {
                var task = OtherTextMove(height, t);
                tasks.Add(task);
            }
        }
        await UniTask.WhenAll(tasks);

        //移動が終わったら_usingTextQueueから_unusedTextQueueにテキストを移す。
        var tx = _usingTextQueue.Dequeue();
        _canMove = true;
        _unusedTextQueue.Enqueue(tx);

        //使用中のテキストが無く、待機中のテキストもない場合はメッセージウィンドウをフェードアウトする。
        if (_usingTextQueue.Count == 0 && !_isTextWaiting)
        {
            FadeOut().Forget();
        }
    }
    /// <summary>
    ///  一番上にあるテキストの移動
    /// </summary>
    async UniTask TopTextMove(float pos, MessageText t)
    {
        await t._textRect.DOMoveY(pos, 0.5f).SetRelative(true).ToUniTask();
        t._textRect.anchoredPosition = new Vector2(0, t._textHeight);
    }
    /// <summary>
    ///  一番上以外のテキストの移動
    /// </summary>
    async UniTask OtherTextMove(float pos, MessageText t)
    {
        var p = t._textRect.anchoredPosition.y + pos;
        await t._textRect.DOMoveY(pos, 0.5f).SetRelative(true).ToUniTask();
        t._textRect.anchoredPosition = new Vector2(0, p);
    }
    public bool isFadeOut = false;
    async public UniTask FadeOut()
    {
        _textWaitTime = constwaitTime;
        _writeWaittime = constWriteWaittime;
        isFadeOut = true;
        await _panelFade.AsyncFadeOut();
        isFadeOut = false;
    }

    /// <summary>
    ///  テキストの表示するポジションの計算
    /// </summary>
    Vector2 CalcTextPosition(float myheight)
    {
        float height = 0;
        height = -_usingTextQueue.Sum(x => x._textHeight);

        height += myheight;
        return new Vector2(0, height);
    }

    /// <summary>
    ///  テキストの待機時間を計算
    /// </summary>
    float CalcTextLatency(MessageText mT, sbyte addWaittime)
    {
        float secound = mT._messageText.text.Length * 0.1f;
        if (secound > 1f) { secound = 1; }
        secound += addWaittime;
        secound *= _textWaitTime;
        return secound;
    }
}
