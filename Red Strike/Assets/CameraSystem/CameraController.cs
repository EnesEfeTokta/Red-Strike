using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Camera cam;

    [Header("Movement (WASD)")]
    public float moveSpeed = 40f;
    public float smoothTime = 0.1f;
    public Vector2 limitX = new Vector2(-100, 100);
    public Vector2 limitZ = new Vector2(-100, 100);

    [Header("Rotation (Right Click)")]
    public float rotationSpeed = 5f;
    public float rotationSmoothTime = 0.1f;

    [Header("Zoom (Scroll)")]
    public float zoomStep = 2f;
    public float minZoom = 5f;
    public float maxZoom = 40f;
    public float zoomSmoothTime = 0.2f;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float targetOrthoSize;

    private Vector3 currentVelocity;
    private float currentZoomVelocity;

    private Vector3 dragOrigin;
    private Vector3 dragCurrent;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    private void Start()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>();

        targetPosition = transform.position;
        targetRotation = transform.rotation;
        
        if (cam != null)
            targetOrthoSize = cam.orthographicSize;
    }

    private void Update()
    {
        HandleWASDMovement();
        HandleMouseDrag();
        HandleRotation();
        HandleZoom();

        ApplyTransform();
    }

    private void HandleWASDMovement()
    {
        if (Mouse.current.middleButton.isPressed) return;

        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1;
        if (Keyboard.current.sKey.isPressed) input.y -= 1;
        if (Keyboard.current.aKey.isPressed) input.x -= 1;
        if (Keyboard.current.dKey.isPressed) input.x += 1;

        if (input == Vector2.zero) return;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        float speed = Keyboard.current.leftShiftKey.isPressed ? moveSpeed * 2f : moveSpeed;

        targetPosition += moveDir * speed * Time.deltaTime;
        ClampTargetPosition();
    }

    private void HandleMouseDrag()
    {
        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (groundPlane.Raycast(ray, out float entry))
            {
                dragOrigin = ray.GetPoint(entry);
            }
        }

        if (Mouse.current.middleButton.isPressed)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (groundPlane.Raycast(ray, out float entry))
            {
                dragCurrent = ray.GetPoint(entry);
                Vector3 difference = dragOrigin - dragCurrent;
                targetPosition += difference;
                ClampTargetPosition();
            }
        }
    }

    private void HandleRotation()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            float rotationAmount = mouseX * rotationSpeed * Time.deltaTime;
            
            Vector3 currentEuler = targetRotation.eulerAngles;
            currentEuler.y += rotationAmount;
            
            targetRotation = Quaternion.Euler(currentEuler);
        }
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.y.ReadValue();

        if (scroll > 0) scroll = -1;
        else if (scroll < 0) scroll = 1;
        else scroll = 0;

        if (scroll != 0)
        {
            targetOrthoSize += scroll * zoomStep;
            targetOrthoSize = Mathf.Clamp(targetOrthoSize, minZoom, maxZoom);
        }
    }

    private void ClampTargetPosition()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, limitX.x, limitX.y);
        targetPosition.z = Mathf.Clamp(targetPosition.z, limitZ.x, limitZ.y);
    }

    private void ApplyTransform()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * (1f / rotationSmoothTime));

        if (cam != null)
        {
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetOrthoSize, ref currentZoomVelocity, zoomSmoothTime);
        }
    }
}