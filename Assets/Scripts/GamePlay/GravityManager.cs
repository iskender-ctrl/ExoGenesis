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
        if (planet.CompareTag("Target"))
            gravityField.tag = "TargetField";
        else
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

        float intensity1 = 0.3f;
        float intensity2 = 0.1f;
        // GravityColor veya Color değerini ayarla
        if (planet.CompareTag("Target"))
        {

            // TargetField için renkler
            newMaterial.SetColor("_GravityColor1", new Color(255f, 0f, 0f) * intensity1);    // Birinci renk
            newMaterial.SetColor("_GravityColor2", new Color(255f, 0f, 0f) * intensity2); // İkinci renk
        }
        else
        {
            newMaterial.SetColor("_GravityColor1", new Color(255f, 255f, 255f) * intensity1);    // Birinci renk
            newMaterial.SetColor("_GravityColor2", new Color(191f, 18f, 188f) * intensity2);   // İkinci renk
        }

        // GravityColor değerini ayarla (mass'tan alınıyor)
        float normalizedMass = Mathf.Clamp(bodyData.mass, 0, 400); // 0-1 arasında normalize et
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
