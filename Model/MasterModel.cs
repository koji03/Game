using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MasterModel
{
   /// <summary>
   /// resoucesからModelをロードする。
   /// </summary>
   async public static UniTask<T> Load<T>(int itemId) where T : class
    {
        T model = await Resources.LoadAsync("MasterItemModel/" + itemId) as T;
        if (model == null)
        {
            Debug.Log("Modelが見つかりません" + typeof(T) + itemId);
        }
        return model;
    }
    /// <summary>
    /// resoucesから選択した種類のModelを全てロードする。
    /// </summary>
    async public static UniTask<List<T>> LoadAllModelsOfSelectedType<T>(int[] ids) where T : class
    {
        List<T> Models = new List<T>();
        foreach(var i in ids)
        {
            var model = await Resources.LoadAsync("MasterItemModel/" + i) as T;
            Models.Add(model);
        }
        return Models;
    }
}
