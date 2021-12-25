using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Extensions;

public enum Ailment
{
    Null = 0,
    Acclimation = 1,
    Poison = 2,
    Warm = 3,
    Curse = 4,
    Sleep = 5,
    BadPoison = 6,
    Burn = 7
}
public class StatusAilment : MonoBehaviour
{
    int EndCurseturn;
    int EndSleepturn;
    //状態異常
    HashSet<Ailment> _ailments = new HashSet<Ailment>();

    /// <summary>
    /// 状態異常を取得
    /// </summary>
    /// <returns></returns>
    public HashSet<Ailment> GetStatusAilment()
    {
        return _ailments;
    }

    /// <summary>
    /// 呪いの状態
    /// </summary>
    IDisposable curse;
    void CurseAilment()
    {
        EndCurseturn = GameManager.Instance._turnNum + 3;
        //3ターン経過したら呪いを治す
        curse = GameManager.Instance.ObserveEveryValueChanged(x => x._turnNum >= EndCurseturn).Skip(1).Subscribe(_=> CurseSubscribe());
    }
    /// <summary>
    /// 呪いのサブスクライブ
    /// </summary>
    void CurseSubscribe()
    {
        RecoveryAIlment(Ailment.Curse);
        curse.Dispose();
    }

    /// <summary>
    /// 催眠の状態
    /// </summary>
    IDisposable[] sleep = new IDisposable[2];
    void SleepAilment()
    {
        //8~2ターンの間催眠状態にする。
        int sleepMaxTurn = 8;
        int sleepMinTurn = 2;
        var cs = GetComponent<CharactorStatus>();
        int current = cs.GetHP();
        EndSleepturn = GameManager.Instance._turnNum + UnityEngine.Random.Range(sleepMinTurn, sleepMaxTurn);
        //ターンが過ぎるか、ダメージを食らうと催眠状態を治す。
        sleep[0] = GameManager.Instance.ObserveEveryValueChanged(x => x._turnNum >= EndSleepturn).Skip(1).Subscribe(_ => SleepSubscribe());
        sleep[1] = cs.ObserveEveryValueChanged(x => x.GetHP() < current).Skip(1).Subscribe(_ => SleepSubscribe());
    }
    /// <summary>
    /// 催眠のサブスクライブ
    /// </summary>
    void SleepSubscribe()
    {
        RecoveryAIlment(Ailment.Sleep);
        foreach(var i in sleep)
        {
            i.Dispose();
        }
    }

    /// <summary>
    /// ObserveEveryValueChangedを閉じる
    /// </summary>
    private void OnDestroy()
    {
        foreach (var i in sleep)
        {
            if(i !=null)
            {
                i.Dispose();
            }
        }
        if(curse!=null)
        {
            curse.Dispose();
        }
    }

    /// <summary>
    /// 状態異常付与
    /// </summary>
    public void AddStatusAilment(Ailment ailment,string myname)
    {
        var ailmentName = ailment.GetAilmentName();

        if (ailmentName != null)
        {
            if (_ailments.Contains(ailment))
            {
                MessageManager.Instance.ShowMessage(myname, "はすでに", ailmentName, "状態になっている。");
                return;
            }

            MessageManager.Instance.ShowMessage(myname,"が", ailmentName, "状態になった");
        }
        var result = _ailments.Add(ailment);
        if (!result) { return; }
        if (Ailment.Curse == ailment) { CurseAilment(); }
        if (Ailment.Sleep == ailment) { SleepAilment(); }
    }
    /// <summary>
    /// 指定の状態異常を回復する
    /// </summary>
    public void RecoveryAIlment(Ailment ailment)
    {
        var ailmentname = ailment.GetAilmentName();
        MessageManager.Instance.ShowMessage(ailmentname, "状態が治った。");
        _ailments.Remove(ailment);
        //毒状態を治すアイテムは猛毒も治す。
        if(ailment == Ailment.Poison)
        {
            _ailments.Remove(Ailment.BadPoison);
        }
    }
    /// <summary>
    /// 全ての状態異常を回復
    /// </summary>
    public void AllRecovery()
    {
        MessageManager.Instance.ShowMessage("全ての状態異常が治った。");
        _ailments.Clear();
    }
    /// <summary>
    /// 状態異常を追加
    /// </summary>
    public void AddAilments(int[]ailments)
    {
        foreach(var ailment in ailments)
        {
            _ailments.Add((Ailment)Enum.ToObject(typeof(Ailment), ailment));
        }
    }

    /// <summary>
    /// 指定した状態異常になっているかどうか
    /// </summary>
    /// <param name="ailments"></param>
    /// <returns></returns>
    public bool HaveAilment(params Ailment[] ailments)
    {
        foreach(var a in ailments)
        {
            if(_ailments.Contains(a))
            {
                return true;
            }
        }
        return false;
    }

}
