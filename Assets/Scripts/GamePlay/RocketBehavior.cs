using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody rocketRigidbody;
    private RocketMovement rocketMovement;
    private RocketCollisionHandler rocketCollisionHandler;
    private RocketLanding rocketLanding;

    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        rocketMovement = new RocketMovement(rocketRigidbody);
        rocketCollisionHandler = new RocketCollisionHandler(this, rocketMovement);
        rocketLanding = GetComponent<RocketLanding>();
    }

    private void Update()
    {
        // Roket açısı 0'a yakın değilse sürekli yönünü hıza göre ayarla
        if (rocketLanding != null && rocketLanding.IsLandingActive())
        {
            float angle = rocketLanding.GetCurrentAngle();
            if (rocketRigidbody.linearVelocity.sqrMagnitude > 0.1f && angle > 1f)
            {
                transform.rotation = Quaternion.LookRotation(rocketRigidbody.linearVelocity.normalized);
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(rocketRigidbody.linearVelocity.normalized);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        rocketCollisionHandler.HandleTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        rocketCollisionHandler.HandleTriggerStay(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        rocketCollisionHandler.HandleCollisionEnter(collision);
    }

    // Instantiate işlemini yapacak metod
    public void InstantiateExplosion(Vector3 position)
    {
        Instantiate(explosionPrefab, position, Quaternion.identity);
    }
}
