using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public Material gravityFieldMaterial; // GravityField için materyal
    public void InitializeGravitySystem()
    {
        // Tüm gökcisimlerini işle
        foreach (var body in GameManager.Instance.celestialBodyData.celestialBodies)
        {
            // Gezegenin sahnedeki objesini bul
            Transform planetTransform = GameManager.Instance.spaceObjectsParent.Find(body.bodyName);
            if (planetTransform != null)
            {
                // GravityField oluştur
                CreateGravityField(planetTransform, body);

                // Gezegen için MeshCollider ekle
                AddPlanetCollider(planetTransform);
            }
        }
    }

    private void CreateGravityField(Transform planet, CelestialBodyData.CelestialBody bodyData)
    {
        // Yeni bir GravityField objesi oluştur
        GameObject gravityField = new GameObject($"{bodyData.bodyName}_GravityField");
        gravityField.transform.SetParent(planet);
        gravityField.transform.localPosition = Vector3.zero;

        // SphereCollider ekle
        SphereCollider gravityCollider = gravityField.AddComponent<SphereCollider>();
        gravityCollider.isTrigger = true; // Çekim alanı tetikleyici olmalı
        gravityCollider.radius = Mathf.Sqrt(bodyData.mass) * GravitySettings.RadiusMultiplier;

        // GravityField objesini etiketle
        gravityField.tag = "GravityField";

        // Küre görseli oluştur
        GameObject sphereVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereVisual.transform.SetParent(gravityField.transform);
        sphereVisual.transform.localPosition = Vector3.zero;
        sphereVisual.transform.localScale = Vector3.one * gravityCollider.radius * 2f; // Çapı collider ile eşleştir

        // Renderer ve materyal ayarı
        MeshRenderer renderer = sphereVisual.GetComponent<MeshRenderer>();
        // Yeni materyal örneği oluştur
        Material newMaterial = new Material(gravityFieldMaterial);
        renderer.material = newMaterial;

        // GravityColor değerini ayarla (mass'tan alınıyor)
        float normalizedMass = Mathf.Clamp(bodyData.mass, 0, 450); // 0-1 arasında normalize et
        newMaterial.SetFloat("_Gravity", normalizedMass);

        // Collider görselinin fiziksel etkisini kaldır (sadece görsel için)
        Destroy(sphereVisual.GetComponent<Collider>());
    }

    private void AddPlanetCollider(Transform planet)
    {
        // Gezegenin MeshFilter bileşenini kontrol et
        MeshFilter meshFilter = planet.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            // MeshCollider ekle veya mevcut olanı al
            MeshCollider meshCollider = planet.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = planet.gameObject.AddComponent<MeshCollider>();
            }

            // MeshCollider ayarlarını yap
            meshCollider.sharedMesh = meshFilter.sharedMesh; // Mesh'i ata
            meshCollider.isTrigger = false;                 // Çarpışmalar için tetikleyici kapalı
            meshCollider.convex = false;                    // Konveks değil
        }
    }
}
