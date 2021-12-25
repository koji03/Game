using UnityEngine;

public class CharactorEquip : MonoBehaviour
{

    public GameObject[] _weaponR;//全ての武器
    public GameObject[] _armorL;//全ての盾
    [SerializeField] WeaponModel _weapon;
    [SerializeField] ArmorModel _armor;

    /// <summary>
    /// 武器のモデルをセット
    /// </summary>
    public void SetWeapon(ItemModel weapon)
    {
        _weapon = (WeaponModel)weapon;
    }
    /// <summary>
    /// 武器のモデルを取得
    /// </summary>
    public ItemModel GetWeapon()
    {
        return _weapon;
    }
    /// <summary>
    /// 盾のモデルを取得
    /// </summary>
    public ItemModel GetArmor()
    {
        return _armor;
    }
    /// <summary>
    /// 盾のモデルをセット
    /// </summary>
    public void SetArmor(ItemModel armor)
    {
        _armor = (ArmorModel)armor;
    }

    /// <summary>
    /// 武器の行動可能回数を取得
    /// </summary>
    public ConsecutiveTimes GetTimes()
    {
        return _weapon._times;
    }
    /// <summary>
    /// 武器の強さを取得
    /// </summary>
    public int GetWeaponStrength()
    {
        if(_weapon !=null)
        {
            return _weapon._strength;
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 盾の強さを取得
    /// </summary>
    public int GetArmorStrength()
    {
        if (_armor != null)
        {
            return _armor._strength;
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 武器の名前
    /// </summary>
    public string GetWeaponName()
    {
        if (_weapon != null)
        {
            return _weapon._name;
        }
        else
        {
            return "無し";
        }
    }
    /// <summary>
    /// 盾の名前
    /// </summary>
    public string GetArmorName()
    {
        if (_armor != null)
        {
            return _armor._name;
        }
        else
        {
            return "無し";
        }
    }
    /// <summary>
    /// 武器の攻撃範囲を取得
    /// </summary>
    public AttackDirection GetWeaponDirection()
    {
        if (_weapon != null)
        {
            return _weapon._attackDirection;
        }
        else
        {
            return AttackDirection.Foword;
        }
    }
    /// <summary>
    /// 武器が貫通可能かどうかを取得
    /// </summary>
    public bool GetisPenetrating()
    {
        if (_weapon != null)
        {
            return _weapon.isPenetrating;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 武器の命中率を取得
    /// </summary>
    public int GetHitRate()
    {
        if (_weapon != null)
        {
            return _weapon._hitrate;
        }
        else
        {
            return 100;
        }
    }
}
