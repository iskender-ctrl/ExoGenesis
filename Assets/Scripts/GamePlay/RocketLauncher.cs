using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    [Header("Roket Ayarları")]
    public GameObject rocketPrefab;
    public Transform spawnPoint;
    public float launchSpeed = 20f;
    public float rocketLifetime = 10f;

    [Header("Çizgi Ayarları")]
    [SerializeField] private LineRenderer guideLine;
    [SerializeField] private float dashSize = 5f;

    private Camera mainCamera;
    private bool isMouseHeld = false;

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
        guideLine.positionCount = 2;
        guideLine.enabled = false;
        guideLine.material = new Material(Shader.Find("Sprites/Default"));
        guideLine.startColor = Color.white;
        guideLine.endColor = Color.white;
        guideLine.textureMode = LineTextureMode.Tile;
        guideLine.material.mainTexture = CreateDashedTexture();
        guideLine.material.mainTextureScale = new Vector2(1f/dashSize, 1);
    }

    private Texture2D CreateDashedTexture()
    {
        Texture2D tex = new Texture2D(4, 1);
        tex.SetPixels(new Color[] {Color.cyan, Color.cyan, Color.clear, Color.clear});
        tex.Apply();
        return tex;
    }

    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isMouseHeld = true;
            guideLine.enabled = true;
        }
        
        if(Input.GetMouseButtonUp(0) && isMouseHeld)
        {
            FireRocket();
            isMouseHeld = false;
            guideLine.enabled = false;
        }
    }

    private void UpdateGuideLine()
    {
        if(!isMouseHeld) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        
        // Çizgiyi spawnPoint'ten mouse pozisyonuna çek
        guideLine.SetPosition(0, spawnPoint.position);
        guideLine.SetPosition(1, mouseWorldPos);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, spawnPoint.position);
        return groundPlane.Raycast(ray, out float distance) ? 
            ray.GetPoint(distance) : 
            spawnPoint.position + spawnPoint.forward * 10f;
    }

    private void FireRocket()
    {
        Vector3 targetPos = GetMouseWorldPosition();
        Vector3 direction = (targetPos - spawnPoint.position).normalized;
        
        GameObject rocket = Instantiate(rocketPrefab, spawnPoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if(rb != null) rb.linearVelocity = direction * launchSpeed;
        
        Destroy(rocket, rocketLifetime);
    }
}