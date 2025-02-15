using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class AdManager : MonoBehaviour
{
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;

    void Start()
    {
        MobileAds.Initialize(initStatus => {
            Debug.Log("AdMob Başlatıldı!");
        });

        // Test cihazı ID'sini konsola yazdır
        string testDeviceId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Test Device ID: " + testDeviceId);

        // Test cihazı olarak tanımlama
        RequestConfiguration requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(new List<string>() { testDeviceId })
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Reklamları yükle
        RequestInterstitial();
        RequestRewarded();
    }

    // Interstitial Reklam Yükleme
    void RequestInterstitial()
    {
        string adUnitId = "ca-app-pub-4386310818185459/1185608672";
        AdRequest request = new AdRequest.Builder().Build();

        interstitial = new InterstitialAd(adUnitId);
        interstitial.LoadAd(request);

        // Reklam yüklendiğinde
        interstitial.OnAdLoaded += (sender, args) =>
        {
            Debug.Log("Interstitial reklam başarıyla yüklendi!");
        };

        // Reklam yüklenemezse
        interstitial.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.LogError($"Interstitial reklam yüklenemedi: {args.LoadAdError.GetMessage()}");
        };

        // Reklam kapatıldığında tekrar yükle
        interstitial.OnAdClosed += (sender, args) =>
        {
            Debug.Log("Interstitial reklam kapatıldı. Yeniden yükleniyor...");
            RequestInterstitial();
        };
    }

    public void ShowInterstitial()
    {
        if (interstitial != null && interstitial.IsLoaded())
        {
            Debug.Log("Interstitial reklam gösteriliyor...");
            interstitial.Show();
        }
        else
        {
            Debug.LogWarning("Interstitial reklam hazır değil!");
        }
    }

    // Rewarded Reklam Yükleme
    void RequestRewarded()
    {
        string adUnitId = "ca-app-pub-4386310818185459/7559445331";
        AdRequest request = new AdRequest.Builder().Build();

        rewardedAd = new RewardedAd(adUnitId);
        rewardedAd.LoadAd(request);

        // Reklam yüklendiğinde
        rewardedAd.OnAdLoaded += (sender, args) =>
        {
            Debug.Log("Rewarded reklam başarıyla yüklendi!");
        };

        // Reklam yüklenemezse
        rewardedAd.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.LogError($"Rewarded reklam yüklenemedi: {args.LoadAdError.GetMessage()}");
        };

        // Reklam kapatıldığında tekrar yükle
        rewardedAd.OnAdClosed += (sender, args) =>
        {
            Debug.Log("Rewarded reklam kapatıldı. Yeniden yükleniyor...");
            RequestRewarded();
        };

        // Ödül kazanıldığında
        rewardedAd.OnUserEarnedReward += (sender, reward) =>
        {
            Debug.Log($"Ödül kazanıldı: {reward.Amount} {reward.Type}");
        };
    }

    public void ShowRewarded()
    {
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            Debug.Log("Rewarded reklam gösteriliyor...");
            rewardedAd.Show();
        }
        else
        {
            Debug.LogWarning("Rewarded reklam hazır değil!");
        }
    }
}
