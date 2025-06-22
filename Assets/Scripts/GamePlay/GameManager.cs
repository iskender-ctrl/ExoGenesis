using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    [Header("Global References")]
    public Transform spaceObjectsParent; // Gezegenlerin bağlı olduğu ana parent
    public CelestialBodyData celestialBodyData; // Gökcisimlerinin verilerini tutan ScriptableObject
    private GravityManager gravityManager; // GravityManager referansı

    private void Awake()
    {
        // Singleton kontrolü
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Fazla instance'ı yok et
        }
        // GravityManager referansını alın
        gravityManager = GetComponent<GravityManager>();
    }
    private void Start()
    {
        // Tüm sistemi başlat
        InitializeSystem();
    }

    private void InitializeSystem()
    {
        // GravityManager ile çekim alanlarını ve collider'ları başlat
        if (gravityManager != null)
        {
            gravityManager.InitializeGravitySystem();
        }
    }
    public void BackToMainMenuScene()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
