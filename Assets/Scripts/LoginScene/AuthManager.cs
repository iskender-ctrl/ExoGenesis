using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using System.Collections;

public class PlayGamesFirebaseAuth : MonoBehaviour
{
    private FirebaseAuth auth;
    public Image loadingBar; // Inspector'da atanmalı
    public string targetScene = "MainMenu"; // Yüklenecek sahne adı

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(LoadSceneWithProgress());
    }

    IEnumerator LoadSceneWithProgress()
    {
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