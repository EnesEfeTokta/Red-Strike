using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        private InputSystem_Actions input;
        private Vector2 moveInput;
        private float verticalInput;

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

            input = new InputSystem_Actions();
        }

        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();

        private void Start()
        {
            targetPosition = transform.position;

            Vector3 eulerAngles = transform.rotation.eulerAngles;
            currentYaw = eulerAngles.y;
            currentPitch = eulerAngles.x;

            if (currentPitch > 180f)
                currentPitch -= 360f;

            // WASD (Vector2)
            input.Player.CameraMove.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            input.Player.CameraMove.canceled += ctx => moveInput = Vector2.zero;

            // Q/E (float)
            input.Player.CameraVertical.performed += ctx => verticalInput = ctx.ReadValue<float>();
            input.Player.CameraVertical.canceled += ctx => verticalInput = 0f;
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
            float speedMultiplier = Keyboard.current.leftShiftKey.isPressed ? fastMoveMultiplier : 1f;

            Vector3 move =
                transform.forward * moveInput.y +
                transform.right * moveInput.x +
                Vector3.up * verticalInput;

            targetPosition += move * moveSpeed * speedMultiplier * Time.deltaTime;

            targetPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, panLimitX.x, panLimitX.y),
                Mathf.Clamp(targetPosition.y, panLimitY.x, panLimitY.y),
                Mathf.Clamp(targetPosition.z, panLimitZ.x, panLimitZ.y)
            );
        }

        private void HandleZoom()
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scroll) > 0.1f)
            {
                Vector3 zoomDir = transform.forward * (scroll / 120f) * zoomSpeed;
                targetPosition += zoomDir;
                targetPosition.y = Mathf.Clamp(targetPosition.y, minZoom, maxZoom);
            }
        }

        private void HandleRotation()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                lastMousePosition = Mouse.current.position.ReadValue();

            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector2 delta = mousePos - (Vector2)lastMousePosition;

                currentYaw += delta.x * rotateSpeed * Time.deltaTime;
                currentPitch -= delta.y * rotateSpeed * Time.deltaTime;
                currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

                lastMousePosition = mousePos;
            }
        }

        private void ApplySmoothMovement()
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            Quaternion targetRot = Quaternion.Euler(currentPitch, currentYaw, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smoothRotation);
        }
    }
}
