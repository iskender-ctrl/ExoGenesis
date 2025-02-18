using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Analytics;
using System.Collections;
using System.Threading.Tasks;
using Firebase.Extensions;

public class PlayGamesFirebaseAuth : MonoBehaviour
{
    private FirebaseApp app;
    public Image loadingBar; // Inspector'da atanmalı
    public string targetScene = "MainMenu"; // Yüklenecek sahne adı

    void Start()
    {
        // Firebase'i başlat
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;

                // Firebase Analytics'i başlat
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("🔥 Firebase Başlatıldı! (Authentication olmadan)");

                // Firebase Event - Oyun başlatıldı
                FirebaseAnalytics.LogEvent("game_started");

                // ✅ Firebase Başladıktan Sonra Sahne Yüklemeyi Başlat
                StartCoroutine(LoadSceneWithProgress());
            }
            else
            {
                Debug.LogError("❌ Firebase başlatılamadı: " + task.Result);
            }
        });
    }

    IEnumerator LoadSceneWithProgress()
    {
        Debug.Log("🚀 Sahne yükleme süreci başlıyor...");

        yield return new WaitForSeconds(1f); // Başlangıçta kısa bir bekleme

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false; // Sahnenin otomatik geçişini engelle

        float progress = 0f;

        while (!asyncLoad.isDone)
        {
            // AsyncOperation'un ilerlemesini al
            float targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // Dolum çubuğunu yumuşak bir şekilde güncelle
            progress = Mathf.MoveTowards(progress, targetProgress, Time.deltaTime);
            loadingBar.fillAmount = progress;

            // Yüzde 90'ı geçtiğinde dolum çubuğunu tam doldur ve sahneyi aktive et
            if (progress >= 0.99f)
            {
                loadingBar.fillAmount = 1f;
                yield return new WaitForSeconds(0.5f); // Tam dolduktan sonra kısa bir bekleme
                asyncLoad.allowSceneActivation = true; // Sahneyi aktive et
            }

            yield return null;
        }
    }
}
