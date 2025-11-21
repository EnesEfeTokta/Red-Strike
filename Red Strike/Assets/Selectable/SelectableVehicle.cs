/*
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SelectableVehicle : SelectableObject
{
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private Outline outline;

    public VehicleControlUI vehicleControlUI;

    private Tank tank;
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
        tank = GetComponent<Tank>();
        outline = GetComponent<Outline>();

        if (tank == null) Debug.LogError("SelectableVehicle bir Tank component'i bulamadı!", this);
        if (outline == null) Debug.LogWarning("SelectableVehicle bir Outline component'i bulamadı!", this);

        Selected = false;
        if (outline != null) outline.OutlineWidth = 0f;
    }

    private void Update()
    {
        if (CameraController.Instance == null || CameraController.Instance.currentMode != CameraMode.FreeLook || tank == null)
            return;

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

                if (hit.transform.IsChildOf(transform) || hit.transform == transform)
                {
                    if (isDoubleClick)
                    {
                        Selected = true;
                        if (outline != null) { outline.OutlineColor = Color.yellow; outline.OutlineWidth = 10f; }

                        tank.IsDoubleSelection = true;
                    }
                    else
                    {
                        Selected = true;
                        if (outline != null) { outline.OutlineColor = Color.yellow; outline.OutlineWidth = 5f; }

                        tank.IsSingleSelection = true;

                        if (vehicleControlUI != null) vehicleControlUI.OnSelect();
                    }
                }
                else if (hit.transform.CompareTag("Enemy"))
                {
                    if (tank.IsSingleSelection || tank.IsDoubleSelection)
                    {
                        GameObject clickedTarget = hit.transform.gameObject;
                        HandleTargetSelection(clickedTarget, isDoubleClick);
                    }
                }
                else
                {
                    if (tank.IsSingleSelection || tank.IsDoubleSelection)
                    {
                        Selected = false;
                        if (outline != null) outline.OutlineWidth = 0f;

                        tank.IsDoubleSelection = false;
                        tank.IsSingleSelection = false;
                        if (vehicleControlUI != null) vehicleControlUI.OnDeselect();
                    }

                    DeselectAllTargets();
                    tank.ClearAllFreeLookTargets();
                }
            }
            else
            {
                if (tank.IsSingleSelection || tank.IsDoubleSelection)
                {
                    Selected = false;
                    if (outline != null) outline.OutlineWidth = 0f;
                    tank.IsDoubleSelection = false;
                    tank.IsSingleSelection = false;
                    if (vehicleControlUI != null) vehicleControlUI.OnDeselect();
                }

                DeselectAllTargets();
                tank.ClearAllFreeLookTargets();
            }
        }
    }

    // Hedef seçme mantığını yönetir
    private void HandleTargetSelection(GameObject target, bool isDoubleClick)
    {
        bool isAlreadyPermanent = tank.PermanentFreeLookTargets.Contains(target.transform);
        bool isCurrentlySingle = tank.CurrentFreeLookTarget == target.transform;

        if (isDoubleClick)
        {
            if (isAlreadyPermanent)
            {
                DeselectTargetVisuals(target);
                tank.RemoveFreeLookTarget(target.transform);
            }
            else
            {
                if (isCurrentlySingle)
                {
                    DeselectTargetVisuals(target);
                    tank.SetFreeLookTarget(null);
                }

                SelectPermanentTargetVisuals(target);
                tank.AddFreeLookTarget(target.transform);
            }
        }
        else
        {
            if (isAlreadyPermanent)
            {
                DeselectTargetVisuals(target);
                tank.RemoveFreeLookTarget(target.transform);
            }
            else if (isCurrentlySingle)
            {
                DeselectTargetVisuals(target);
                tank.SetFreeLookTarget(null);
            }
            else
            {
                if (tank.CurrentFreeLookTarget != null)
                {
                    DeselectTargetVisuals(tank.CurrentFreeLookTarget.gameObject);
                }

                SelectTargetVisuals(target);
                tank.SetFreeLookTarget(target.transform);
            }
        }
    }

    /// <summary>
    /// Bir hedefin tekil seçildiğini gösteren görsel efekti (Outline) ayarlar.
    /// </summary>
    private void SelectTargetVisuals(GameObject target)
    {
        Outline targetOutline = target.GetComponent<Outline>();
        if (targetOutline != null)
        {
            targetOutline.OutlineWidth = 3f;
            targetOutline.OutlineColor = Color.red;
            targetOutline.enabled = true;
            targetOutline.On();
        }
    }

    /// <summary>
    /// Bir hedefin kalıcı seçildiğini gösteren görsel efekti (Outline) ayarlar.
    /// </summary>
    private void SelectPermanentTargetVisuals(GameObject target)
    {
        Outline targetOutline = target.GetComponent<Outline>();
        if (targetOutline != null)
        {
            targetOutline.OutlineWidth = 5f;
            targetOutline.OutlineColor = Color.red;
            targetOutline.enabled = true;
            targetOutline.On();
        }
    }

    /// <summary>
    /// Bir hedefin seçimini kaldıran görsel efekti (Outline) kapatır.
    /// </summary>
    private void DeselectTargetVisuals(GameObject target)
    {
        if (target == null) return;
        Outline targetOutline = target.GetComponent<Outline>();
        if (targetOutline != null)
        {
            targetOutline.OutlineWidth = 0f;
            targetOutline.enabled = false;
            targetOutline.Off();
        }
    }

    /// <summary>
    /// Tüm mevcut hedeflerin görsel seçim efektlerini kaldırır.
    /// </summary>
    private void DeselectAllTargets()
    {
        if (tank.CurrentFreeLookTarget != null)
        {
            DeselectTargetVisuals(tank.CurrentFreeLookTarget.gameObject);
        }

        List<Transform> targetsToDeselect = new List<Transform>(tank.PermanentFreeLookTargets);
        foreach (var targetTransform in targetsToDeselect)
        {
            if (targetTransform != null)
                DeselectTargetVisuals(targetTransform.gameObject);
        }
    }

}
*/
