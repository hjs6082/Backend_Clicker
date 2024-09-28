using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ��ư�� ����ϱ� ���� �ʿ�
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

    // ��ư ����
    [SerializeField] private Button showAdButton;
    [SerializeField] private int rewardGem;

    // Start is called before the first frame update
    void Start()
    {
        showAdButton = this.gameObject.GetComponent<Button>();
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // SDK �ʱ�ȭ �Ϸ� �� �۾�
        });

        // ���� �ε�
        LoadRewardedAd();

        // ��ư Ŭ�� �̺�Ʈ�� ���� ǥ�� �޼��� ����
        showAdButton.onClick.AddListener(ShowRewardedAd);
    }

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd()
    {
        // ���� ���� ����
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("������ ���� �ε� ��...");

        // ���� ��û ����
        var adRequest = new AdRequest();

        // ���� ��û�� ���� ���� �ε�
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // ���� �ε� ���� �� ó��
                if (error != null || ad == null)
                {
                    Debug.LogError("������ ���� �ε� ����: " + error);
                    return;
                }

                Debug.Log("������ ���� �ε� ����!");

                _rewardedAd = ad;

                // �̺�Ʈ �ڵ鷯 ���
                RegisterEventHandlers(_rewardedAd);

                // ���� �ε� �� ���ε� �ڵ鷯 ���
                RegisterReloadHandler(_rewardedAd);
            });
    }

    // ���� ǥ��
    public void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                InGameScene.Managers.Game.UpdateGem(rewardGem);
                StaticManager.UI.AlertUI.OpenAlertUI("���� ��������", "Gem " + rewardGem + "��(��) ���޵Ǿ����ϴ�.");
                //UpdateUI();
                // ���� ���� ����
                Debug.Log("���޵Ǿ����ϴ�");
            });
        }
        else
        {
            Debug.Log("������ ���� �ε���� �ʾҽ��ϴ�.");
        }
    }

    // ���� �̺�Ʈ �ڵ鷯 ���
    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("���� ���� �߻�: {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("���� ������ ��ϵǾ����ϴ�.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("���� Ŭ���Ǿ����ϴ�.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("���� ��ü ȭ���� ���Ƚ��ϴ�.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("���� ��ü ȭ���� �������ϴ�.");
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("���� ��ü ȭ�� ���� ����: " + error);
        };
    }

    // ���� ���ε� �ڵ鷯 ���
    private void RegisterReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("���� �������ϴ�. ���ο� ���� �ε��մϴ�.");
            LoadRewardedAd(); // ���� ���� �� ���ο� ���� �ε�
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("���� ���� ����: " + error + ". ���ο� ���� �ε��մϴ�.");
            LoadRewardedAd(); // ���� ���� ���� �� ���ο� ���� �ε�
        };
    }
}
