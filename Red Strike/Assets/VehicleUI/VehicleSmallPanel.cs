using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class VehicleSmallPanel : VehicleControlUI
{
    public GameObject vehicleSmallPanelObject;

    public Image healthBarImage;
    public Image energyBarImage;
    public Image attackCooldownBarImage;

    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;

    private float healthBarHeight;
    private float energyBarHeight;
    private float attackCooldownBarHeight;

    private void Start()
    {
        vehicleSmallPanelObject.SetActive(false);

        slot1Button.onClick.AddListener(Slot1ButtonClick);
        slot2Button.onClick.AddListener(Slot2ButtonClick);
        slot3Button.onClick.AddListener(Slot3ButtonClick);
    }

    private void PanelOpenAnimation(bool isOpen)
    {
        if (isOpen)
        {
            vehicleSmallPanelObject.SetActive(true);
            vehicleSmallPanelObject.transform.localScale = Vector3.zero;
            vehicleSmallPanelObject.transform.DOScale(new Vector3(0.008f, 0.01f, 0.01f), 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            vehicleSmallPanelObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => vehicleSmallPanelObject.SetActive(false));
        }
    }

    public override void UpdateUIProperties(float health, float energy, float attackCooldown, float maxHealth, float maxEnergy, float maxAttackCooldown)
    {
        healthBarHeight = health / maxHealth;
        energyBarHeight = energy / maxEnergy;
        attackCooldownBarHeight = attackCooldown / maxAttackCooldown;

        healthBarImage.DOKill();
        energyBarImage.DOKill();
        attackCooldownBarImage.DOKill();

        healthBarImage.DOFillAmount(healthBarHeight, 0.3f).SetEase(Ease.OutCubic);
        energyBarImage.DOFillAmount(energyBarHeight, 0.3f).SetEase(Ease.OutCubic);
        attackCooldownBarImage.DOFillAmount(attackCooldownBarHeight, 0.3f).SetEase(Ease.OutCubic);
    }

    private void Slot1ButtonClick()
    {
        // Handle Slot 1 button click
        Debug.Log("Slot 1 button clicked!");
    }

    private void Slot2ButtonClick()
    {
        // Handle Slot 2 button click
        Debug.Log("Slot 2 button clicked!");
    }

    private void Slot3ButtonClick()
    {
        // Handle Slot 3 button click
        Debug.Log("Slot 3 button clicked!");
    }

    public override void OnSelect()
    {
        base.OnSelect();
        PanelOpenAnimation(true);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        PanelOpenAnimation(false);
    }
    public override void OnClick()
    {
        base.OnClick();
        vehicleSmallPanelObject.SetActive(!vehicleSmallPanelObject.activeSelf);
    }
}
