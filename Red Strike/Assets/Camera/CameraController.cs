using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public CameraMode currentMode = CameraMode.FreeLook;

    private Camera mainCamera;

    public Transform selectedVehicle;

    [Header("Free Look Camera Settings")]
    public float moveSpeed = 20f;
    public float zoomSpeed = 5f;
    public float rotateSpeed = 100f;
    public float verticalSpeed = 10f;

    [Header("TPS Camera Settings")]
    public Transform tpsCameraPosition;
    public float heightOffset = 2f;
    public float tpsSensitivityX = 2f;
    public float tpsSensitivityY = 2f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 40f;

    public float currentYaw;
    public float currentPitch;

    public Vector2 panLimitX = new Vector2(-50f, 50f);
    public Vector2 panLimitZ = new Vector2(-50f, 50f);
    public Vector2 panLimitY = new Vector2(5f, 50f);

    public float minZoom = 5f;
    public float maxZoom = 50f;

    public Vector3 cameraPosition;

    public Vector3 cameraRotation;

    private Vector3 lastMousePosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchCameraMode();
        }
    }

    private void LateUpdate()
    {
        if (currentMode == CameraMode.FreeLook)
        {
            HandleMovement();
            HandleZoom();
            HandleRotation();
        }
        else if (currentMode == CameraMode.ThirdPerson)
        {
            RotateTPSCameraWithMouse();
        }
    }

    private void SwitchCameraMode()
    {
        currentMode = (currentMode == CameraMode.FreeLook) ? CameraMode.ThirdPerson : CameraMode.FreeLook;

        if (currentMode == CameraMode.FreeLook)
        {
            mainCamera.transform.position = cameraPosition;
            mainCamera.transform.rotation = Quaternion.Euler(cameraRotation);
        }
    }

    #region Free Look Camera
    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;

        if (Input.GetKey(KeyCode.E)) move += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) move -= Vector3.up;

        transform.position += move * moveSpeed * Time.deltaTime;

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, panLimitX.x, panLimitX.y),
            Mathf.Clamp(transform.position.y, panLimitY.x, panLimitY.y),
            Mathf.Clamp(transform.position.z, panLimitZ.x, panLimitZ.y)
        );

        cameraPosition = transform.position;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;

        Vector3 newPosition = transform.position + zoomDirection;
        newPosition.y = Mathf.Clamp(newPosition.y, minZoom, maxZoom);
        transform.position = newPosition;
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationX = delta.x * rotateSpeed * Time.deltaTime;
            float rotationY = -delta.y * rotateSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, rotationX, Space.World);
            transform.Rotate(Vector3.right, rotationY, Space.Self);

            lastMousePosition = Input.mousePosition;

            cameraRotation = transform.eulerAngles;
        }
    }
    #endregion

    #region Third Person Camera

    private void RotateTPSCameraWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * tpsSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * tpsSensitivityY;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        if (selectedVehicle != null)
        {
            Vector3 offset = new Vector3(0f, heightOffset, -15f);
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 desiredPosition = selectedVehicle.position + rotation * offset;

            // Add a smooth shake effect
            Vector3 shakeOffset = new Vector3(
                Mathf.PerlinNoise(Time.time * 2f, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * 2f) - 0.5f,
                Mathf.PerlinNoise(Time.time * 2f, Time.time * 2f) - 0.5f
            ) * 0.2f; // Adjust shake intensity here

            tpsCameraPosition.position = Vector3.Lerp(tpsCameraPosition.position, desiredPosition + shakeOffset, Time.deltaTime * moveSpeed);
            mainCamera.transform.position = tpsCameraPosition.position;
            mainCamera.transform.rotation = Quaternion.LookRotation((selectedVehicle.position + Vector3.up * heightOffset) - mainCamera.transform.position);

            // Gradually reduce shake effect over time
            shakeOffset *= 0.9f;
        }
    }
    #endregion
}

public enum CameraMode
{
    FreeLook,
    ThirdPerson
}