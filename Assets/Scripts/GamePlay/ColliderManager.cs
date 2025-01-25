using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    public CelestialBodyData celestialBodyData;
    public Material sphereMaterial;

    void Start()
    {
        foreach (Transform child in transform)
        {
            var bodyData = celestialBodyData.celestialBodies.Find(b => b.bodyName == child.name);
            if (bodyData != null)
            {
                CreateGravityVisualizer(child, bodyData);
                AddPlanetMeshCollider(child, bodyData);
            }
        }
    }

    private void CreateGravityVisualizer(Transform planet, CelestialBodyData.CelestialBody bodyData)
    {
        // Yeni bir küre oluştur
        GameObject gravitySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gravitySphere.name = $"{planet.name}_GravityField";
        gravitySphere.transform.SetParent(planet); // Gezegenin alt nesnesi yap
        gravitySphere.tag = "GravityField";
        gravitySphere.transform.localPosition = Vector3.zero;

        // Çapı hesapla ve uygula
        float globalScale = planet.lossyScale.x;
        float radius = Mathf.Sqrt(bodyData.mass) * GravitySettings.RadiusMultiplier / globalScale;
        gravitySphere.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

        // Collider ekle
        SphereCollider gravityCollider = gravitySphere.GetComponent<SphereCollider>();
        if (gravityCollider == null)
        {
            gravityCollider = gravitySphere.gameObject.AddComponent<SphereCollider>();
        }

        // Collider'ı ayarla
        gravityCollider.isTrigger = true; // Yerçekim alanı için tetikleyici olarak ayarla
        gravityCollider.radius = radius * 25;

        // Materyal ekle
        MeshRenderer renderer = gravitySphere.GetComponent<MeshRenderer>();
        if (sphereMaterial != null)
        {
            renderer.material = sphereMaterial;
        }
    }

    private void AddPlanetMeshCollider(Transform planet, CelestialBodyData.CelestialBody bodyData)
    {
        // MeshFilter var mı kontrol et (MeshCollider'ın bir mesh'e ihtiyacı var)
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
            meshCollider.isTrigger = false;                 // isTrigger özelliğini ata
            meshCollider.convex = false;                    // Convex ayarını ata (gerekiyorsa)
        }
    }
}
