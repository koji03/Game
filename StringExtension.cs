namespace Extensions
{
    // 静的クラス
    public static class StringExtension
    {
        public static string NumToString(this int i)
        {
            if(i == 0) { return "-"; }
            else { return i.ToString(); }
        }
        public static string NumToString(this float i)
        {
            if (i == 0) { return "-"; }
            else { return i.ToString(); }
        }
    }

    /// <summary>
    /// 行動できる回数から武器の重さへの表記
    /// </summary>
    public static class ConsecutiveTimesExtension
    {
        public static string GetWeight(this ConsecutiveTimes times)
        {
            switch(times)
            {
                case ConsecutiveTimes.One:
                    return "重い";
                case ConsecutiveTimes.Two:
                    return "普通";
                case ConsecutiveTimes.Three:
                    return "少し軽い";
                case ConsecutiveTimes.Four:
                    return "とても軽い";
                case ConsecutiveTimes.Five:
                    return "非常に軽い";
                default:
                    return "";
            }
        }
    }
    /// <summary>
    /// 状態異常の表記
    /// </summary>
    public static class AilmentExtension
    {
        public static string GetAilmentName(this Ailment ailment)
        {
            switch (ailment)
            {
                case Ailment.Poison:
                    return "どく";
                case Ailment.Curse:
                    return "のろい";
                case Ailment.Sleep:
                    return "ねむり";
                case Ailment.BadPoison:
                    return "もうどく";
                case Ailment.Burn:
                    return "やけど";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 特殊な状態の表記
        /// </summary>
        public static string GetSpAilmentName(this SpAilment ailment)
        {
            switch (ailment)
            {
                case SpAilment.AtkBuff:
                    return "攻撃UP";
                case SpAilment.DefBuff:
                    return "防御UP";
                case SpAilment.AtkDeBuff:
                    return "攻撃DOWN";
                case SpAilment.DefDeBuff:
                    return "防御DOWN";
                case SpAilment.SpeedUp:
                    return "行動力UP";
                case SpAilment.StopTime:
                    return "時間停止";
                case SpAilment.Star:
                    return "無敵";
                case SpAilment.MPHalf:
                    return "MP半減";
                case SpAilment.Safeguard:
                    return "状態無効";
                case SpAilment.Insomnia:
                    return "不眠";
                default:
                    return null;
            }
        }
    }
}