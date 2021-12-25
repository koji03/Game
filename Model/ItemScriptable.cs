using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Model : ScriptableObject
{
    public int _ID;
    public string _description;
    public string _name;
    public GameObject _prefab;
}
public abstract class ItemModel : Model
{
    public ItemType itemType;
    public EffectData _effect;
    public abstract UniTask<bool> UseItem();
}

public abstract class EquipModel : ItemModel
{
    public int _strength;
}


public abstract class BlockModel : Model
{
    public int _HP;
    public abstract UniTask ActionAfterDestroyed(Vector3 position);
}

public abstract class CharactorModel : Model
{
    public int _baseHP;
    public int _baseMP;
    public int _baseAtk;
    public int _baseDef;
    public int _baseEXP = 1000;
}


public abstract class TimeModel : Model
{
    public byte _count;
    public abstract UniTask Play(Transform blockT);
    public abstract UniTask DestroyAction(Transform blockT);
}