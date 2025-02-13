using UnityEngine;
using TMPro;

public class RocketBehavior : MonoBehaviour
{
    [SerializeField] private float spiralSpeed = 4f;

    private Rigidbody rocketRigidbody;
    private bool isInTargetField;
    private Transform currentTarget;
    private float initialDistance;
    private Vector3 orbitDirection;
    private TextMeshProUGUI speedText; // Hız göstergesi prefab'ı
    [SerializeField] private GameObject explosionPrefab;
   

    void Start()
    {
        rocketRigidbody = GetComponent<Rigidbody>();
        if (speedText == null)
        {
            speedText = GetComponentInChildren<TextMeshProUGUI>(); // Roketin child objeleri içinde Text bileşenini bul
        }
    }

    void FixedUpdate()
    {
        if (isInTargetField && currentTarget != null)
            ApplySpiralMotion();
    }

    void Update()
    {
        UpdateRotation();
        UpdateSpeedUI();
    }
    

    private void UpdateSpeedUI()
    {
        if (speedText != null && rocketRigidbody != null)
        {
            float speed = rocketRigidbody.linearVelocity.magnitude;
            speedText.text = $"{speed:F1} m/s";
        }
    }
    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "TargetField":
                EnterTargetField(other);
                break;
            case "CelestialBody":
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                DestroyRocket();
                break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TargetField"))
            ExitTargetField();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("GravityField"))
            HandleStandardGravity(other);
        else if (other.CompareTag("TargetField"))
            HandleTargetGravity(other);
    }

    private void EnterTargetField(Collider other)
    {
        isInTargetField = true;
        currentTarget = other.transform.parent;
        initialDistance = Vector3.Distance(transform.position, currentTarget.position);
        CalculateOrbitDirection();
    }

    private void ExitTargetField()
    {
        isInTargetField = false;
        currentTarget = null;
    }

    private void CalculateOrbitDirection()
    {
        if (currentTarget == null) return;

        Vector3 relativePosition = transform.position - currentTarget.position;
        Vector3 relativeVelocity = rocketRigidbody.linearVelocity;

        orbitDirection = Vector3.Cross(relativePosition, relativeVelocity).y > 0 ? Vector3.up : Vector3.down;
    }

    private void HandleStandardGravity(Collider other)
    {
        var body = CelestialBodyHelper.FindBodyByName(other.transform.parent.name);
        if (body != null)
            ApplyGravitationalForce(body, other.transform.position);
    }

    private void HandleTargetGravity(Collider other)
    {
        var targetBody = CelestialBodyHelper.FindBodyByName(other.transform.parent.name);
        if (targetBody != null)
        {
            ApplyGravitationalForce(targetBody, other.transform.position, 0.1f);
            StabilizeOrbit(targetBody, other.transform.position);
        }
    }

    private void ApplyGravitationalForce(CelestialBodyData.CelestialBody body, Vector3 gravityCenter, float intensityMultiplier = 1f)
    {
        Vector3 direction = (gravityCenter - transform.position).normalized;
        float distance = Vector3.Distance(gravityCenter, transform.position);
        float forceMagnitude = (GravitySettings.GravitationalConstant * body.mass) / (distance * distance);

        rocketRigidbody.AddForce(direction * forceMagnitude * intensityMultiplier);
    }

    private void StabilizeOrbit(CelestialBodyData.CelestialBody body, Vector3 gravityCenter)
    {
        float distance = Vector3.Distance(gravityCenter, transform.position);
        float orbitalSpeed = Mathf.Sqrt((GravitySettings.GravitationalConstant * body.mass) / distance) * GravitySettings.OrbitalSpeedMultiplier;

        Vector3 tangentDirection = Vector3.Cross(transform.position - gravityCenter, orbitDirection).normalized;
        rocketRigidbody.linearVelocity = Vector3.Lerp(
            rocketRigidbody.linearVelocity,
            tangentDirection * orbitalSpeed,
            Time.deltaTime
        );
    }

    private void ApplySpiralMotion()
    {
        float currentDistance = Vector3.Distance(transform.position, currentTarget.position);
        float spiralProgress = 1 - (currentDistance / initialDistance);

        Vector3 spiralDirection = (currentTarget.position - transform.position).normalized;
        rocketRigidbody.AddForce(spiralDirection * spiralSpeed * spiralProgress, ForceMode.Acceleration);
        rocketRigidbody.linearVelocity *= 1 + (0.3f * Time.deltaTime);
    }

    private void UpdateRotation()
    {
        if (rocketRigidbody.linearVelocity.magnitude > 0.1f)
        {
            float rotationSpeed = isInTargetField ? 5f : 10f;
            Vector3 smoothedVelocity = Vector3.Lerp(transform.forward, rocketRigidbody.linearVelocity.normalized, Time.deltaTime * rotationSpeed);
            Quaternion targetRotation = Quaternion.LookRotation(smoothedVelocity);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            rocketRigidbody.isKinematic = true;
            Debug.Log("Roket hedefe ulaştı ve durdu!");
        }
        else if (collision.gameObject.CompareTag("CelestialBody"))
        {


            DestroyRocket();
        }
    }

    private void DestroyRocket()
    {
        if (rocketRigidbody != null)
        {
            rocketRigidbody.isKinematic = false;
            rocketRigidbody.linearVelocity = Vector3.zero;
            rocketRigidbody.angularVelocity = Vector3.zero;
            rocketRigidbody.isKinematic = true;
        }

        gameObject.SetActive(false);
    }
}
