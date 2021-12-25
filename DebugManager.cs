using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

//System.IOを忘れずに
using System.IO;
using Cysharp.Threading.Tasks;

public class DebugManager : MonoBehaviour
{
    public bool _isDebugMode;

    // Start is called before the first frame update
    [SerializeField] GameObject _clickArea,buttons,canvas;
    public EventTrigger _EventTrigger;

    [SerializeField]GameObject[] objects;
    
    //StopCoroutineのためにCoroutineで宣言しておく
    Coroutine PressCorutine;
    bool isPressDown = false;
    float PressTime = 1f;
    void Awake()
    {
        HideObjects();
        Debug.unityLogger.logEnabled = _isDebugMode;
        //でバックモードでないなら
        if (!_isDebugMode)
        {
            _clickArea.SetActive(false);
            Destroy(gameObject);
            return;
        }

        //PointerDownイベントの登録
        EventTrigger.Entry pressdown = new EventTrigger.Entry();
        pressdown.eventID = EventTriggerType.PointerDown;
        pressdown.callback.AddListener((data) => PointerDown());
        _EventTrigger.triggers.Add(pressdown);

        //PointerUpイベントの登録
        EventTrigger.Entry pressup = new EventTrigger.Entry();
        pressup.eventID = EventTriggerType.PointerUp;
        pressup.callback.AddListener((data) => PointerUp());
        _EventTrigger.triggers.Add(pressup);

        canvas.transform.Find("LogPanel/Text").GetComponent<CatchLog>().Init();

    }
    void HideObjects()
    {
        buttons.SetActive(false);
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(false);
        }
    }
    //EventTriggerのPointerDownイベントに登録する処理
    void PointerDown()
    {
        Debug.Log("Press Start");
        //連続でタップした時に長押しにならないよう前のCoroutineを止める
        if (PressCorutine != null)
        {
            StopCoroutine(PressCorutine);
        }
        //StopCoroutineで止められるように予め宣言したCoroutineに代入
        PressCorutine = StartCoroutine(TimeForPointerDown());
    }

    //長押しコルーチン
    IEnumerator TimeForPointerDown()
    {
        //プレス開始
        isPressDown = true;

        //待機時間
        yield return new WaitForSeconds(PressTime);

        //押されたままなら長押しの挙動
        if (isPressDown)
        {
            Debug.Log("Long Press Done");

            //お好みの長押し時の挙動をここに書く
            buttons.SetActive(true);

        }
        //プレス処理終了
        isPressDown = false;
    }

    //EventTriggerのPointerUpイベントに登録する処理
    void PointerUp()
    {
        if (isPressDown)
        {
            Debug.Log("Short Press Done");
            isPressDown = false;

            //お好みの短押し時の挙動をここに書く(無い場合は書かなくても良い)
            HideObjects();
        }
        Debug.Log("Press End");
    }


    public void OnReset()
    {
        Destroy(GameObject.Find("GameManager"));
        SceneManager.LoadScene(0);
    }
    public void OnLog()
    {
        GameObject parent = GameObject.Find("DebugCanvas");
        GameObject panel = parent.transform.Find("LogPanel").gameObject;
        panel.SetActive(true);
        buttons.SetActive(false);
    }

    public void DeleteData()
    {
        Save.Delete("");
    }
    public void AllItem()
    {
        var item = Resources.LoadAll<ItemModel>("MasterItemModel/");
        var ids1 = item.Select(x => x._ID).ToArray();
        var ids2 = item.Select(x => x._ID).ToArray();
        var ids3 = item.Select(x => x._ID).ToArray();
        var ids4 = item.Select(x => x._ID).ToArray();
        var ids5 = item.Select(x => x._ID).ToArray();

        int[] mergedArray = ids1.Concat(ids2).ToArray();
        int[] mergedArray2 = mergedArray.Concat(ids3).ToArray();
        int[] mergedArray3 = mergedArray2.Concat(ids4).ToArray();
        int[] mergedArray4 = mergedArray3.Concat(ids5).ToArray();
    }

}
