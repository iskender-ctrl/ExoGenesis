using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject[] planetPrefabs;
    private LevelDatabase levelDatabase;

    void Start()
    {
        LoadLevelData();
        int currentLevel = PlayerDataManager.GetLevel();
        LoadLevel(currentLevel);
    }

    void LoadLevelData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("levelData"); 
        if (jsonFile != null)
        {
            levelDatabase = JsonUtility.FromJson<LevelDatabase>(jsonFile.text);
        }
        else
        {
            Debug.LogError("levelData.json bulunamadı!");
        }
    }

    void LoadLevel(int level)
    {
        LevelData levelData = levelDatabase.levels.Find(l => l.level == level);

        if (levelData != null)
        {
            Debug.Log("Level " + level + " yükleniyor...");
            foreach (PlanetData planet in levelData.planets)
            {
                GameObject prefab = FindPlanetPrefab(planet.name);
                if (prefab != null)
                {
                    Vector3 position = new Vector3(planet.position[0], planet.position[1], planet.position[2]);
                    GameObject newPlanet = Instantiate(prefab, position, Quaternion.identity);
                    newPlanet.transform.localScale = Vector3.one * planet.scale;

                    // Hedef gezegen işaretlensin (renk değişimi gibi)
                    if (planet.name == levelData.targetPlanet)
                    {
                        newPlanet.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Level " + level + " bulunamadı!");
        }
    }

    GameObject FindPlanetPrefab(string name)
    {
        foreach (GameObject prefab in planetPrefabs)
        {
            if (prefab.name == name)
            {
                return prefab;
            }
        }
        return null;
    }

    // ✅ LEVEL TAMAMLANDIĞINDA ÇAĞRILACAK METOD
    public void LevelCompleted()
    {
        int nextLevel = PlayerDataManager.GetLevel() + 1;
        Debug.Log("Level tamamlandı! Yeni Level: " + nextLevel);
        
        PlayerDataManager.SetLevel(nextLevel);
        RestartScene();
    }

    // ✅ SAHNEYİ YENİDEN YÜKLER
    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
