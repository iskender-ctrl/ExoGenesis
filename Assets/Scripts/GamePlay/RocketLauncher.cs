using UnityEngine;
using UnityEngine.EventSystems;
public class RocketLauncher : MonoBehaviour
{
    [Header("Roket Ayarları")]
    public Transform spawnPoint;
    public float launchSpeed = 20f;
    public float rocketLifetime = 10f;
    [SerializeField] private FuelSystem fuelSystem;

    [Header("Çizgi Ayarları")]
    public LineRenderer guideLine;
    public Material dashedLineMaterial;
    public float lineMaxLength = 50f;

    [Header("Dönme Ayarları")]
    [SerializeField] private Transform rotatingPlatform; // Roketin bağlı olduğu platform (gezegen)
    [SerializeField] private float rotationSpeed = 5f;

    private Quaternion targetRotation = Quaternion.identity;

    private Camera mainCamera;
    private bool isTouching = false;
    private bool inputResetRequired = false;

    private GameObject currentRocket; // 🔹 Aktif roket
    public static bool IsPanelOpen = false;
    public static RocketLauncher Instance { get; private set; } // Singleton

    [Header("Roket Respawn Ayarı")]
    [SerializeField] private float respawnDelay = 1.5f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
        InitializeGuideLine();
        SpawnRocket(); // 🎯 İlk roket sahneye yerleştirilir
    }

    void Update()
    {
        if (IsPanelOpen)
        {
            isTouching = false;
            guideLine.enabled = false;
            return;
        }

        if (inputResetRequired)
        {
            if (!Input.GetMouseButton(0) && Input.touchCount == 0)
                inputResetRequired = false;

            return;
        }

        HandleInput();
        UpdateGuideLine();
        UpdateRotation();
    }

    private void InitializeGuideLine()
    {
        guideLine.positionCount = 2;
        guideLine.enabled = false;
        guideLine.material = dashedLineMaterial;
        guideLine.textureMode = LineTextureMode.Tile;
    }

    private void HandleInput()
    {
        bool uiBlocked = false;
#if UNITY_EDITOR || UNITY_STANDALONE
        uiBlocked = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
    if (Input.touchCount > 0)
        uiBlocked = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif

        if (uiBlocked)
            return;
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (IsPanelOpen) return;

            isTouching = true;
            guideLine.enabled = true;
        }

        if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && isTouching)
        {
            isTouching = false;
            guideLine.enabled = false;

            if (IsPanelOpen) return;

            if (fuelSystem.HasFuel())
            {
                LaunchRocket(); // 🚀 Fırlat
            }
            else
            {
                Debug.Log("Yakıt tükendi! Fırlatma yapılamaz.");
            }

            inputResetRequired = true;
        }
    }

    private void UpdateGuideLine()
    {
        if (!isTouching || IsPanelOpen || currentRocket == null)
        {
            guideLine.enabled = false;
            return;
        }

        Vector3 targetPos = GetTouchOrMousePosition();
        Vector3 launchDirection = (targetPos - spawnPoint.position).normalized;

        guideLine.SetPosition(0, spawnPoint.position);
        guideLine.SetPosition(1, spawnPoint.position + launchDirection * lineMaxLength);

        targetRotation = Quaternion.LookRotation(launchDirection);
    }

    private void UpdateRotation()
    {
        if (!isTouching) return;

        if (currentRocket != null)
        {
            currentRocket.transform.rotation = Quaternion.Lerp(
                currentRocket.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }

        if (rotatingPlatform != null)
        {
            rotatingPlatform.rotation = Quaternion.Lerp(
                rotatingPlatform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    private Vector3 GetTouchOrMousePosition()
    {
        if (Input.touchCount > 0)
            return GetWorldPosition(Input.GetTouch(0).position);

        return GetWorldPosition(Input.mousePosition);
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, spawnPoint.position);
        if (groundPlane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return spawnPoint.position + spawnPoint.forward * 10f;
    }

    public void SpawnRocket()
    {
        if (currentRocket != null)
            Destroy(currentRocket);

        string selectedRocket = PlayerPrefs.GetString("SelectedRocket", "XWeeng");
        GameObject prefab = Resources.Load<GameObject>($"Rockets/{selectedRocket}");

        if (prefab == null)
        {
            Debug.LogError($"🚫 Prefab bulunamadı: Rockets/{selectedRocket}");
            return;
        }

        currentRocket = Instantiate(prefab, spawnPoint.position, transform.rotation);
    }

    private void LaunchRocket()
    {
        if (currentRocket == null)
        {
            Debug.LogWarning("🚫 Fırlatılacak roket yok.");
            return;
        }
        currentRocket.GetComponent<RocketLanding>().particle.SetActive(true);
        LevelManager.Instance?.ResetShotState();

        Vector3 targetPos = GetTouchOrMousePosition();
        Vector3 direction = (targetPos - spawnPoint.position).normalized;

        Rigidbody rb = currentRocket.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * launchSpeed;

        Destroy(currentRocket, rocketLifetime);
        currentRocket = null;
    }
}
