using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour
{
    static private string _gameId;
    static private string _placementId = "Interstitial_Android";

    public static bool isTest = false;
    void Awake()
    {
        DontDestroyOnLoad(this);
        #if UNITY_IOS
               _gameId = "4476588";
        #elif UNITY_ANDROID
                _gameId = "4476589";
        #endif
        Advertisement.Initialize(_gameId, testMode: isTest);
    }
    //広告を表示する。
    async public static UniTask ShowUnityInterstitialAds()
    {
        while (!Advertisement.IsReady(_placementId))
        {
            await UniTask.Yield();
        }
         Advertisement.Show(_placementId);
    }

}