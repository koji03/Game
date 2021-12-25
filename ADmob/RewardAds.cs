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
    /// �L�������񐔂𑝂₷
    /// </summary>
    /// <param name="num">���Z�����</param>
    public static void AddRewardCount(int num)
    {
        _rewardViewableCount += num;
        if (_rewardViewableCount < 0)
        {
            _rewardViewableCount = 0;
        }
    }
    /// <summary>
    /// �L�������񐔂��Z�b�g����B
    /// </summary>
    /// <param name="num">�Z�b�g���鐔</param>
    public static void SetRewardCount(int num)
    {
        _rewardViewableCount = num;
        if(_rewardViewableCount <0)
        {
            _rewardViewableCount = 0;
        }
    }
    /// <summary>
    /// �����{�^�����N���b�N
    /// </summary>
    public void OnReward()
    {
        if (_rewardViewableCount <= 0) return;
        if (_isClick) { return; }
        bool cangetItem = ItemManager.Instance.CanGetItem();
        if (!cangetItem) { _rewardText.text = "�A�C�e��������ȏ㏊���ł��܂���B"; return; }
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
        // �L���̊����X�e�[�^�X���Ƃɏ������`:
        if (showResult == ShowResult.Finished)
        {
            // �L�����������������烆�[�U�[�ɕ�V��^����
            GetItem();
        }
        else if (showResult == ShowResult.Skipped)
        {
            // �L�����X�L�b�v�������[�U�[�ɂ͕�V��^���Ȃ�
            _isClick = false;
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("�G���[�ɂ��L���͐������I�����܂���ł���");
            _isClick = false;
        }
    }
    bool _cangetItems = true;

    //�L��������A�A�C�e���l��
    void GetItem()
    {
        if (!_cangetItems) { return; }
        _cangetItems = false;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("�l���A�C�e��");
        sb.AppendLine(" ");
        for (int i=0; i< _rewordNum; i++)
        {
            //��������Ȃ�I���B
            bool canGetItem = ItemManager.Instance.CanGetItem();
            if (!canGetItem) { break; }

            //�����_���ɂP�A�C�e����I��
            var item = _rewordItems[Random.Range(0, _rewordItems.Length)];

            //�A�C�e�����o�b�O�ɒǉ�
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
