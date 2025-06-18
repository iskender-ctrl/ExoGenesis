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

        yield return new WaitForSeconds(0.5f); // Başlangıçta bekleme

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        float loadingTime = 3f; // Slider en az bu süre boyunca dönecek
        float elapsedTime = 0f;

        float fakeFill = 0f;

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            fakeFill += Time.deltaTime * 1.5f; // Ne kadar hızlı dönsün? 1.5 => 1.5 saniyede bir tur
            loadingBar.fillAmount = fakeFill % 1f;
            yield return null;
        }

        // Gerçek sahne yükleme ilerlemesine geç
        float realProgress = 0f;
        while (!asyncLoad.isDone)
        {
            float targetProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            realProgress = Mathf.MoveTowards(realProgress, targetProgress, Time.deltaTime);
            loadingBar.fillAmount = realProgress;

            if (realProgress >= 0.99f)
            {
                loadingBar.fillAmount = 1f;
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
