using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Movement Settings")]
    public float moveSpeed = 20f;
    public float fastMoveMultiplier = 2f;
    public float smoothTime = 0.1f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    [Header("Rotation Settings")]
    public float rotateSpeed = 100f;
    public float smoothRotation = 5f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Bounds")]
    public Vector2 panLimitX = new Vector2(-50f, 50f);
    public Vector2 panLimitZ = new Vector2(-50f, 50f);
    public Vector2 panLimitY = new Vector2(5f, 50f);

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private Vector3 lastMousePosition;
    private float currentYaw;
    private float currentPitch;

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
        targetPosition = transform.position;
        
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        currentYaw = eulerAngles.y;
        currentPitch = eulerAngles.x;
        
        if (currentPitch > 180f)
            currentPitch -= 360f;
    }

    private void LateUpdate()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
        ApplySmoothMovement();
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;
        float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? fastMoveMultiplier : 1f;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.E)) move += Vector3.up;
        if (Input.GetKey(KeyCode.Q)) move -= Vector3.up;

        targetPosition += move * moveSpeed * speedMultiplier * Time.deltaTime;

        targetPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, panLimitX.x, panLimitX.y),
            Mathf.Clamp(targetPosition.y, panLimitY.x, panLimitY.y),
            Mathf.Clamp(targetPosition.z, panLimitZ.x, panLimitZ.y)
        );
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;
            targetPosition += zoomDirection;
            targetPosition.y = Mathf.Clamp(targetPosition.y, minZoom, maxZoom);
        }
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
            
            currentYaw += delta.x * rotateSpeed * Time.deltaTime;
            currentPitch -= delta.y * rotateSpeed * Time.deltaTime;
            
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            
            lastMousePosition = Input.mousePosition;
        }
    }

    private void ApplySmoothMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        
        Quaternion targetRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothRotation);
    }
}
