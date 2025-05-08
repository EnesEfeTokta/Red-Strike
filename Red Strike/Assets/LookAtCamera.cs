using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main; // Cache the main camera reference
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform.position);
            transform.Rotate(0, 180, 0); // Rotate to face the camera
        }
    }
}
