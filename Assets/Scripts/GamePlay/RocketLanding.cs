using UnityEngine;

public class RocketLanding : MonoBehaviour
{
    private BoxCollider rocketCollider;
    private Rigidbody rocketRigidbody;

    private bool isLandingActive = false;
    private Transform currentPlanetTransform;
    private float currentGravityFieldRadius;

    // Hibrit sistem için süre ve hız kontrol değişkenleri
    private float maxTangentVelocity = 2f;
    private bool isReversePhase = false;
    private float currentAngle = 0f; // Güncel açı değeri (getter için)
    private bool speedForced = false;
    private float planetRadius; // Gezegenin yarıçapı
    private void Start()
    {
        rocketCollider = GetComponent<BoxCollider>(); // Roketin box collider bileşeni
        rocketRigidbody = GetComponent<Rigidbody>(); // Roketin Rigidbody bileşeni
    }

    public void InitializeLanding(float planetRadius, float gravityFieldRadius, Transform planetTransform)
    {
        this.planetRadius = planetRadius; // Gezegen yarıçapı burada atanıyor
        currentPlanetTransform = planetTransform;
        currentGravityFieldRadius = gravityFieldRadius;
        isLandingActive = true;
        isReversePhase = false;

        // 🟡 Hızı doğrudan 1.0f yap
        Vector3 velocityDirection = rocketRigidbody.linearVelocity.normalized;
        rocketRigidbody.linearVelocity = velocityDirection * 1.0f;
    }

    private void FixedUpdate()
    {
        if (isLandingActive && currentPlanetTransform != null)
        {
            ApplyRocketLanding(currentPlanetTransform, currentGravityFieldRadius);
        }
    }

    public void ApplyRocketLanding(Transform planetTransform, float gravityFieldRadius)
    {
        Vector3 gravityCenter = planetTransform.position;

        // --- 1. Roketin dünya uzayındaki merkez pozisyonu
        Vector3 rocketCenterWorld = transform.TransformPoint(rocketCollider.center);

        // --- 2. Roketin arka ucu (iniş yapan uç, world space'te)
        float rocketLength = rocketCollider.size.z * transform.lossyScale.z;
        Vector3 backPosition = rocketCenterWorld - transform.forward * (rocketLength / 2f);

        // --- 3. Normal çizgi = Gezegen merkezinden roketin arka ucuna çizilen çizgi
        Vector3 normalVector = (backPosition - gravityCenter).normalized;

        // --- 4. Roketin yönü
        Vector3 rocketDirection = transform.forward;

        // --- 5. Roketin hız vektörü
        Vector3 rocketVelocity = rocketRigidbody.linearVelocity;

        // --- Hız sabitleme: ilk çerçevede sadece bir kere sabit hız uygulanır
        if (!speedForced)
        {
            rocketVelocity = rocketVelocity.normalized * 1.0f;
            rocketRigidbody.linearVelocity = rocketVelocity;
            speedForced = true;
        }

        // --- 6. Roket hızını normal/tangent bileşenlere ayır
        float normalComponent = Vector3.Dot(rocketVelocity, normalVector);
        Vector3 normalVelocity = normalComponent * normalVector;
        Vector3 tangentVelocity = rocketVelocity - normalVelocity;

        // --- 7. Açının büyüklüğü
        float angle = Vector3.Angle(rocketDirection, normalVector);
        currentAngle = angle; // güncel açı kaydedilir

        // --- 8. Açının işareti (sağ/sol sapma)
        Vector3 cross = Vector3.Cross(normalVector, rocketDirection);


        // --- 9. Giriş açısı katsayısı (trigonometrik)
        float coefficient = 0f;
        if (angle >= 90f && angle <= 180f)
        {
            float t = (angle - 90f) / 90f;
            coefficient = Mathf.Sin(t * Mathf.PI * 0.5f);
        }
        else if (angle < 90f)
        {
            float t = Mathf.Abs(90f - angle) / 90f;
            coefficient = Mathf.Cos(t * Mathf.PI * 0.5f);
        }

        // --- 10. TangentVelocity manipülasyonu
        if (angle > 90f && tangentVelocity.magnitude < maxTangentVelocity)
        {
            tangentVelocity += tangentVelocity.normalized * (coefficient * Time.deltaTime);
        }
        else
        {
            tangentVelocity = Vector3.MoveTowards(tangentVelocity, Vector3.zero, Time.deltaTime);
        }

        // --- 11. NormalVelocity manipülasyonu
        if (!isReversePhase)
        {
            normalVelocity = Vector3.MoveTowards(normalVelocity, Vector3.zero, coefficient * Time.deltaTime);

            if (normalVelocity.magnitude < 0.01f)
                isReversePhase = true;
        }
        if (angle < 1f)
        {
            normalVelocity += -normalVector/2 * Time.deltaTime;
        }
        Debug.Log(angle);
        // --- 12. Hızı güncelle
        rocketRigidbody.linearVelocity = normalVelocity + tangentVelocity;
    }

    public void StopLanding()
    {
        isLandingActive = false;
    }

    public bool IsLandingActive()
    {
        return isLandingActive;
    }

    public float GetCurrentAngle()
    {
        return currentAngle;
    }
}
