public class Setting
{
    public static bool IsTutorial = false;
    public static byte NumCanHaveHeldItems = 20;
    //IDとModelの区分
    public static int[] WeaponID =new int[] {999,2000 };
    public static int[] ArmorID = new int[] { 1999, 3000 };
    public static int[] BookID = new int[] { 499, 1000 };
    public static int[] SkillID = new int[] { 19999, 25000 };


    /// <summary>
    /// アイテム上限を更新する。
    /// </summary>
    public static void UpdateNumMaxItemsFromType(StageType type)
    {
        switch (type)
        {
            case StageType.Forest:
                NumCanHaveHeldItems = 20;
                break;
            case StageType.Deseart:
                NumCanHaveHeldItems = 25;
                break;
            case StageType.Winter:
                NumCanHaveHeldItems = 30;
                break;
            case StageType.Dungeon:
                NumCanHaveHeldItems = 35;
                break;
            case StageType.Last:
                NumCanHaveHeldItems = 40;
                break;
        }
    }

    /// <summary>
    /// IDが装備のIDかどうか
    /// </summary>
    public static bool IsEquipIDs(int id)
    {
        return (id > WeaponID[0] && id < ArmorID[1]);
    }
    /// <summary>
    /// IDが秘伝書かどうか
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsSkillBookIDs(int id)
    {
        return (id > BookID[0] && id < BookID[1]);
    }
    /// <summary>
    /// IDが盾かどうか
    /// </summary>
    public static bool IsArmorIDs(int id)
    {
        return (id > ArmorID[0] && id < ArmorID[1]);
    }
    /// <summary>
    /// IDが武器かどうか
    /// </summary>
    public static bool IsWeaponIDs(int id)
    {
        return (id > WeaponID[0] && id < WeaponID[1]);
    }
    /// <summary>
    /// IDがスキルかどうか
    /// </summary>
    public static bool IsSkillIDs(int id)
    {
        return (id > SkillID[0] && id < SkillID[1]);
    }
}
