public class ItemCellEquip : ICellItem
{
    public int _strength;
    public ItemCellEquip(EquipModel model, int count = 1)
    {
        _strength = model._strength;
        _ItemCount = count;
        _ItemModel = model;
    }
}
