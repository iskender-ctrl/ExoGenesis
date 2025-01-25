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

    void OnTriggerStay(Collider other)
    {
        // Çekim alanına girdiğinde çalışır
        if (other.CompareTag("GravityField"))
        {
            // Çekim alanına sahip gezegeni bul
            string planetName = other.transform.name;
            var body = celestialBodies.Find(b => b.bodyName == planetName);
            
            if (body != null)
            {
                
                ApplyGravitationalAssist(body, other.transform.position);
                Debug.Log("çekim alanına girdi");
            }
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
