using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class MessageManager : Singleton<MessageManager>
{
    public struct TextData
    {
        public string[] _texts;
        public sbyte _displayTime;
    }
    [SerializeField] MessageWindow _messageWindow;


    Queue<string> messageQueue = new Queue<string>();


    /// <summary>
    /// メッセージを表示
    /// </summary>
    public void ShowMessage(params string[] texts)
    {
        var data =new TextData();
        data._texts = texts;
        data._displayTime = 0;
        //テキストの表示
        AddTexts(data).Forget();
    }

    /// <summary>
    /// メッセージの表示時間を加算して表示
    /// </summary>
    public void ShowMessage(sbyte addwaittime,params string[] text)
    {
        var data = new TextData();
        data._texts = text;
        data._displayTime = addwaittime;
        //テキストの表示
        AddTexts(data).Forget();
    }


    bool isOpenTextArea = false;
    //messageQueueにテキストを追加して表示をする。
    async UniTask AddTexts(TextData texts)
    {
        //メッセージウィンドウがフェードアウト中は待機
        while(_messageWindow.isFadeOut)
        {
            await UniTask.Yield();
        }

        StringBuilder builder = new StringBuilder();
        foreach (var text in texts._texts)
        {
            builder.Append(text);
        }

        //既にテキストウィンドウが表示されている場合はmessageQueueにテキストを追加して終わる。
        messageQueue.Enqueue(builder.ToString());
        if (isOpenTextArea) { return; }
        _messageWindow._textWaitTime = MessageWindow.constwaitTime;
        ShowTexts(texts._displayTime).Forget();
    }
    //messageQueueが0になるまでループしてテキストを表示する。
    async UniTask ShowTexts(sbyte time)
    {
        isOpenTextArea = true;
        while (messageQueue.Count !=0)
        {
            var text = messageQueue.Dequeue();
            await _messageWindow.WriteText(text, time);
        }
        isOpenTextArea = false;
    }
}
