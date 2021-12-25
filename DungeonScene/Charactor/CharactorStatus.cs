using UnityEngine;
using UniRx;
using System.Text;
using Cysharp.Threading.Tasks;
using System;


//プレイヤーとエネミーのステータスを計算する。
public class CharactorStatus : MonoBehaviour
{
    int _Lvl;
    int _HP;
    int _MaxHP;
    int _MP;
    int _MaxMP;
    int _Atk;
    int _Def;

    //計算の元になる値
    int _baseHP;
    int _baseMP;
    int _baseAtk;
    int _baseDef;

    public int _maxLevel = 200;
    [NonSerialized]public int _currentExp;
    public int _totalExp { get;private set; }
    public int _baseExp = 1000;
    [NonSerialized] public int[] expToNextLevel;

    //レベルで覚えるスキルのリスト
    LevelSkills _levelSkills;
    //レベルアップ時のエフェクト
    [SerializeField] EffectData _LvupEffect;
    StatusSpAilment _sp;

    public int GetLv()
    {
        return _Lvl;
    }
    public int GetHP()
    {
        return _HP;
    }
    public int GetMaxHP()
    {
        return _MaxHP;
    }
    public int GetMP()
    {
        return _MP;
    }
    public int GetMaxMP()
    {
        return _MaxMP;
    }
    /// <summary>
    /// バフがかかっていたら２倍
    /// </summary>
    public int GetAtk()
    {
        if (_sp.HaveAilment(SpAilment.AtkBuff)) return (int)(_Atk * 2f);
        return _Atk;
    }
    /// <summary>
    /// バフがかかっていたら２倍
    /// </summary>
    public int GetDef()
    {
        if (_sp.HaveAilment(SpAilment.DefBuff)) return (int)(_Def * 2f);
        return _Def;
    }
    public void DownHP(int hp)
    {
        _HP -= hp;
        //ダメージテキストを表示
        DamageTextObjectPool.Instance.ShowDamageText(hp, transform.position);
    }
    public void SetHP(int hp)
    {
        _HP = hp;
    }
    public void SetMP(int mp)
    {
        _MP = mp;
        //最大値を超えないようにする。
        if (_MP > _MaxMP)
        {
            _MP = _MaxMP;
        }
    }

