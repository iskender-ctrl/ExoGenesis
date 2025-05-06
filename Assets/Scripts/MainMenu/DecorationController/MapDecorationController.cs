using UnityEngine;

public class MapDecorationController : MonoBehaviour
{
    public static MapDecorationController Instance;
    public ClickablePlanetDatabase planetDatabase;
    public string planetName;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 🔥 Bu obje fazlaysa yok et
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 🎯 Yalnızca bir tanesi kalır
    }
}
