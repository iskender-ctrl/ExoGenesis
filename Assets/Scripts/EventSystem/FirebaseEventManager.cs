using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public static class FirebaseEventManager
{
    private static bool isFirebaseReady = false;

    /*────────────────────────────  INIT  ────────────────────────────*/
    static FirebaseEventManager()
    {
        FirebaseApp.CheckAndFixDependenciesAsync()
                   .ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // FirebaseApp app = FirebaseApp.DefaultInstance;  // İsterseniz hâlâ tutabilirsiniz
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

    /*──────────────────────────  HELPER  ───────────────────────────*/
    private static void LogEvent(string name, params Parameter[] param)
    {
        if (!isFirebaseReady)
        {
            Debug.LogWarning($"⚠️ Firebase hazır değil → {name}");
            return;
        }
        FirebaseAnalytics.LogEvent(name, param);
#if UNITY_EDITOR
        Debug.Log($"📊 Firebase Event: {name}");
#endif
    }

    /*────────────────────  MEVCUT EVENTLER  ────────────────────*/
    public static void LogLevelStart(int level) =>
        LogEvent("level_start", new Parameter("level_number", level));

    public static void LogLevelComplete(int level, int earnedCoins) =>
        LogEvent("level_complete",
                 new Parameter("level_number", level),
                 new Parameter("earned_coins", earnedCoins));

    public static void LogAdWatched(string adType) =>
        LogEvent("ad_watched", new Parameter("ad_type", adType));

    public static void LogRewardClaimed(string rewardType) =>
        LogEvent("reward_claimed", new Parameter("reward_type", rewardType));

    public static void LogItemPurchased(string itemName, int price) =>
        LogEvent("item_purchased",
                 new Parameter("item_name", itemName),
                 new Parameter("price", price));

    public static void LogNoAdsPurchased(int price) =>
        LogEvent("no_ads_purchased", new Parameter("price", price));

    public static void LogPlanetSelected(string planetName) =>
        LogEvent("planet_selected", new Parameter("planet_name", planetName));

    public static void LogDecorationPlaced(string decoName) =>
        LogEvent("decoration_placed", new Parameter("decoration_name", decoName));

    /*───────────────────  YENİ - ÇEKİRDEK OYUN  ───────────────────*/
    public static void LogRocketLaunched(string rocket, float fuelLeft) =>
        LogEvent("rocket_launched",
                 new Parameter("rocket_name", rocket),
                 new Parameter("fuel_left", fuelLeft));

    public static void LogShotResult(int level, bool hit, float distance) =>
        LogEvent("shot_result",
                 new Parameter("level_number", level),
                 new Parameter("hit_target", hit ? 1 : 0),
                 new Parameter("distance_to_target", distance));

    public static void LogPopulationChange(string planet, int delta) =>
        LogEvent("population_change",
                 new Parameter("planet_name", planet),
                 new Parameter("delta", delta));

    public static void LogPlanetUpgraded(string planet, int newStage) =>
        LogEvent("planet_upgraded",
                 new Parameter("planet_name", planet),
                 new Parameter("new_stage", newStage));

    /*─────────────────────  YENİ - EKONOMİ  ───────────────────────*/
    public static void LogCoinEarned(string source, int amount) =>
        LogEvent("coin_earned",
                 new Parameter("source", source),
                 new Parameter("amount", amount));

    public static void LogCoinSpent(string sink, int amount) =>
        LogEvent("coin_spent",
                 new Parameter("sink", sink),
                 new Parameter("amount", amount));

    /*────────────────────  YENİ - REKLAM  ────────────────────────*/
    public static void LogAdImpression(string placementId, bool rewarded) =>
        LogEvent("ad_impression",
                 new Parameter("placement_id", placementId),
                 new Parameter("rewarded", rewarded ? 1 : 0));

    /*────────────────────  YENİ - IAP FUNNEL  ─────────────────────*/
    public static void LogPurchaseStart(string productId) =>
        LogEvent("purchase_start", new Parameter("product_id", productId));

    public static void LogPurchaseFail(string productId, string error) =>
        LogEvent("purchase_fail",
                 new Parameter("product_id", productId),
                 new Parameter("error_code", error));

    /*────────────────────  YENİ - UX / MENÜ  ─────────────────────*/
    public static void LogMenuOpen(string menuName) =>
        LogEvent("menu_open", new Parameter("menu_name", menuName));

    public static void LogButtonClick(string buttonId) =>
        LogEvent("button_click", new Parameter("button_id", buttonId));

    /*────────────────────  YENİ - TEKNİK  ────────────────────────*/
    public static void LogException(string type, string message) =>
        LogEvent("exception",
                 new Parameter("exception_type", type),
                 new Parameter("message", message));
}
