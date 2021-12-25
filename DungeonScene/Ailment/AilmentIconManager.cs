using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using Extensions;

public class AilmentIconManager : MonoBehaviour
{
    [SerializeField] AilmentIcon _ailmentIconPrefab;
    [SerializeField] Color positiveColor , negativeColor;
    public void Init()
    {
        var ailments = GameManager.Instance._player._statusAilment.GetStatusAilment();
        var stat = GameManager.Instance._player._spAilment.GetStatusAilment();

        //プレイヤーの状態異常が更新されたときに、画面に状態異常アイコンを表示したり隠したりする。
        ailments.ObserveEveryValueChanged(x => x.Count).Subscribe(_=> CheckAilmentIcon(ailments));

        //プレイヤーの特殊状態（バフ・デバフ等）が更新されたときに、画面にアイコンを表示したり隠したりする。
        stat.ObserveEveryValueChanged(x => x.Count()).Subscribe(_=> CheckAilmentIcon(stat));
    }

    List<AilmentIcon> _ailmenticons = new List<AilmentIcon>();
    List<AilmentIcon> _spailmenticons = new List<AilmentIcon>();

    /// <summary>
    /// 現在表示しているアイコンとプレイヤーの現在の状態異常を比較して、
    /// アイコンの作成と削除をする
    /// 特殊状態異常
    /// </summary>
    void CheckAilmentIcon(HashSet<SpAilment> playerspAilments)
    {
        //アイコンの作成
        foreach (var ailment in playerspAilments)
        {
            if (ailment == SpAilment.Null) { continue; }
            //既にアイコンを表示している場合は飛ばす。
            if (_spailmenticons.FirstOrDefault(x => x.ailmentname == ailment.GetSpAilmentName()))
            {
                continue;
            }
            //アイコンを作成する
            var icon = Instantiate(_ailmentIconPrefab, transform);
            icon.WriteIconText(ailment.GetSpAilmentName(), negativeColor,false);
            _spailmenticons.Add(icon);
        }

        //アイコンの削除
        foreach(var i in _spailmenticons)
        {
            var icon = playerspAilments.Any(x => x.GetSpAilmentName() == i.ailmentname);
            //状態異常が消えていたら削除する
            if (!icon)
            {
                i.gameObject.SetActive(false);
                Destroy(i.gameObject);
                i._isDelete = true;
            }
        }
        _spailmenticons.RemoveAll(x => x._isDelete);
    }
    /// <summary>
    /// 現在表示しているアイコンとプレイヤーの現在の状態異常を比較して、
    /// アイコンの作成と削除をする
    /// 通常の状態異常
    /// </summary>
    void CheckAilmentIcon(HashSet<Ailment> playerAilments)
    {
        //アイコンの作成
        foreach (var a in playerAilments)
        {
            if (a == Ailment.Null) { continue; }
            //既にアイコンを表示している場合は飛ばす。
            if (_ailmenticons.FirstOrDefault(x => x.ailmentname == a.GetAilmentName()))
            {
                continue;
            }
            var icon = Instantiate(_ailmentIconPrefab, transform);
            icon.WriteIconText(a.GetAilmentName(), positiveColor,false);
            _ailmenticons.Add(icon);
        }
        //アイコンの削除
        foreach (var i in _ailmenticons)
        {
            //状態異常が消えていたら削除する
            var icon = playerAilments.Any(x => x.GetAilmentName() == i.ailmentname);
            if (!icon)
            {
                i.gameObject.SetActive(false);
                Destroy(i.gameObject);
                i._isDelete = true;
            }
        }
        _ailmenticons.RemoveAll(x => x._isDelete);
    }

}
