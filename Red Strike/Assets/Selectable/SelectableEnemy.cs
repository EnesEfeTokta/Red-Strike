/*
using PlayFab.EconomyModels;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableEnemy : SelectableObject
{
    private Outline outline;
    private GameObject selectedObject;
    private GameObject permanentlySelectedObject;
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private Tank tank;

    private void Start()
    {
        tank = GetComponent<Tank>();
        outline = GetComponent<Outline>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && (tank.IsSingleSelection || tank.IsDoubleSelection))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Enemy"))
                {
                    print("Hit an enemy: " + hit.transform.gameObject.name);
                }

                bool isDoubleClick = (Time.time - lastClickTime) < doubleClickThreshold;
                lastClickTime = Time.time;

                if (hit.transform.IsChildOf(transform))
                {
                    if (isDoubleClick)
                    {
                        permanentlySelectedObject = hit.transform.gameObject;
                        outline.OutlineColor = Color.red;
                        outline.OutlineWidth = 10f;
                    }
                    else
                    {
                        selectedObject = hit.transform.gameObject;
                        outline.OutlineColor = Color.red;
                        outline.OutlineWidth = 5f;
                    }
                }
                else
                {
                    if ((isDoubleClick && permanentlySelectedObject != null) || (!isDoubleClick && selectedObject != null))
                    {
                        outline.OutlineWidth = 0f;
                        permanentlySelectedObject = null;
                    }
                }
            }
        }
    }
}
*/
