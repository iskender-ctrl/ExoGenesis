using UnityEngine;

public class RocketCollision : MonoBehaviour
{
    private Rigidbody rocketRigidbody;

    void Start()
    {
        // Rigidbody referansını başlangıçta al
        rocketRigidbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("CelestialBody"))
        {

            // Roketin hareketini durdur
            if (rocketRigidbody != null)
            {
                rocketRigidbody.linearVelocity = Vector3.zero;
                rocketRigidbody.angularVelocity = Vector3.zero;
                rocketRigidbody.isKinematic = true; // Roketi hareket ettirmeyi durdur
            }
            // Çarpışma sonrası roketi yok et
            Destroy(gameObject, 0.0f);
        }
    }
}
