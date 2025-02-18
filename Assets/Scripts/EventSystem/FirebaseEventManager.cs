using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public static class FirebaseEventManager
{
    private static bool isFirebaseReady = false;

    static FirebaseEventManager()
    {
        // Firebase başlatılıyor
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                isFirebaseReady = true;
                Debug.Log("🔥 Firebase Event Sistemi Başlatıldı!");
            }
            else
            {
                Debug.LogError("❌ Firebase başlatılamadı: " + task.Result);
            }
        });
    }

    // ✅ Firebase event gönderme fonksiyonu
    private static void LogEvent(string eventName, params Parameter[] parameters)
    {
        if (!isFirebaseReady)
        {
            Debug.LogWarning("⚠️ Firebase hazır değil, event gönderilemedi: " + eventName);
            return;
        }

        FirebaseAnalytics.LogEvent(eventName, parameters);
        Debug.Log("📊 Firebase Event Gönderildi: " + eventName);
    }

    // ✅ Level Başlangıç Eventi
    public static void LogLevelStart(int levelNumber)
    {
        LogEvent("level_start", new Parameter("level_number", levelNumber));
    }

    // ✅ Level Tamamlama Eventi
    public static void LogLevelComplete(int levelNumber, int earnedCoins)
    {
        LogEvent("level_complete", new Parameter("level_number", levelNumber), new Parameter("earned_coins", earnedCoins));
    }

    // ✅ Reklam İzleme Eventi (Geçiş & Ödüllü)
    public static void LogAdWatched(string adType)
    {
        LogEvent("ad_watched", new Parameter("ad_type", adType));
    }

    // ✅ Ödül Reklamından Kazanç Eventi
    public static void LogRewardClaimed(string rewardType)
    {
        LogEvent("reward_claimed", new Parameter("reward_type", rewardType));
    }

    // ✅ Satın Alma Eventi
    public static void LogItemPurchased(string itemName, int price)
    {
        LogEvent("item_purchased", new Parameter("item_name", itemName), new Parameter("price", price));
    }

    // ✅ No Ads Satın Alma Eventi
    public static void LogNoAdsPurchased(int price)
    {
        LogEvent("no_ads_purchased", new Parameter("price", price));
    }

    // ✅ Gezegen Seçme Eventi
    public static void LogPlanetSelected(string planetName)
    {
        LogEvent("planet_selected", new Parameter("planet_name", planetName));
    }

    // ✅ Dekorasyon Yerleştirme Eventi
    public static void LogDecorationPlaced(string decorationName)
    {
        LogEvent("decoration_placed", new Parameter("decoration_name", decorationName));
    }
}
