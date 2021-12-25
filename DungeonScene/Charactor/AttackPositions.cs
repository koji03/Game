using UnityEngine;

//攻撃の範囲パターン
public enum AttackDirection
{
    Foword = 0,//前方
    TweFoword = 1,//直線2マス
    FourFoword = 2,//直線4マス
    EightFoword = 3,//直線8マス
    ThreeHorizon = 4,//前方3マス
    OneSquareAround =5,//周囲1マス
    TwoSquaresAround = 6, //周囲2マス
    Here = 7//0マス(同じ場所)
}

public static class AttackPositions
{
    /// <summary>
    /// 攻撃の方向と、攻撃者のポジションから攻撃範囲のマスの位置を配列にして返す。
    /// </summary>
    public static Vector3Int[] GetAttackPosition(AttackDirection _direction, Transform transform)
    {
        switch(_direction)
        {
            case AttackDirection.Foword:
                return Foword(transform);
            case AttackDirection.TweFoword:
                return TweFoword(transform);
            case AttackDirection.FourFoword:
                return FourFoword(transform);
            case AttackDirection.EightFoword:
                return EightFoword(transform);
            case AttackDirection.ThreeHorizon:
                return ThreeHorizon(transform);
            case AttackDirection.OneSquareAround:
                return EightSquaresAround(transform);
            case AttackDirection.TwoSquaresAround:
                return TwoSquaresAround(transform);
            case AttackDirection.Here:
                return new Vector3Int[]
                    {
                        Vector3Int.RoundToInt(transform.position)
                    };
            default:
                return ThreeHorizon(transform);
        }
    }

    public static Vector3Int[] Foword(Transform transform)
    {
        Vector3Int start = Vector3Int.RoundToInt(transform.position);
        Vector3Int pos1 = start + Vector3Int.RoundToInt(transform.forward);

        Vector3Int[] attackposition = new Vector3Int[]
        {
            pos1
        };
        return attackposition;
    }
    public static Vector3Int[] TweFoword(Transform transform)
    {
        Vector3Int start = Vector3Int.RoundToInt(transform.position);
        Vector3Int pos1 = start + Vector3Int.RoundToInt(transform.forward);
        Vector3Int pos2 = start + Vector3Int.RoundToInt(transform.forward) * 2;
        Vector3Int[] attackposition = new Vector3Int[]
        {
            pos1,pos2
        };
        return attackposition;
    }
    public static Vector3Int[] FourFoword(Transform transform)
    {
        Vector3Int start = Vector3Int.RoundToInt(transform.position);
        Vector3Int pos1 = start + Vector3Int.RoundToInt(transform.forward);
        Vector3Int pos2 = start + Vector3Int.RoundToInt(transform.forward) * 2;
        Vector3Int pos3 = start + Vector3Int.RoundToInt(transform.forward) * 3;
        Vector3Int pos4 = start + Vector3Int.RoundToInt(transform.forward) * 4;
        Vector3Int[] attackposition = new Vector3Int[]
        {
            pos1,pos2,pos3,pos4
        };
        return attackposition;
    }

    public static Vector3Int[] EightFoword(Transform transform)
    {
        Vector3Int start = Vector3Int.RoundToInt(transform.position);
        Vector3Int pos1 = start + Vector3Int.RoundToInt(transform.forward);
        Vector3Int pos2 = start + Vector3Int.RoundToInt(transform.forward) * 2;
        Vector3Int pos3 = start + Vector3Int.RoundToInt(transform.forward) * 3;
        Vector3Int pos4 = start + Vector3Int.RoundToInt(transform.forward) * 4;
        Vector3Int pos5 = start + Vector3Int.RoundToInt(transform.forward) * 5;
        Vector3Int pos6 = start + Vector3Int.RoundToInt(transform.forward) * 6;
        Vector3Int pos7 = start + Vector3Int.RoundToInt(transform.forward) * 7;
        Vector3Int pos8 = start + Vector3Int.RoundToInt(transform.forward) * 8;
        Vector3Int[] attackposition = new Vector3Int[]
        {
            pos1,pos2,pos3,pos4,pos5,pos6,pos7,pos8
        };
        return attackposition;
    }

