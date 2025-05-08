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

    private Tween healthWarningTween;
    private Tween energyWarningTween;
    private Tween cooldownWarningTween;


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

        HandleBarWarning(healthBarImage, healthBarHeight, ref healthWarningTween);
        HandleBarWarning(energyBarImage, energyBarHeight, ref energyWarningTween);
        HandleBarWarning(attackCooldownBarImage, attackCooldownBarHeight, ref cooldownWarningTween);
    }

    private void HandleBarWarning(Image barImage, float fillValue, ref Tween tweenRef)
    {
        if (fillValue < 0.2f)
        {
            if (tweenRef == null || !tweenRef.IsActive())
            {
                Color originalColor = barImage.color;
                tweenRef = barImage.DOColor(barImage.color, 0.3f)
                    .OnStart(() =>
                    {
                        barImage.DOFade(0.3f, 0.3f).SetLoops(-1, LoopType.Yoyo);
                    });
            }
        }
        else
        {
            if (tweenRef != null && tweenRef.IsActive())
            {
                tweenRef.Kill();
                barImage.DOKill();
                barImage.color = Color.white;
                barImage.canvasRenderer.SetAlpha(1f);
            }
        }
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
        PanelOpenAnimation(true);
    }

    public override void OnDeselect()
    {
        PanelOpenAnimation(false);
    }
    public override void OnClick()
    {
        vehicleSmallPanelObject.SetActive(!vehicleSmallPanelObject.activeSelf);
    }
}
