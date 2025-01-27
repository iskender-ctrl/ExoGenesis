using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    private Rigidbody rocketRigidbody;
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
        if (other.CompareTag("TargetField")) // Çekim alanında kalma
        {
            var body = CelestialBodyHelper.FindBodyByName(other.transform.parent.name);
            if (body != null)
            {
                // Spiral hareketi veya yörüngeyi sürdür
                transform.RotateAround(other.transform.position, Vector3.up, 1f);
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
        if (collision.gameObject.CompareTag("TargetField"))
        {
            if (rocketRigidbody != null)
            {
                rocketRigidbody.linearVelocity = Vector3.zero;
                rocketRigidbody.angularVelocity = Vector3.zero;
                rocketRigidbody.isKinematic = true;
            }
        }
    }
    
}
