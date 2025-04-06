using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableVehicle : SelectableObject
{
    private GameObject selectedObject;
    private GameObject permanentlySelectedObject;
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private Outline outline;

    public VehicleControlUI vehicleControlUI;

    private bool selected = false;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if (value)
            {
                outline.On();
            }
            else
            {
                outline.Off();
            }
        }
    }

    private void Start()
    {
        outline = GetComponent<Outline>();
        Selected = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                bool isDoubleClick = (Time.time - lastClickTime) < doubleClickThreshold;
                lastClickTime = Time.time;

                if (hit.transform.IsChildOf(transform))
                {
                    if (isDoubleClick)
                    {
                        permanentlySelectedObject = hit.transform.gameObject;
                        Selected = true;
                        outline.OutlineColor = Color.yellow;
                        outline.OutlineWidth = 10f;
                        Debug.Log("Permanently Selected: " + permanentlySelectedObject.name);

                        vehicleControlUI.OnDeselect();
                    }
                    else
                    {
                        selectedObject = hit.transform.gameObject;
                        Selected = true;
                        outline.OutlineColor = Color.yellow;
                        outline.OutlineWidth = 5f;
                        Debug.Log("Temporarily Selected: " + selectedObject.name);

                        vehicleControlUI.OnSelect();
                    }
                }
                else
                {
                    if (isDoubleClick && permanentlySelectedObject != null)
                    {
                        Debug.Log("Permanently Deselected: " + permanentlySelectedObject.name);
                        Selected = false;
                        permanentlySelectedObject = null;
                    }
                    else if (!isDoubleClick && selectedObject != null)
                    {
                        Debug.Log("Temporarily Deselected: " + selectedObject.name);
                        Selected = false;
                        selectedObject = null;
                    }

                    vehicleControlUI.OnDeselect();
                }
            }
        }
    }
}
