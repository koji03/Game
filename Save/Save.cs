using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class Progress
{
    public int _stageLevel;
    public List<int> _blockPosition;
    public List<int> _enemyID;
    public List<int> _enemyPosition;
    public List<int> _itemID;
    public List<int> _itemsPosition;
    public Vector3 _playerNextPosition;
}

public static class Save
{
    //パス
    public const string heldItemDataPath = "HeldItemData";
    public const string playerSkillDataPath = "SkillData";
    public const string playerEquipDataPath = "EquipData";
    public const string blockPath = "BlockData";
    public const string stagePath = "StageData";
    public const string playerStagePath = "PlayerStage";
    public const string progressDataPath = "ProgressData";
    public const string rewordItems = "RewordItemData";
    public const string setting = "SaveSetting";


    static Queue<string> pathes = new Queue<string>();


    // シリアライズ
    async public static UniTask<T> Seialize<T>(string filename, T data)
    {
        var combinedPath = Path.Combine(GetInternalStoragePath(), filename);
        var queuePath = CreateRandomPath();
        pathes.Enqueue(queuePath);

        //別のデータをシリアライズ/デシリアライズをしているときは待機をする。
        while(IsFileLocked(queuePath))
        {
            await UniTask.Yield();
        }
        //データの保存
        using (var stream = new FileStream(combinedPath, FileMode.Create))
        {
            var serializer = new XmlSerializer(typeof(T));
            await UniTask.Run(()=>serializer.Serialize(stream, data));
        }
        pathes.Dequeue();
        return data;
    }

    // デシリアライズ
    async public static UniTask<T> Deserialize<T>(string filename)
    {
        try
        {
            //パスを作成
            var combinedPath = Path.Combine(GetInternalStoragePath(), filename);
            var queuePath = CreateRandomPath();
            pathes.Enqueue(queuePath);

            //別のデータをシリアライズ/デシリアライズをしているときは待機をする。
            while (IsFileLocked(queuePath))
            {
                await UniTask.Yield();
            }
            using (var stream = new FileStream(combinedPath, FileMode.OpenOrCreate))
            {
                var serializer = new XmlSerializer(typeof(T));
                var data =  await UniTask.Run(()=>(T)serializer.Deserialize(stream));
                pathes.Dequeue();
                return data;

            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            pathes.Dequeue();
            return (T)default;
        }

    }

    static bool IsFileLocked(string queuePath)
    {
        if (pathes.Peek() == queuePath)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    static string CreateRandomPath()
    {
        string random = Guid.NewGuid().ToString("N");
        return random;
    }
    static string GetInternalStoragePath()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var getFilesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir"))
        {
            string secureDataPath = getFilesDir.Call<string>("getCanonicalPath");
            return secureDataPath;
        }
#else
        return Application.persistentDataPath;
#endif
    }

    public static void Delete(string targetDirectoryPath)
    {
        var combinedPath = Path.Combine(GetInternalStoragePath(), targetDirectoryPath);

        if (!Directory.Exists(combinedPath))
        {
            return;
        }

        //ディレクトリ以外の全ファイルを削除
        string[] filePaths = Directory.GetFiles(combinedPath);
        foreach (string filePath in filePaths)
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        //ディレクトリの中のディレクトリも再帰的に削除
        string[] directoryPaths = Directory.GetDirectories(combinedPath);
        foreach (string directoryPath in directoryPaths)
        {
            Delete(directoryPath);
        }

        //中が空になったらディレクトリ自身も削除
        Directory.Delete(combinedPath, false);
    }

}
