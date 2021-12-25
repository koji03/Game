using System.Collections.Generic;
using System.Linq;

public class Sort
{
    public enum SortType{
        ID,
        Name,
        Item,
        Book,
        Equip,
        Strength,
    }
    /// <summary>
    /// アイテムの表示順番を変える。
    /// </summary>
    public List<ICellData> SortData(List<ICellData> data, SortType sortType)
    {

        switch (sortType)
        {
            case SortType.Item:
                var itemcell = data.OfType<ICellItem>().ToList();
                return itemcell.Where(x => x._ItemModel._ID <= Setting.BookID[0]).OfType<ICellData>().ToList();
            case SortType.Equip:
                var Equip = data.OfType<ICellItem>().ToList();
                return Equip.Where(x => x._ItemModel._ID > Setting.WeaponID[0]).OfType<ICellData>().ToList();
            case SortType.Book:
                var itemcell2 = data.OfType<ItemCellBase>().ToList();
                return itemcell2.Where(x => x._ItemModel._ID > Setting.BookID[0] && x._ItemModel._ID < Setting.WeaponID[0]).OfType<ICellData>().ToList();
            case SortType.ID:
                var itemcell3 = data.OfType<ICellItem>().ToList();
                return itemcell3.OrderBy(x => x._ItemModel._ID).OfType<ICellData>().ToList();
            case SortType.Name:
                var itemcell5 = data.OfType<ICellItem>().ToList();
                return itemcell5.OrderBy(x => x._ItemModel._name).OfType<ICellData>().ToList();
            case SortType.Strength:
                    var itemcell4 = data.OfType<ItemCellEquip>().ToList();
                    return itemcell4.OrderBy(x => x._strength).OfType<ICellData>().ToList();
            default:
                return null;
        }
    }

}
