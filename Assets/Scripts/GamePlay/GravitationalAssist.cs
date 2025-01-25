using System.Collections.Generic;
using UnityEngine;

public class GravitationalAssist : MonoBehaviour
{
    public CelestialBodyData celestialBodyData;
    private Rigidbody rocketRigidbody;
    private List<CelestialBodyData.CelestialBody> celestialBodies;

    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        if (rocketRigidbody == null)
        {
            enabled = false;
            return;
        }

        // CelestialBodyData'dan tüm gezegen bilgilerini al
        if (celestialBodyData != null)
        {
            celestialBodies = celestialBodyData.celestialBodies;
        }
        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        UpdateRotation();
    }

    public GameObject spaceObjectsParent; // SpaceObjects objesi için referans

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GravityField"))
        {
            Debug.Log($"GravityField tetiklendi: {other.name}");

            // Gezegenleri SpaceObjects parent'ı üzerinden kontrol et
            foreach (var body in celestialBodies)
            {
                Debug.Log($"Kontrol edilen gezegen: {body.bodyName}");

                // SpaceObjects altından gezegeni bul
                Transform planetTransform = spaceObjectsParent.transform.Find(body.bodyName);
                if (planetTransform == null)
                {
                    Debug.LogWarning($"Gezegen bulunamadı: {body.bodyName}");
                    continue;
                }

                // Child objeleri kontrol et
                foreach (Transform child in planetTransform)
                {
                    Debug.Log($"Kontrol edilen child obje: {child.name}");
                    if (other.transform == child)
                    {
                        ApplyGravitationalAssist(body, other.transform.position);
                        Debug.Log($"{body.bodyName} çekim alanına girdi (child obje: {child.name}).");
                        return;
                    }
                }
            }

            Debug.LogWarning("GravityField bir gezegenle eşleşmedi.");
        }
    }



    void ApplyGravitationalAssist(CelestialBodyData.CelestialBody body, Vector3 gravityCenter)
    {
        // Roket ile gezegen arasındaki mesafeyi ve yönü hesapla
        Vector3 direction = (gravityCenter - transform.position).normalized;
        float distance = Vector3.Distance(gravityCenter, transform.position);

        // Çekim kuvvetini hesapla
        float forceMagnitude = (GravitySettings.GravitationalConstant * body.mass) / (distance * distance);
        Vector3 force = direction * forceMagnitude;

        // Kuvveti roketin hızına uygula
        rocketRigidbody.linearVelocity += force * Time.deltaTime;
    }

    void UpdateRotation()
    {
        // Roketin mevcut hızına göre rotasyonunu güncelle
        if (rocketRigidbody.linearVelocity.magnitude > 0.1f) // Hız yeterince büyükse yönlendirme yap
        {
            Quaternion rotation = Quaternion.LookRotation(rocketRigidbody.linearVelocity.normalized);
            transform.rotation = rotation;
        }
    }
}
