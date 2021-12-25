using Cysharp.Threading.Tasks;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardAds : MonoBehaviour, IUnityAdsListener
{
    static private string placementId = "Rewarded_Android";

    bool _isClick = false;

    [SerializeField] ItemModel[] _rewordItems;
    [SerializeField] int _rewordNum = 1;
    [SerializeField] TextMeshProUGUI _rewardText,_rewardCount;
    [SerializeField] public static int _getLevel { get;private set; } = 5;
    public static int _rewardViewableCount { get;private set; } = 0;

    /// <summary>
    /// 広告視聴回数を増やす
    /// </summary>
    /// <param name="num">加算する回数</param>
    public static void AddRewardCount(int num)
    {
        _rewardViewableCount += num;
        if (_rewardViewableCount < 0)
        {
            _rewardViewableCount = 0;
        }
    }
    /// <summary>
    /// 広告視聴回数をセットする。
    /// </summary>
    /// <param name="num">セットする数</param>
    public static void SetRewardCount(int num)
    {
        _rewardViewableCount = num;
        if(_rewardViewableCount <0)
        {
            _rewardViewableCount = 0;
        }
    }
    /// <summary>
    /// 視聴ボタンをクリック
    /// </summary>
    public void OnReward()
    {
        if (_rewardViewableCount <= 0) return;
        if (_isClick) { return; }
        bool cangetItem = ItemManager.Instance.CanGetItem();
        if (!cangetItem) { _rewardText.text = "アイテムをこれ以上所持できません。"; return; }
        _isClick = true;
        ShowRewardedVideo().Forget();
    }
    static private string _gameId;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
#if UNITY_IOS
               _gameId = "4476588";
#elif UNITY_ANDROID
        _gameId = "4476589";
#endif
        Advertisement.AddListener(this);
        Advertisement.Initialize(_gameId, testMode: UnityAds.isTest);
    }

    public void OnClose()
    {
        if (_isClick) { return; }
        gameObject.SetActive(false);
    }
    public void ShowRewordPanel()
    {
        gameObject.SetActive(true);
    }
    async public UniTask ShowRewardedVideo()
    {
        while (!Advertisement.IsReady(placementId))
        {
            await UniTask.Yield();
        }
        Advertisement.Show(placementId);
    }
    private void OnEnable()
    {
        _rewardCount.text = _rewardViewableCount.ToString();
        _rewardText.text = "";
    }

    public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult)
    {
        Debug.Log("OnUnityAdsDidFinish");
        // 広告の完了ステータスごとに処理を定義:
        if (showResult == ShowResult.Finished)
        {
            // 広告視聴が完了したらユーザーに報酬を与える
            GetItem();
        }
        else if (showResult == ShowResult.Skipped)
        {
            // 広告をスキップしたユーザーには報酬を与えない
            _isClick = false;
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("エラーにより広告は正しく終了しませんでした");
            _isClick = false;
        }
    }
    bool _cangetItems = true;

    //広告視聴後、アイテム獲得
    void GetItem()
    {
        if (!_cangetItems) { return; }
        _cangetItems = false;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("獲得アイテム");
        sb.AppendLine(" ");
        for (int i=0; i< _rewordNum; i++)
        {
            //所持上限なら終わる。
            bool canGetItem = ItemManager.Instance.CanGetItem();
            if (!canGetItem) { break; }

            //ランダムに１つアイテムを選ぶ
            var item = _rewordItems[Random.Range(0, _rewordItems.Length)];

            //アイテムをバッグに追加
            ItemManager.Instance.AddItem(item);
            sb.AppendLine(item._name);
        }
        _rewardViewableCount--;
        GameManager.Instance.SetRewordCount(_rewardViewableCount);
        _rewardCount.text = _rewardViewableCount.ToString();
        _rewardText.text = sb.ToString();
        _cangetItems = true;
        _isClick = false;
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("OnUnityAdsReady");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("OnUnityAdsDidError");
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("OnUnityAdsDidStart");
    }
}
