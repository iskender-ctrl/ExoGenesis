using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMain : MonoBehaviour
{
    /*public void StartGame()
    {
        int playerLevel = PlayerDataManager.GetLevel();
        string sceneName = "Level" + playerLevel; // Örneğin, Level1, Level2, Level3...

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Böyle bir sahne bulunamadı: " + sceneName);
        }
    }*/
    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(3);
    }
}
