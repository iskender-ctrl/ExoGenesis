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

        // SphereCollider ekle (tetikleyici olarak çalışır)
        SphereCollider gravityCollider = gravityField.AddComponent<SphereCollider>();
        gravityCollider.isTrigger = true;
        gravityCollider.radius = Mathf.Sqrt(bodyData.mass) * GravitySettings.RadiusMultiplier;

        // GravityField objesini etiketle
        if (planet.CompareTag("Target"))
            gravityField.tag = "TargetField";
        else
            gravityField.tag = "GravityField";

        // Quad görseli oluştur
        GameObject quadVisual = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadVisual.transform.SetParent(gravityField.transform);
        quadVisual.transform.localPosition = new Vector3(0f, -0.1f, 0f);

        // Quad'ı XZ düzlemine döndür (varsayılanı XY düzlemi)
        quadVisual.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

        // Quad'ın boyutunu collider çapına göre ayarla
        float diameter = gravityCollider.radius * 2f;
        quadVisual.transform.localScale = new Vector3(diameter, diameter, 1f);

        // Renderer ve materyal ayarı
        MeshRenderer renderer = quadVisual.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(gravityFieldMaterial);
        renderer.material = newMaterial;

        // Renk ve efekt yoğunlukları
        float minMass = 50f;
        float maxMass = 550f;
        float t = Mathf.InverseLerp(minMass, maxMass, bodyData.mass);



        // Ya da gradient ile:
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
        new GradientColorKey(Color.white, 0f),
        new GradientColorKey(Color.blue, 0.25f),
        new GradientColorKey(Color.green, 0.5f),
        new GradientColorKey(Color.yellow, 0.75f),
        new GradientColorKey(Color.red, 1f)
            },
            new GradientAlphaKey[] {
        new GradientAlphaKey(1f, 0f),
        new GradientAlphaKey(1f, 1f)
            }
        );

        Color ringColor = gradient.Evaluate(t);
        if (planet.CompareTag("Target"))
            newMaterial.SetColor("_RingColor", Color.red);
        else
            newMaterial.SetColor("_RingColor", ringColor);




        // Quad üzerindeki collider'ı kaldır (sadece görsel amaçlı)
        Destroy(quadVisual.GetComponent<Collider>());
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