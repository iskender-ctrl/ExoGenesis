using System.Collections.Generic;
using UnityEngine;

public class CelestialBodyManager : MonoBehaviour
{
    // Singleton – diğer scriptlerden erişim kolaylığı
    public static CelestialBodyManager Instance;

    // Sahnedeki tüm gezegen GameObject’leri
    public List<GameObject> celestialObjects = new List<GameObject>();

    /* ───────────────────────── LIFE-CYCLE ───────────────────────── */

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshCelestialObjects();     // Sahne açılır açılmaz listeyi doldur
    }

    private void FixedUpdate()
    {
        // Her fizik adımında tüm gezegenlerin dönüş/yörünge hareketini uygula
        foreach (var obj in celestialObjects)
        {
            ApplyRotation(obj);
            ApplyOrbitalMotion(obj);
        }
    }

    /* ───────────────────────── PUBLIC YARDIMCILAR ───────────────────────── */

    /// <summary>
    /// Sahnedeki tüm gezegenleri yeniden tarar ve listeyi günceller.
    /// (Örneğin sahne yeniden yüklendiğinde veya toplu spawn yapıldığında)
    /// </summary>
    public void RefreshCelestialObjects()
    {
        celestialObjects.Clear();

        Transform parent = GameManager.Instance.spaceObjectsParent;
        foreach (Transform child in parent)
        {
            celestialObjects.Add(child.gameObject);
        }
    }

    /// <summary>
    /// Dinamik olarak sahneye eklenen (Instantiate) yeni gezegeni listeye kaydeder.
    /// LevelManager içinde, Instantiate işleminden hemen sonra çağırın.
    /// </summary>
    public void RegisterNewCelestial(GameObject obj)
    {
        celestialObjects.Add(obj);
    }

    /* ───────────────────────── İÇ HAREKET METOTLARI ───────────────────────── */

    private void ApplyRotation(GameObject obj)
    {
        // Kendi ekseninde dönüş
        var bodyData = CelestialBodyHelper.FindBodyByName(obj.name);
        if (bodyData != null)
        {
            obj.transform.Rotate(Vector3.up, bodyData.rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyOrbitalMotion(GameObject obj)
    {
        // Yörüngede dönme
        var bodyData = CelestialBodyHelper.FindBodyByName(obj.name);
        if (bodyData == null || string.IsNullOrEmpty(bodyData.orbitAround))
            return;

        // Orbit merkezini sahnedeki parent altında ada göre bul
        Transform orbitCenter = GameManager.Instance.spaceObjectsParent
                                .Find(bodyData.orbitAround);
        if (orbitCenter == null)
            return;  // orbitAround ismi hatalıysa sessizce çık

        float distance    = Vector3.Distance(obj.transform.position, orbitCenter.position);
        float centralMass = CelestialBodyHelper.FindBodyByName(orbitCenter.name)?.mass ?? 0f;
        if (centralMass <= 0f)
            return;  // merkezi kütle tanımlı değilse döndürme yapma

        float orbitalSpeed = Mathf.Sqrt((GravitySettings.GravitationalConstant *
                                         GravitySettings.OrbitalSpeedMultiplier *
                                         centralMass) / distance);

        obj.transform.RotateAround(orbitCenter.position, Vector3.up,
                                   orbitalSpeed * Time.fixedDeltaTime);
    }
}
