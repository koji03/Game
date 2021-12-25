using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// フィールド上のエネミー、ブロック、アイテムの情報を扱う。
/// </summary>
public class ObjectsData
{
    public List<Object>[] _objects;
 
    int _inedx;
    /// <summary>
    /// オブジェクトの情報
    /// </summary>
    [Serializable]
    public class Object
    {
        //オブジェクトのタイプ（エネミー、アイテム、ブロックのどれか）
        public objectType _type;
        //ゲームオブジェクト
        public GameObject _object;
        //モデル
        public Model _model;
        public Object(objectType item, GameObject obj, Model model)
        {
            _type = item;
            _object = obj;
            _model = model;
        }
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int index)
    {
        _inedx = index;
        _objects = new List<Object>[index];
    }

    /// <summary>
    /// オブジェクトのデータを追加する
    /// </summary>
    public void AddObjectData(objectType _item, int index,GameObject obj, Model model)
    {
        if (_objects[index] == null)
        {
            _objects[index] = new List<Object>(1);
        }
        var b = new Object(_item, obj, model);
        _objects[index].Add(b);
    }
    /// <summary>
    /// オブジェクトのデータを削除
    /// </summary>
    public void RemoveObjectData(int index, objectType _item,int _id)
    {
        _objects[index].RemoveAll(b => b._type == _item && b._model._ID == _id);
    }
    /// <summary>
    /// オブジェクトデータのリストを初期化
    /// </summary>
    public void InitObjectDataArray()
    {
        _objects = new List<Object>[_inedx];
    }
    /// <summary>
    /// 全てのデータを削除
    /// </summary>
    public void RemoveAllObjectBlock(objectType type)
    {
        foreach(var block in _objects)
        {
            if(block == null) { continue; }
            block.RemoveAll(b => b._type == type);
        }
    }

    public struct objectData
    {
        public int _modelID;
        public int _position;
        public int _objectType;
    }
    public struct objectDeserialize
    {
        public Model _model;
        public objectType _type;
        public int _position;
    }

    /// <summary>
    /// オブジェクトのデータをセーブする。
    /// </summary>
    /// <returns></returns>
    async public UniTask SaveObjects()
    {
        List<objectData> inf = new List<objectData>();
        for(int i=0; i< _objects.Count(); i++)
        {
            if(_objects[i] ==null)
            {
                continue;
            }
            foreach(var b in _objects[i])
            {
                objectData obj = new objectData();
                obj._modelID = b._model._ID;
                obj._position = i;
                obj._objectType = (int)b._type;
                inf.Add(obj);
            }

        }
        await Save.Seialize(Save.blockPath, inf);
    }
    /// <summary>
    /// オブジェクトのデータをロード
    /// </summary>
    async public UniTask<List<objectDeserialize>> Deserialize()
    {
        var data = await Save.Deserialize<List<objectData>>(Save.blockPath);
        List<objectDeserialize> inf2 = new List<objectDeserialize>();
        foreach (var d in data)
        {
            var model = await MasterModel.Load<Model>(d._modelID);
            objectDeserialize obj = new objectDeserialize();
            obj._model = model;
            obj._position = d._position;
            obj._type = (objectType)Enum.ToObject(typeof(objectType),d._objectType);
            inf2.Add(obj);
        }
        return inf2;
    }
}
