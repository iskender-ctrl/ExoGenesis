using UnityEngine;
using TMPro;

public class RocketBehavior : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private TextMeshProUGUI speedText;

    private Rigidbody rocketRigidbody;
    private RocketMovement rocketMovement;
    private RocketCollisionHandler rocketCollisionHandler;
    private RocketUI rocketUI;

    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        rocketMovement = new RocketMovement(rocketRigidbody);
        rocketCollisionHandler = new RocketCollisionHandler(this, rocketMovement);
        rocketUI = new RocketUI(speedText, rocketRigidbody);
    }
    void FixedUpdate()
    {

    }

    void Update()
    {
        rocketUI.UpdateRocketSpeedUI();
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