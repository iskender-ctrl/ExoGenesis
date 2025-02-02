using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [Header("Roket Ayarları")]
    public GameObject rocketPrefab;
    public Transform spawnPoint;
    public float launchSpeed = 20f;
    public float rocketLifetime = 10f;

    [Header("Çizgi Ayarları")]
    public LineRenderer guideLine;
    public Material dashedLineMaterial;
    public float lineMaxLength = 50f;

    private Camera mainCamera;
    private bool isTouching = false;

    void Start()
    {
        mainCamera = Camera.main;
        InitializeGuideLine();
    }

    void Update()
    {
        HandleInput();
        UpdateGuideLine();
    }

    private void InitializeGuideLine()
    {
        if (guideLine == null)
        {
            Debug.LogError("Line Renderer bileşeni atanmamış!");
            return;
        }

        guideLine.positionCount = 2;
        guideLine.enabled = false;
        guideLine.material = dashedLineMaterial;
        guideLine.textureMode = LineTextureMode.Tile;
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isTouching = true;
            guideLine.enabled = true;
        }

        if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && isTouching)
        {
            FireRocket();
            isTouching = false;
            guideLine.enabled = false;
        }
    }

    private void UpdateGuideLine()
    {
        if (!isTouching) return;

        Vector3 targetPos = GetTouchOrMousePosition();
        Vector3 launchDirection = (targetPos - spawnPoint.position).normalized;

        guideLine.SetPosition(0, spawnPoint.position);
        guideLine.SetPosition(1, spawnPoint.position + launchDirection * lineMaxLength);
    }

    private Vector3 GetTouchOrMousePosition()
    {
        if (Input.touchCount > 0)
        {
            return GetWorldPosition(Input.GetTouch(0).position);
        }
        return GetWorldPosition(Input.mousePosition);
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, spawnPoint.position);
        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return spawnPoint.position + spawnPoint.forward * 10f;
    }

    private void FireRocket()
    {
        Vector3 targetPos = GetTouchOrMousePosition();
        Vector3 direction = (targetPos - spawnPoint.position).normalized;

        GameObject rocket = Instantiate(rocketPrefab, spawnPoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * launchSpeed;
        }

        Destroy(rocket, rocketLifetime);
    }
}
