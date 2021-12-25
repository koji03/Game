using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class SaveHeldItemData 
{
    public class SaveHeldItem
    {
        public int ID;
        public int Count;
    }
    //新規追加
    async public static UniTask SaveHeldItems(int[] ids)
    {
        //所持アイテムのリストを作成
        List<SaveHeldItem> currentHeldItems = new List<SaveHeldItem>();
        foreach(var id in ids)
        {
            //idが0以下ならアイテムではない
            if (id<=0)
            {
                continue;
            }
            //同じIDがあればcountを増やす
            var list = currentHeldItems.Find(x => x.ID == id);
            if (list !=null)
            {
                list.Count++;
            }else
            {
                //同じidがなければ新規
                var heldItem = new SaveHeldItem();
                heldItem.ID = id;
                heldItem.Count = 1;
                currentHeldItems.Add(heldItem);
            }
        }
        //保存
        await Save.Seialize(Save.heldItemDataPath, currentHeldItems);
    }
    async public static UniTask SaveHeldItems(List<SaveHeldItem> items)
    {
        //id0以下があれば削除
        items.RemoveAll(x => x.ID <= 0);
        await Save.Seialize(Save.heldItemDataPath, items);
    }

    async public static UniTask<List<SaveHeldItem>> Deserialize()
    {
        return await Save.Deserialize<List<SaveHeldItem>>(Save.heldItemDataPath);
    }
}
