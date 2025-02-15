using UnityEngine;

public class MapDecorationController : MonoBehaviour
{
    public static MapDecorationController Instance;
    public ClickablePlanetDatabase planetDatabase; // Gezegen verileri
    public string planetName;
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