    public static Vector3Int[] ThreeHorizon(Transform transform)
    {

        Vector3Int start = Vector3Int.RoundToInt(transform.position);
        Vector3Int pos = start + Vector3Int.RoundToInt(transform.forward);
        Vector3Int pos2 = pos + Vector3Int.RoundToInt(transform.right) * -1;
        Vector3Int pos3 = pos + Vector3Int.RoundToInt(transform.right);

        //絶対値を調べて2以上あれば1にする。
        pos2.x = start.x + Mathf.Clamp(pos2.x - start.x, -1,1);
        pos2.z = start.z + Mathf.Clamp(pos2.z - start.z, -1, 1);
        pos3.x = start.x + Mathf.Clamp(pos3.x - start.x, -1, 1);
        pos3.z = start.z + Mathf.Clamp(pos3.z - start.z, -1, 1);
        Vector3Int[] attackposition = new Vector3Int[]
        {
            pos,pos2,pos3
        };
        return attackposition;
    }
    static Vector3Int vecforward = new Vector3Int(0, 0, 1);
    public static Vector3Int[] EightSquaresAround(Transform transform)
    {
        Vector3Int start = Vector3Int.RoundToInt(transform.position);
        Vector3Int pos = start + vecforward;
        Vector3Int pos2 = start + vecforward + Vector3Int.right * -1;
        Vector3Int pos3 = start + vecforward + Vector3Int.right;
        Vector3Int pos4 = start + Vector3Int.right * -1;
        Vector3Int pos5 = start + Vector3Int.right;
        Vector3Int pos6 = start + -vecforward;
        Vector3Int pos7 = start + -vecforward + Vector3Int.right * -1;
        Vector3Int pos8 = start + -vecforward + Vector3Int.right;

        Vector3Int[] attackposition = new Vector3Int[]
        {
            pos,pos2,pos3,pos4,pos5,pos6,pos7,pos8
        };
        return attackposition;
    }

    public static Vector3Int[] TwoSquaresAround(Transform transform)
    {
        Vector3Int start = Vector3Int.RoundToInt(transform.position);
        Vector3Int pos = start + vecforward;

        Vector3Int pos2 = start + vecforward + Vector3Int.right * -1;
        Vector3Int pos3 = start + vecforward + Vector3Int.right;


        Vector3Int pos6 = start + -vecforward;
        Vector3Int pos7 = start + -vecforward + Vector3Int.right * -1;
        Vector3Int pos8 = start + -vecforward + Vector3Int.right;
        Vector3Int pos21 = start + -vecforward + Vector3Int.right * -2;
        Vector3Int pos24 = start + -vecforward + Vector3Int.right * 2;
        Vector3Int pos15 = start + -vecforward * 2 + Vector3Int.right * 2 * -1;
        Vector3Int pos16 = start + -vecforward * 2 + Vector3Int.right * 2;
        Vector3Int pos14 = start + -vecforward * 2;


        Vector3Int pos9 = start + vecforward * 2;
        Vector3Int pos10 = start + vecforward * 2 + Vector3Int.right * 2 * -1;
        Vector3Int pos11 = start + vecforward * 2 + Vector3Int.right * 2;
        Vector3Int pos12 = start + vecforward * 2 * -1;
        Vector3Int pos17 = start + vecforward * 2 + Vector3Int.right;
        Vector3Int pos18 = start + vecforward * 2 + Vector3Int.right * -1;

    

        Vector3Int pos5 = start + Vector3Int.right;
        Vector3Int pos4 = start + Vector3Int.right * -1;
        Vector3Int pos19 = start + Vector3Int.right * -2;



        Vector3Int pos13 = start + vecforward * -2 + Vector3Int.right;
        Vector3Int pos25 = start + vecforward * -2 + Vector3Int.right * -1;

        Vector3Int pos20 = start + vecforward + Vector3Int.right * -2;

        Vector3Int pos22 = start + Vector3Int.right * 2;
        Vector3Int pos23 = start + vecforward + Vector3Int.right * 2;


        Vector3Int[] attackposition = new Vector3Int[]
        {
            pos,pos2,pos3,pos4,pos5,pos6,pos7,pos8,
            pos9,pos10,pos11,pos12,pos13,pos14,pos15,
            pos16,pos17,pos18,pos19,pos20,pos21,pos22,pos23,
            pos24,pos25
        };
        return attackposition;
    }
}