    public void DownMP(int mp)
    {
        _MP -= mp;
        //最大値を超えないようにする
        if (_MP > _MaxMP)
        {
            _MP = _MaxMP;
        }
    }
    /// <summary>
    /// 初期化
    /// ステータスの計算と経験値の計算
    /// </summary>
    public void Init(CharactorModel _model,int lv =1)
    {
        //レベルで覚えるスキルを格納
        _levelSkills = GetComponent<LevelSkills>();
        _sp = GetComponent<StatusSpAilment>();

        _baseHP = _model._baseHP;
        _baseMP = _model._baseMP;
        _baseAtk = _model._baseAtk;
        _baseDef = _model._baseDef;
        _baseExp = _model._baseEXP;
        _Lvl = lv;

        //各レベルに必要経験値を計算して配列に格納する。
        expToNextLevel = new int[_maxLevel];
        expToNextLevel[1] = _baseExp;
        for(int level=2; level < expToNextLevel.Length; level++)
        {
            expToNextLevel[level] = _baseExp * level * Mathf.Max(Mathf.FloorToInt(Mathf.Log(level, 3)), 1);
        }

        //ステータスの計算
        CalcStatus();

        _HP = _MaxHP;
        _MP = _MaxMP;
        if (_MP > _MaxMP)
        {
            _MP = _MaxMP;
        }

    }
    /// <summary>
    /// 経験値を与える。
    /// </summary>
    async public UniTask AddExp(int expToAdd)
    {
        _currentExp += expToAdd;
        _totalExp += expToAdd;
        //最大レベル以下なら
        if (_Lvl < _maxLevel)
        {
            //一時保存　レベルアップ時にどれくらい上がったかを比較するため。
            int lv = _Lvl;
            int hp = _MaxHP;
            int mp = _MaxMP;
            int atk = _Atk;
            int def = _Def;

            //最大レベル以下で現在の経験値が次のレベルに必要な経験値を上回っている場合、
            //レベルを上げる。
            while (_Lvl < _maxLevel && _currentExp > expToNextLevel[_Lvl])
            {
                _currentExp -= expToNextLevel[_Lvl];
                _Lvl++;
                //スキルの習得
                await AcquireSkill(_Lvl);
                CalcStatus();
            }
            //レベルが上がっていた場合
            int diffLv = _Lvl - lv;
            if(diffLv > 0)
            {
                Locator<IEffectManager>.I.PlayEffect(transform.position, _LvupEffect, transform.rotation).Forget();

                //上昇幅を計算してメッセージに表示する。
                string upHP = (_MaxHP - hp).ToString();
                int upMP = _MaxMP - mp;
                int upAtk = _Atk - atk;
                int upDef = _Def - def;
                var sb = new StringBuilder();
                sb.Append("レベルが"); sb.Append(diffLv); sb.AppendLine("上がった");
                sb.Append("・HP+"); sb.Append(upHP);
                sb.Append("・MP+"); sb.Append(upMP);
                sb.Append("・攻撃+"); sb.Append(upAtk);
                sb.Append("・防御+"); sb.Append(upDef);
                MessageManager.Instance.ShowMessage(sb.ToString());
            }

        }

        if(_Lvl >= _maxLevel)
        {
            _currentExp = 0;
        }
    }
    /// <summary>
    /// 習得レベルになっていた場合、スキルを習得する
    /// </summary>
    async UniTask AcquireSkill(int lv)
    {
        var skill = _levelSkills.GetSkillModels(lv);
        if(skill !=null)
        {
            var s = (Skill)Enum.ToObject(typeof(Skill), skill._ID);
            var AbstractSkill = SkillManager.Instance.GetSkillData(s);
            await SkillManager.Instance.LearnNewSkill(AbstractSkill, GameManager.Instance._player.GetSkillList());
        }
    }
    /// <summary>
    /// レベルアップに必要な経験値を返す。
    /// </summary>
    public int RequiredExp()
    {
        return expToNextLevel[_Lvl] - _currentExp;
    }
    /// <summary>
    /// トータル経験値をセットしてレベルやステータスを計算する。
    /// </summary>
    public void SetTotalExp(int expToAdd)
    {
        _currentExp += expToAdd;
        _totalExp += expToAdd;
        if (_Lvl < _maxLevel)
        {
            while (_Lvl < _maxLevel && _currentExp > expToNextLevel[_Lvl])
            {
                _currentExp -= expToNextLevel[_Lvl];
                _Lvl++;
                CalcStatus();
            }
        }
        if (_Lvl >= _maxLevel)
        {
            _currentExp = 0;
        }
    }

    /// <summary>
    /// 現在のレベルでのステータスを計算する。
    /// </summary>
        void CalcStatus()
    {
        int mh = _MaxHP;
        _MaxHP = Mathf.FloorToInt(_baseHP + Mathf.Min(60, _Lvl / 100 * _baseHP) + _Lvl)-1;
        _HP += _MaxHP- mh;

        int mm = _MaxMP;
        _MaxMP = Mathf.FloorToInt(_baseMP +_Lvl * 0.5f);
        _MP += _MaxMP -mm;
        if(_MP > _MaxMP)
        {
            _MP = _MaxMP;
        }
        _Atk = Mathf.FloorToInt(_baseAtk + Mathf.FloorToInt(_Lvl / 100 * _baseAtk) + _Lvl);
        _Def = Mathf.FloorToInt(_baseDef + Mathf.FloorToInt(_Lvl / 100 * _baseDef) + _Lvl);

    }
}
