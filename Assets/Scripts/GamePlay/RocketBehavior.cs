using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    private Rigidbody rocketRigidbody; private float polarTime; // Spiral hareket için zaman sayacı
    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        UpdateRotation();
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GravityField"))
        {
            var body = CelestialBodyHelper.FindBodyByName(other.transform.parent.name);
            if (body != null)
            {
                ApplyGravitationalForce(body, other.transform.position);
            }
        }
        
    }

    private void ApplyGravitationalForce(CelestialBodyData.CelestialBody body, Vector3 gravityCenter)
    {
        Vector3 direction = (gravityCenter - transform.position).normalized;
        float distance = Vector3.Distance(gravityCenter, transform.position);
        float forceMagnitude = (GravitySettings.Instance.gravitationalConstant * body.mass) / (distance * distance);
        Vector3 force = direction * forceMagnitude;
        rocketRigidbody.AddForce(force);

    }
    private void ApplyPolarForce(CelestialBodyData.CelestialBody body, Vector3 gravityCenter)
    {
        // Roket ve gezegen arasındaki mesafeyi hesapla
        Vector3 directionToCenter = (gravityCenter - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, gravityCenter);

        // Radyal Kuvvet (gezegene doğru çekim)
        float radialForceMagnitude = (GravitySettings.Instance.gravitationalConstant * body.mass) / (distance * distance);
        Vector3 radialForce = directionToCenter * radialForceMagnitude;

        // Tanjantial Kuvvet (gezegen etrafında dönme)
        Vector3 tangentialDirection = Vector3.Cross(directionToCenter, Vector3.up).normalized; // Yukarı eksene göre çapraz
        float tangentialForceMagnitude = radialForceMagnitude * 0.5f; // Tanjantial kuvveti radyal kuvvetin bir oranı
        Vector3 tangentialForce = tangentialDirection * tangentialForceMagnitude;

        // Toplam Kuvvet
        Vector3 totalForce = radialForce + tangentialForce;

        // Rokete kuvvet uygula
        rocketRigidbody.AddForce(totalForce);
    }
    private void UpdateRotation()
    {
        // Eğer roketin bir hızı varsa, yönü hız vektörüne çevir
        if (rocketRigidbody.linearVelocity.magnitude > 0.1f) // Eğer hız küçükse dönüş yapma
        {
            Quaternion targetRotation = Quaternion.LookRotation(rocketRigidbody.linearVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f); // Smooth dönüş
        }
    }
    void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.CompareTag("CelestialBody"))
        {
            if (rocketRigidbody != null)
            {
                rocketRigidbody.linearVelocity = Vector3.zero;
                rocketRigidbody.angularVelocity = Vector3.zero;
                rocketRigidbody.isKinematic = true;
            }
            Destroy(gameObject, 0.0f);
        }
    }
}
