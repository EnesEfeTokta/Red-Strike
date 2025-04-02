using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float zoomSpeed = 5f;
    public float rotateSpeed = 100f;
    public float verticalSpeed = 10f;

    public Vector2 panLimitX = new Vector2(-50f, 50f);
    public Vector2 panLimitZ = new Vector2(-50f, 50f);
    public Vector2 panLimitY = new Vector2(5f, 50f);

    public float minZoom = 5f;
    public float maxZoom = 50f;

    private Vector3 lastMousePosition;

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

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
        }
    }
}
