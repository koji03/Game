using System.Collections;
using TMPro;
using UnityEngine;

public class ItemCount : MonoBehaviour
{
    public TextMeshProUGUI _countText, _maxText;
    [System.NonSerialized]public byte _count = 0;
    [System.NonSerialized] public byte _max;
    public void OnItemCount(bool UP)
    {
        if(UP)
        {
            _count++;
        }
        else
        {
            _count--;
        }

        if (_count > _max)
        {
            _count = _max;
        }

        if (_count < 0) { _count = 0; }
        _countText.text = _count.ToString();
    }
    bool isPressDown = false;
    float _firstInterval = .5f;
    float _interval = 0.1f;
    Coroutine PressCorutine;

    public void PointerDown(bool UP)
    {
        //プレス開始
        isPressDown = true;

        //連続でタップした時に長押しにならないよう前のCoroutineを止める
        if (PressCorutine != null)
        {
            StopCoroutine(PressCorutine);
        }
        //StopCoroutineで止められるように予め宣言したCoroutineに代入
        PressCorutine = StartCoroutine(TimeForPointerDown(UP));
    }
    IEnumerator TimeForPointerDown(bool UP)
    {
        OnItemCount(UP);
        yield return new WaitForSeconds(_firstInterval);

        //待機時間
        while (isPressDown)
        {
            OnItemCount(UP);
            yield return new WaitForSeconds(_interval);

        }
    }

    //EventTriggerのPointerUpイベントに登録する処理
    public void PointerUp()
    {
        if (isPressDown)
        {
            isPressDown = false;
        }
    }
}
