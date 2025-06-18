using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    private System.Action onRewardedComplete;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
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

        interstitial.OnAdLoaded += (sender, args) =>
        {
            Debug.Log("Interstitial reklam başarıyla yüklendi!");
        };

        interstitial.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.LogError($"Interstitial reklam yüklenemedi: {args.LoadAdError.GetMessage()}");
        };

        interstitial.OnAdClosed += (sender, args) =>
        {
            Debug.Log("Interstitial reklam kapatıldı. Yeniden yükleniyor...");
            RequestInterstitial();
        };
    }

    public void ShowInterstitial(System.Action onAdClosed)
    {
        if (interstitial != null && interstitial.IsLoaded())
        {
            Debug.Log("Interstitial reklam gösteriliyor...");

            // Geçici olay dinleyicisi
            System.EventHandler<System.EventArgs> handler = null;
            handler = (sender, args) =>
            {
                interstitial.OnAdClosed -= handler; // Tek seferlik çalışsın
                onAdClosed?.Invoke();
                RequestInterstitial(); // Reklamı tekrar yükle
            };

            interstitial.OnAdClosed += handler;
            interstitial.Show();
        }
        else
        {
            Debug.LogWarning("⛔ Interstitial reklam hazır değil, panel direkt açılıyor");
            onAdClosed?.Invoke(); // Reklam yoksa paneli yine de aç
        }
    }

    // Rewarded Reklam Yükleme
    void RequestRewarded()
    {
        string adUnitId = "ca-app-pub-4386310818185459/7559445331";
        AdRequest request = new AdRequest.Builder().Build();

        rewardedAd = new RewardedAd(adUnitId);
        rewardedAd.LoadAd(request);

        rewardedAd.OnAdLoaded += (sender, args) =>
        {
            Debug.Log("Rewarded reklam başarıyla yüklendi!");
        };

        rewardedAd.OnAdFailedToLoad += (sender, args) =>
        {
            Debug.LogError($"Rewarded reklam yüklenemedi: {args.LoadAdError.GetMessage()}");
        };

        rewardedAd.OnAdClosed += (sender, args) =>
        {
            Debug.Log("Rewarded reklam kapatıldı. Yeniden yükleniyor...");
            RequestRewarded();
        };

        rewardedAd.OnUserEarnedReward += (sender, reward) =>
        {
            Debug.Log($"Ödül kazanıldı: {reward.Amount} {reward.Type}");
            onRewardedComplete?.Invoke();
            onRewardedComplete = null; // güvenlik için sıfırla
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

    public void ShowRewardedForFuel(System.Action onComplete)
    {
        if (rewardedAd != null && rewardedAd.IsLoaded())
        {
            onRewardedComplete = onComplete;
            rewardedAd.Show();
        }
        else
        {
            Debug.LogWarning("⚠️ Rewarded reklam hazır değil!");
        }
    }
}
