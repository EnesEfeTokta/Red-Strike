using UnityEngine;

public class SelectableVehicle : SelectableObject
{
    private GameObject selectedObject;
    private GameObject permanentlySelectedObject;
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
        outline.Off();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                bool isDoubleClick = (Time.time - lastClickTime) < doubleClickThreshold;
                lastClickTime = Time.time;

                if (hit.transform == transform)
                {
                    if (isDoubleClick)
                    {
                        permanentlySelectedObject = hit.transform.gameObject;
                        outline.On();
                        outline.OutlineColor = Color.yellow; // Change color to red for permanent selection
                        outline.OutlineWidth = 10f; // Increase outline width for permanent selection
                        Debug.Log("Permanently Selected: " + permanentlySelectedObject.name);
                    }
                    else
                    {
                        selectedObject = hit.transform.gameObject;
                        outline.On();
                        outline.OutlineColor = Color.yellow; // Change color to yellow for temporary selection
                        outline.OutlineWidth = 5f; // Normal outline width for temporary selection
                        Debug.Log("Temporarily Selected: " + selectedObject.name);
                    }
                }
                else
                {
                    if (isDoubleClick && permanentlySelectedObject != null)
                    {
                        Debug.Log("Permanently Deselected: " + permanentlySelectedObject.name);
                        outline.Off();
                        permanentlySelectedObject = null;
                    }
                    else if (!isDoubleClick && selectedObject != null)
                    {
                        Debug.Log("Temporarily Deselected: " + selectedObject.name);
                        outline.Off();
                        selectedObject = null;
                    }
                }
            }
        }
    }
}