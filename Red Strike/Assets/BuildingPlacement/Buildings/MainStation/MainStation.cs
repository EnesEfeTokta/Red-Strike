using UnityEngine;

public class MainStation : BuildingPlacement.Buildings.Building
{
    public Transform radarTransform;
    public float radarRotationSpeed = 20f;

    private void Update()
    {
        radarTransform.Rotate(Vector3.up, radarRotationSpeed * Time.deltaTime);
    }
}
