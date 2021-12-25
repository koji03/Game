using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

/// <summary>
/// 行動力のスライダー
/// </summary>
public class ActSlider: MonoBehaviour
{
    public float _intervalcount { get; private set; }
    float _baseCount = 60;
    float _maxCount;
    [SerializeField]Slider _timeSlider;

    Tweener _sliderTweener;

    //スライダーを減らす。
    void DonwActSlider(float time)
    {
        _maxCount -= time;
        _sliderTweener = _timeSlider.DOValue(_maxCount, 0.5f);
    }
    //スライダーのリセット
    async UniTask ResetSlider(float maxCount)
    {
        //スライダーの動きを終わらせる。
        _sliderTweener.Kill(false);
        await _timeSlider.DOValue(0, 0.1f).ToUniTask();
        _timeSlider.maxValue = maxCount;
        _maxCount = maxCount;
        await _timeSlider.DOValue(maxCount, 0.2f).ToUniTask();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    public void CountDown(ConsecutiveTimes count)
    {
        var num = CalcConsecutiveTimes(count);

        _intervalcount -= num;
        DonwActSlider(num);
    }

    public void ResetCount()
    {
        _intervalcount = _baseCount;
        ResetSlider(_baseCount).Forget();
    }
    /// <summary>
    /// 行動できる回数を数値に変換して返す。
    /// </summary>
    float CalcConsecutiveTimes(ConsecutiveTimes times)
    {
        if(GameManager.Instance.GetEnemies().Count == 0)
        {
            GameManager.Instance._player._canMove = true;
        }
        if (GameManager.Instance.GetIsHalf())
        {
            switch (times)
            {
                case ConsecutiveTimes.One:
                    return Mathf.Ceil(_baseCount/2);
                case ConsecutiveTimes.Two:
                    return Mathf.Ceil(_baseCount / 3);
                case ConsecutiveTimes.Three:
                    return Mathf.Ceil(_baseCount / 4);
                case ConsecutiveTimes.Four:
                    return Mathf.Ceil(_baseCount / 5);
                case ConsecutiveTimes.Five:
                    return Mathf.Ceil(_baseCount / 6);
                case ConsecutiveTimes.Reset:
                    return Mathf.Ceil(_baseCount);
                default:
                    return 0;
            }
        }
        else
        {
            switch (times)
            {
                case ConsecutiveTimes.One:
                    return Mathf.Ceil(_baseCount);
                case ConsecutiveTimes.Two:
                    return Mathf.Ceil(_baseCount / 2);
                case ConsecutiveTimes.Three:
                    return Mathf.Ceil(_baseCount / 3);
                case ConsecutiveTimes.Four:
                    return Mathf.Ceil(_baseCount / 4);
                case ConsecutiveTimes.Five:
                    return Mathf.Ceil(_baseCount / 5);
                case ConsecutiveTimes.Reset:
                    return Mathf.Ceil(_baseCount);
                default:
                    return 0;
            }
        }

    }
}
public enum ConsecutiveTimes
{
    Zero = 0,
    One = 1,
    Two = 2,
    Three =3,
    Four =4,
    Five =5,
    Reset = 6,
}
