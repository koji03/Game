using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
public class PlayerController : MonoBehaviour
{

    bool isPressDown = false;
    float _firstInterval = .5f;
    float _interval = 0.1f;
    //StopCoroutineのためにCoroutineで宣言しておく
    Coroutine PressCorutine;
    bool _centerPush = false;

    [SerializeField] Menu _menu;

    //中央のボタンを押したとき
    public void PushDown()
    {
        _centerPush = true;
    }
    //中央のボタンを離したとき
    public void PushUp()
    {
        _centerPush = false;
    }

    /// <summary>
    /// 中央ボタンを押している状態で方向キーにスライドしたら回転をする
    /// </summary>
    /// <param name="direction"></param>
    public void PushEnter(int direction)
    {
        if(_centerPush && !GameManager.Instance._playerSelect)
        {
            GameManager.Instance._player.Rotate((Direction)Enum.ToObject(typeof(Direction), direction));
        }
    }

    /// <summary>
    /// 方向キーを押したとき
    /// </summary>
    public void PushKey(int direction)
    {
        //プレス開始
        isPressDown = true;

        //連続でタップした時に長押しにならないよう前のCoroutineを止める
        if (PressCorutine != null)
        {
            StopCoroutine(PressCorutine);
        }
        //StopCoroutineで止められるように予め宣言したCoroutineに代入
        PressCorutine = StartCoroutine(TimeForPointerDown(direction));
    }
    /// <summary>
    /// ターンを飛ばす（ターンを終了する）
    /// </summary>
    public void OnSkip()
    {
        GameManager.Instance.PlayerAction(ConsecutiveTimes.Reset).Forget();
    }

    /// <summary>
    /// 通常攻撃ボタン
    /// </summary>
    public void OnAttack()
    {
        Player p = GameManager.Instance._player;
        //催眠状態なら何もしない
        if (p._statusAilment.HaveAilment(Ailment.Sleep))
        {
            return;
        }
        //武器を装備していない場合
        CharactorEquip weapon = p._charactorWeapon;
        if(weapon.GetWeapon() == null)
        {
            GameManager.Instance.PlayerAction(() => p.OnAttack(
            AttackDirection.Foword,
            false,
            0,
            100), ConsecutiveTimes.One).Forget();
        }
        else
        {
            GameManager.Instance.PlayerAction(() => p.OnAttack(
            weapon.GetWeaponDirection(),
            weapon.GetisPenetrating(),
            weapon.GetWeaponStrength(),
            weapon.GetHitRate()), weapon.GetTimes()).Forget();
        }
    }
    /// <summary>
    /// 盾を構える（防御状態になる）
    /// </summary>
    public void OnDefend()
    {
        Player p = GameManager.Instance._player;
        //催眠状態の場合は何もしない
        if (p._statusAilment.HaveAilment(Ailment.Sleep))
        {
            return;
        }
        GameManager.Instance.PlayerAction(() => p.OnDefend(),p._charactorWeapon.GetTimes()).Forget();
    }
    /// <summary>
    /// 使用可能なスキルのリストを表示
    /// </summary>
    public void OnSkill()
    {
        //催眠状態の場合は何もしない
        if (GameManager.Instance._player._statusAilment.HaveAilment(Ailment.Sleep))
        {
            return;
        }
        //スキルパネルを表示する
        var skillList = GameManager.Instance._player.GetSkillList();
        SkillManager.Instance.ShowPlayerSkillsPanel(skillList);
    }
    /// <summary>
    /// 所持アイテムを表示
    /// </summary>
    public void OnItem()
    {
        //アイテムパネルを表示する。
        _menu.OpenMenu();
    }

    /// <summary>
    /// 長押し処理
    /// </summary>
    IEnumerator TimeForPointerDown(int direction)
    {
        //プレイヤーを移動させる。
        Player p = GameManager.Instance._player;
        var times = p._charactorWeapon.GetTimes();
        GameManager.Instance.PlayerAction(() => p.ATMove((Direction)Enum.ToObject(typeof(Direction), direction)), times).Forget();
        yield return new WaitForSeconds(_firstInterval);

        //長押ししている場合は連続で移動。
        while (isPressDown)
        {
            GameManager.Instance.PlayerAction(() => GameManager.Instance._player.ATMove((Direction)Enum.ToObject(typeof(Direction), direction)), times).Forget();
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
