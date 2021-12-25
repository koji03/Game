using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// 特殊な状態
/// </summary>
public enum SpAilment
{
    Null = 0,
    AtkBuff = 1,
    DefBuff = 2,
    AtkDeBuff = 3,
    DefDeBuff = 4,
    SpeedUp = 5,
    StopTime = 6,
    Star = 7,
    MPHalf = 8,
    Safeguard = 9,
    Insomnia = 10
}
public class StatusSpAilment : MonoBehaviour
{
    HashSet<SpAilment> _ailments = new HashSet<SpAilment>();

    [System.NonSerialized]public byte _starCount;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        var stageLv = GameManager.Instance;
        //特殊状態異常は階層が変われば終わる。
        stageLv.ObserveEveryValueChanged(_ => _._currentStage).Subscribe(_ => ResetSpAilment()).AddTo(this);
    }
    /// <summary>
    /// 現在の特殊状態異常をすべて取得
    /// </summary>
    public HashSet<SpAilment> GetStatusAilment()
    {
        return _ailments;
    }
    /// <summary>
    /// 特殊状態異常を追加
    /// </summary>
    public void AddStatusAilment(SpAilment ailment)
    {
        _ailments.Add(ailment);
    }
    /// <summary>
    /// 指定の特殊状態異常を持っているかどうか
    /// </summary>
    public bool HaveAilment(params SpAilment[] ailments)
    {
        foreach (var a in ailments)
        {
            if (_ailments.Contains(a))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 全ての特殊状態異常を削除
    /// </summary>
    public void ResetSpAilment()
    {
        _ailments.Clear();
    }
    /// <summary>
    /// 指定の特殊状態異常を回復
    /// </summary>
    /// <param name="sp"></param>
    public void RemoveSpAilment(SpAilment sp)
    {
        _ailments.Remove(sp);
    }
    /// <summary>
    /// スター状態のカウントダウン
    /// </summary>
    public void StarCountDown()
    {
        _starCount--;
        if(_starCount ==0)
        {
            _ailments.Remove(SpAilment.Star);
        }
    }
}
