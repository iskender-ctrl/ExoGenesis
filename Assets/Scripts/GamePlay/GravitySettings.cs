using UnityEngine;

public class GravitySettings : MonoBehaviour
{
    [Header("Yerçekimi Ayarları")]
    public float gravitationalConstant = 0.1f; // Yerçekimi sabiti
    public float radiusMultiplier = 1f; // Görsel alan boyutu çarpanı
    public float orbitalSpeedMultiplier =1f;
    private static GravitySettings instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static float GravitationalConstant => instance.gravitationalConstant;
    public static float RadiusMultiplier => instance.radiusMultiplier;
    public static float OrbitalSpeedMultiplier => instance.orbitalSpeedMultiplier;
}
