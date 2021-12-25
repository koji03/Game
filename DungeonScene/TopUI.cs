using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using System;

public class TopUI : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _floor,_playerLvlText,_lifeText;
    [SerializeField]Text _HPLabel, _maxHPLabel,_MPLabel, _maxMPLabel;
    [SerializeField]Slider _HPSlider, _MPSlider;
    [SerializeField] Image _lifecircle;
    int _hp;
    int _mp;

    private void OnDestroy()
    {
        foreach(var dispose in _disposables)
        {
            if(dispose !=null)
            {
                dispose.Dispose();
            }
        }
    }

    IDisposable[] _disposables = new IDisposable[5];
    public void Init(int hp,int mp,int maxHP,int maxMP,int floor, int _playerLvl)
    {
        _hp = hp;
        _mp = mp;
        _HPLabel.text = hp.ToString();
        _MPLabel.text = mp.ToString();
        _maxHPLabel.text = "/" + maxHP;
        _maxMPLabel.text = "/" + maxMP;
        _playerLvlText.text = _playerLvl.ToString();
        _HPSlider.maxValue = maxHP;
        _HPSlider.value = maxHP;
        SetFloor(floor);

        Player p = GameManager.Instance._player;
        //プレイヤーの満腹度が減ったらゲージを減らす
        _disposables[0] = p.ObserveEveryValueChanged(_ => _._life).Subscribe(_ => ChangeLife(_));
        //プレイヤーのHPが減ればHPバーを減らす
        _disposables[1] = p.ObserveEveryValueChanged(_ => _.GetHP()).Subscribe(_ => DownHP(GameManager.Instance._player.GetHP()));
        //プレイヤーのMPが減ればMPバーを減らす
        _disposables[2] = p.ObserveEveryValueChanged(_ => _.GetMP()).Subscribe(_ => DownMP(GameManager.Instance._player.GetMP()));
        //レベルが上がった時にLvの表示を変える。
        _disposables[3] = p.ObserveEveryValueChanged(_ => _.GetLv()).Subscribe(_ => LvUp(GameManager.Instance._player.GetLv(), GameManager.Instance._player.GetMaxHP(), GameManager.Instance._player.GetMaxMP()));
        //階層が変わった時に表示を変える
        _disposables[4] = GameManager.Instance.ObserveEveryValueChanged(_ => _._currentStage).Subscribe(_ => SetFloor(GameManager.Instance._currentStage));
    }

    void ChangeLife(float life)
    {
        _lifeText.text = life.ToString();
        _lifecircle.fillAmount = life / 100;
    }
    void DownHP(int downHP)
    {
        float time = 0.3f;
        _HPSlider.DOValue(downHP, time);
        _HPLabel.DOCounter(_hp, downHP, time);
        _hp = downHP;
    }
    void DownMP(int downMP)
    {
        float time = 0.3f;
        _MPSlider.DOValue(downMP, time);
        _MPLabel.DOCounter(_hp, downMP, time);
        _mp = downMP;
    }
    void MaxHPUP(int up)
    {
        _HPSlider.maxValue = up;
        _maxHPLabel.text = "/"+_HPSlider.maxValue;

    }
    void MaxMPUP(int up)
    {
        _MPSlider.maxValue = up;
        _maxMPLabel.text = "/" + _MPSlider.maxValue;

    }
    void LvUp(int _playerLvl,int hp, int mp)
    {
        _playerLvlText.text = _playerLvl.ToString();
        MaxHPUP(hp);
        MaxMPUP(mp);
    }
    void SetFloor(int floor)
    {
        _floor.text = floor.ToString();
    }
}
