using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 버튼을 사용하기 위해 필요
using GoogleMobileAds.Api;

public class GoogleMobileAdsDemoScript : MonoBehaviour
{
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-9112055371825372/5240014774";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
    private string _adUnitId = "unused";
#endif

    private RewardedAd _rewardedAd;

    // 버튼 변수
    [SerializeField] private Button showAdButton;
    [SerializeField] private int rewardGem;

    // Start is called before the first frame update
    void Start()
    {
        showAdButton = this.gameObject.GetComponent<Button>();
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // SDK 초기화 완료 후 작업
        });

        // 광고 로드
        LoadRewardedAd();

        // 버튼 클릭 이벤트에 광고 표시 메서드 연결
        showAdButton.onClick.AddListener(ShowRewardedAd);
    }

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd()
    {
        // 기존 광고 정리
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("보상형 광고 로드 중...");

        // 광고 요청 생성
        var adRequest = new AdRequest();

        // 광고 요청을 통해 광고 로드
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // 광고 로드 실패 시 처리
                if (error != null || ad == null)
                {
                    Debug.LogError("보상형 광고 로드 실패: " + error);
                    return;
                }

                Debug.Log("보상형 광고 로드 성공!");

                _rewardedAd = ad;

                // 이벤트 핸들러 등록
                RegisterEventHandlers(_rewardedAd);

                // 광고 로드 후 리로드 핸들러 등록
                RegisterReloadHandler(_rewardedAd);
            });
    }

    // 광고 표시
    public void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                InGameScene.Managers.Game.UpdateGem(rewardGem);
                StaticManager.UI.AlertUI.OpenAlertUI("광고 보상으로", "Gem " + rewardGem + "이(가) 지급되었습니다.");
                //UpdateUI();
                // 보상 지급 로직
                Debug.Log("지급되었습니다");
            });
        }
        else
        {
            Debug.Log("보상형 광고가 로드되지 않았습니다.");
        }
    }

    // 광고 이벤트 핸들러 등록
    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("광고 수익 발생: {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("광고 노출이 기록되었습니다.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("광고가 클릭되었습니다.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("광고 전체 화면이 열렸습니다.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("광고 전체 화면이 닫혔습니다.");
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("광고 전체 화면 열기 실패: " + error);
        };
    }

    // 광고 리로드 핸들러 등록
    private void RegisterReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("광고가 닫혔습니다. 새로운 광고를 로드합니다.");
            LoadRewardedAd(); // 광고가 닫힌 후 새로운 광고 로드
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("광고 열기 실패: " + error + ". 새로운 광고를 로드합니다.");
            LoadRewardedAd(); // 광고 열기 실패 시 새로운 광고 로드
        };
    }
}
