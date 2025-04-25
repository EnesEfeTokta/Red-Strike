using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SelectableVehicle : SelectableObject
{
    private GameObject selectedObject;
    private GameObject permanentlySelectedObject;
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;

    private Outline outline;

    public VehicleControlUI vehicleControlUI;

    private Tank tank;
    
    private GameObject currentlySelectedTarget { get; set; } = null;
    private List<GameObject> permanentTargets = new List<GameObject>();

    public List<GameObject> PermanentTargets
    {
        get { return permanentTargets; }
        set { permanentTargets = value; }
    }

    public GameObject CurrentlySelectedTarget
    {
        get { return currentlySelectedTarget; }
        set { currentlySelectedTarget = value; }
    }

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
        Selected = false;
    }

    private void Update()
    {
        if (CameraController.Instance.currentMode != CameraMode.FreeLook)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast ile tıklanan nesneyi kontrol ediyor.
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

                        tank.IsDoubleSelection = true;

                        vehicleControlUI.OnDeselect();
                    }
                    else
                    {
                        selectedObject = hit.transform.gameObject;
                        Selected = true;
                        outline.OutlineColor = Color.yellow;
                        outline.OutlineWidth = 5f;

                        tank.IsSingleSelection = true;

                        vehicleControlUI.OnSelect();
                    }
                }
                else
                {
                    if (!hit.transform.gameObject.CompareTag("Enemy")) // Eüer vurulan nesne "Enemy" tagına sahip değilse...
                    {
                        // Eğer araç seçilmişse, iptal etme işlemi yapılır.
                        // Eğer araç seçilmemişse, iptal etme işlemi yapılmaz.
                        if ((isDoubleClick && permanentlySelectedObject != null) || (!isDoubleClick && selectedObject != null))
                        {
                            Selected = false;
                            outline.OutlineWidth = 0f;
                            permanentlySelectedObject = null;
                            selectedObject = null;

                            tank.IsDoubleSelection = false;
                            tank.IsSingleSelection = false;
                        }

                        // Eğer hedef seçilmişse, iptal etme işlemi yapılır.
                        // Eğer hedef seçilmemişse, iptal etme işlemi yapılmaz.
                        if (currentlySelectedTarget != null)
                            DeselectTarget(currentlySelectedTarget);

                        currentlySelectedTarget = null;

                        foreach (var target in new List<GameObject>(permanentTargets))
                        {
                            DeselectTarget(target);
                        }
                        permanentTargets.Clear();

                        vehicleControlUI.OnDeselect();
                    }
                    else
                    {
                        if (selected) // Eğer araç seçilmişse hedef seçilebilir.
                        {
                            GameObject clickedTarget = hit.transform.gameObject;

                            bool isAlreadyPermanent = permanentTargets.Contains(clickedTarget);
                            bool isSingleSelected = currentlySelectedTarget == clickedTarget;

                            if (isDoubleClick)
                            {
                                if (isSingleSelected)
                                {
                                    DeselectTarget(clickedTarget);
                                }

                                if (isAlreadyPermanent)
                                {
                                    DeselectTarget(clickedTarget);
                                }
                                else
                                {
                                    SelectPermanentTarget(clickedTarget);
                                    permanentTargets.Add(clickedTarget);
                                }

                                currentlySelectedTarget = null;
                            }
                            else
                            {
                                if (isAlreadyPermanent)
                                {
                                    DeselectTarget(clickedTarget);
                                    permanentTargets.Remove(clickedTarget);
                                }
                                else
                                {
                                    // Önceki tekli hedefi kaldır
                                    if (currentlySelectedTarget != null)
                                        DeselectTarget(currentlySelectedTarget);

                                    SelectTarget(clickedTarget);
                                    currentlySelectedTarget = clickedTarget;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Hedefi seçmek için kullanılır. Seçilen hedefin outline'ını ayarlar ve önceki hedefi iptal eder.
    /// Eğer hedef zaten seçilmişse, iptal etme işlemi yapılmaz.
    /// </summary>
    /// <param name="target">Seçilecek hedef nesnesi.</param>
    private void SelectTarget(GameObject target)
    {
        if (currentlySelectedTarget != null && currentlySelectedTarget != target)
        {
            DeselectTarget(currentlySelectedTarget);
        }

        Outline targetOutline = target.GetComponent<Outline>();
        if (targetOutline != null)
        {
            targetOutline.OutlineWidth = 3f;
            targetOutline.OutlineColor = Color.red;
            targetOutline.On();
        }

        currentlySelectedTarget = target;
    }

    private void SelectPermanentTarget(GameObject target)
    {
        Outline targetOutline = target.GetComponent<Outline>();
        if (targetOutline != null)
        {
            targetOutline.OutlineWidth = 5f;
            targetOutline.OutlineColor = Color.red;
            targetOutline.On();
        }
    }


    /// <summary>
    /// Hedefi iptal etmek için kullanılır. Seçilen hedefin outline'ını kapatır ve referansı temizler.
    /// Eğer hedef iptal edilirse, currentlySelectedTarget null olarak ayarlanır.
    /// </summary>
    /// <param name="target">Iptal edilecek hedef nesnesi.</param>
    private void DeselectTarget(GameObject target)
    {
        Outline targetOutline = target.GetComponent<Outline>();
        if (targetOutline != null)
        {
            targetOutline.Off();
        }

        if (target == currentlySelectedTarget)
        {
            currentlySelectedTarget = null;
        }

        if (permanentTargets.Contains(target))
        {
            permanentTargets.Remove(target);
        }
    }
}
