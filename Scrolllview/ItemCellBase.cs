
public class ItemCellBase : ICellItem
{
    public ItemCellBase(ItemModel model,int count = 1)
    {
        _ItemCount = count;
        _ItemModel = model;
    }
}
