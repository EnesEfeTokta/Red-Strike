using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TpsUiController : MonoBehaviour
{
    [Header("UI References - Engine")]
    public Image engineValueBar;
    public TMP_Text engineValueText;
    public RectTransform engineValueTag;

    [Header("UI References - Fuel")]
    public Image fuelValueBar;
    public TMP_Text fuelValueText;

    [Header("UI References - Shield")]
    public Image shieldValueBar;
    public TMP_Text shieldValueText;

    [Header("UI References - Health")]
    public Image healthValueBar;
    public TMP_Text healthValueText;

    [Header("Engine Specific Settings")]
    public Color engineValueColorLow = Color.red;
    public Color engineValueColorHigh = Color.green;
    public Vector2 engineTagStartAnchoredPos = new Vector2(0, -50f);
    public Vector2 engineTagEndAnchoredPos = new Vector2(0, 50f);

    [Header("Warning Icon Settings")]
    public GameObject warningIconPrefab;
    [Range(0f, 1f)]
    public float warningThreshold = 0.2f;
    public float warningIconScaleMin = 0.8f;
    public float warningIconScaleMax = 1.2f;
    public float warningIconScaleSpeed = 2f;
    public Vector2 warningIconOffset = new Vector2(10, -10);

    [Header("Help Me Button Settings")]
    public Button helpMeButton;
    public GameObject helpIcon;
    public float helpIconScaleMin = 0.9f;
    public float helpIconScaleMax = 1.1f;
    public float helpIconScaleSpeed = 1.5f;
    public float helpIconRotationSpeed = 90f;

    [Header("Fire Cooldown Settings")]
    public Image fireCooldownBar;
    public Image fireCooldownBarBackground;
    public TMP_Text fireCooldownText;
    public float fireCooldownDuration = 3.0f;
    public GameObject fireCrosshair;

    [Header("Station/Repair Button Settings")]
    public Button stationButton;
    public GameObject stationArrowIcon;
    public Button repairButton;
    public GameObject repairArrowIcon;
    public float arrowMoveDistance = 10f;
    public float arrowMoveSpeed = 2f;
    public Vector3 arrowMoveDirection = Vector3.left;

    [Header("Current Values (For Inspector Testing)")]
    [Range(0, 100)] public float inspectorEngineValue;
    [Range(0, 100)] public float inspectorFuelValue;
    [Range(0, 100)] public float inspectorShieldValue;
    [Range(0, 100)] public float inspectorHealthValue;

    private float _engineValue; private float _fuelValue; private float _shieldValue; private float _healthValue;
    private class ActiveWarning { public GameObject IconInstance { get; set; } public Coroutine AnimationCoroutine { get; set; } }
    private Dictionary<Image, ActiveWarning> activeWarnings = new Dictionary<Image, ActiveWarning>();
    private bool isHelpActive = false; private Coroutine helpIconAnimationCoroutine;
    private Vector3 originalHelpIconScale; private Quaternion originalHelpIconRotation;
    private bool isFireCooldownActive = false; private float currentFireCooldownTime = 0f; private Coroutine fireCooldownCoroutine;

    private bool isStationArrowAnimating = false;
    private Coroutine stationArrowCoroutine;
    private Vector3 originalStationArrowLocalPos;

    private bool isRepairArrowAnimating = false;
    private Coroutine repairArrowCoroutine;
    private Vector3 originalRepairArrowLocalPos;

    public float EngineValue { get => _engineValue; set { float c = Mathf.Clamp(value, 0f, 100f); if (_engineValue != c) { _engineValue = c; inspectorEngineValue = _engineValue; UpdateEngineUI(_engineValue); } } }
    public float FuelValue { get => _fuelValue; set { float c = Mathf.Clamp(value, 0f, 100f); if (_fuelValue != c) { _fuelValue = c; inspectorFuelValue = _fuelValue; UpdateFuelUI(_fuelValue); } } }
    public float ShieldValue { get => _shieldValue; set { float c = Mathf.Clamp(value, 0f, 100f); if (_shieldValue != c) { _shieldValue = c; inspectorShieldValue = _shieldValue; UpdateShieldUI(_shieldValue); } } }
    public float HealthValue { get => _healthValue; set { float c = Mathf.Clamp(value, 0f, 100f); if (_healthValue != c) { _healthValue = c; inspectorHealthValue = _healthValue; UpdateHealthUI(_healthValue); } } }

    void Awake()
    {
        _engineValue = Mathf.Clamp(inspectorEngineValue, 0f, 100f);
        _fuelValue = Mathf.Clamp(inspectorFuelValue, 0f, 100f);
        _shieldValue = Mathf.Clamp(inspectorShieldValue, 0f, 100f);
        _healthValue = Mathf.Clamp(inspectorHealthValue, 0f, 100f);

        if (helpIcon != null) { originalHelpIconScale = helpIcon.transform.localScale; originalHelpIconRotation = helpIcon.transform.rotation; }

        if (stationArrowIcon != null) originalStationArrowLocalPos = stationArrowIcon.transform.localPosition;
        if (repairArrowIcon != null) originalRepairArrowLocalPos = repairArrowIcon.transform.localPosition;
    }

    void Start()
    {
        if (helpMeButton != null) helpMeButton.onClick.AddListener(ToggleHelpMode);
        if (fireCooldownBar != null) fireCooldownBar.gameObject.SetActive(false);
        if (fireCooldownBarBackground != null) fireCooldownBarBackground.gameObject.SetActive(false);
        if (fireCooldownText != null) fireCooldownText.gameObject.SetActive(false);
        if (fireCrosshair != null) fireCrosshair.SetActive(true);

        if (stationButton != null && stationArrowIcon != null)
        {
            stationButton.onClick.AddListener(ToggleStationArrowAnimation);
            stationArrowIcon.SetActive(false);
        }

        if (repairButton != null && repairArrowIcon != null)
        {
            repairButton.onClick.AddListener(ToggleRepairArrowAnimation);
            repairArrowIcon.SetActive(false);
        }


        UpdateEngineUI(_engineValue); UpdateFuelUI(_fuelValue); UpdateShieldUI(_shieldValue); UpdateHealthUI(_healthValue);
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            if (Mathf.Approximately(_engineValue, inspectorEngineValue) == false) EngineValue = inspectorEngineValue;
            if (Mathf.Approximately(_fuelValue, inspectorFuelValue) == false) FuelValue = inspectorFuelValue;
            if (Mathf.Approximately(_shieldValue, inspectorShieldValue) == false) ShieldValue = inspectorShieldValue;
            if (Mathf.Approximately(_healthValue, inspectorHealthValue) == false) HealthValue = inspectorHealthValue;
            if (Input.GetKeyDown(KeyCode.Space)) StartFireCooldown();
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            _engineValue = Mathf.Clamp(inspectorEngineValue, 0f, 100f);
            _fuelValue = Mathf.Clamp(inspectorFuelValue, 0f, 100f);
            _shieldValue = Mathf.Clamp(inspectorShieldValue, 0f, 100f);
            _healthValue = Mathf.Clamp(inspectorHealthValue, 0f, 100f);

            if (engineValueBar) UpdateEngineUI(_engineValue, false);
            if (fuelValueBar) UpdateFuelUI(_fuelValue, false);
            if (shieldValueBar) UpdateShieldUI(_shieldValue, false);
            if (healthValueBar) UpdateHealthUI(_healthValue, false);
        }
    }

    private void UpdateEngineUI(float cv, bool mwi = true)
    {
        if (engineValueBar == null || engineValueText == null)
            return;

        float f = cv / 100f; engineValueBar.fillAmount = f;
        engineValueText.text = $"{(int)cv}";
        engineValueBar.color = Color.Lerp(engineValueColorLow, engineValueColorHigh, f);

        if (engineValueTag != null) engineValueTag.anchoredPosition = Vector2.Lerp(engineTagStartAnchoredPos, engineTagEndAnchoredPos, f);
        if (mwi && Application.isPlaying) HandleWarningIcon(engineValueBar, cv);
    }

    private void UpdateFuelUI(float cv, bool mwi = true)
    {
        if (fuelValueBar == null || fuelValueText == null)
            return;

        float f = cv / 100f;
        fuelValueBar.fillAmount = f;
        fuelValueText.text = $"{(int)cv}";

        if (mwi && Application.isPlaying) HandleWarningIcon(fuelValueBar, cv);
    }

    private void UpdateShieldUI(float cv, bool mwi = true)
    {
        if (shieldValueBar == null || shieldValueText == null)
            return;

        float f = cv / 100f;
        shieldValueBar.fillAmount = f;
        shieldValueText.text = $"{(int)cv}";
        if (mwi && Application.isPlaying) HandleWarningIcon(shieldValueBar, cv);
    }

    private void UpdateHealthUI(float cv, bool mwi = true)
    {
        if (healthValueBar == null || healthValueText == null)
            return;

        float f = cv / 100f;
        healthValueBar.fillAmount = f;
        healthValueText.text = $"{(int)cv}";
        if (mwi && Application.isPlaying) HandleWarningIcon(healthValueBar, cv);
    }

    private void HandleWarningIcon(Image tb, float cv)
    {
        if (warningIconPrefab == null || tb == null)
            return;

        float cfp = cv / 100f;
        bool nw = cfp <= warningThreshold;
        if (activeWarnings.TryGetValue(tb, out ActiveWarning ew))
        {
            if (!nw)
            {
                if (ew.AnimationCoroutine != null) StopCoroutine(ew.AnimationCoroutine);
                if (ew.IconInstance != null) Destroy(ew.IconInstance);
                activeWarnings.Remove(tb);
            }
        }
        else
        {
            if (nw)
            {
                GameObject ii = Instantiate(warningIconPrefab);
                ii.name = $"{tb.name}_WarningIcon";
                RectTransform ir = ii.GetComponent<RectTransform>();
                if (ir != null)
                {
                    ii.transform.SetParent(tb.transform, false);
                    ir.anchorMin = new Vector2(1, 1);
                    ir.anchorMax = new Vector2(1, 1);
                    ir.pivot = new Vector2(1, 1);
                    ir.anchoredPosition = warningIconOffset;
                    ii.transform.localScale = Vector3.one;
                }
                Coroutine ac = StartCoroutine(AnimateWarningIconScaling(ii.transform));
                activeWarnings[tb] = new ActiveWarning { IconInstance = ii, AnimationCoroutine = ac };
            }
        }
    }

    IEnumerator AnimateWarningIconScaling(Transform it)
    {
        if (it == null)
            yield break;

        while (it != null)
        {
            float sm = Mathf.PingPong(Time.time * warningIconScaleSpeed, warningIconScaleMax - warningIconScaleMin) + warningIconScaleMin;
            it.localScale = Vector3.one * sm;
            yield return null;
        }
    }

    public void ToggleHelpMode()
    {
        if (helpIcon == null)
            return;

        isHelpActive = !isHelpActive;
        if (isHelpActive)
        {
            if (helpIconAnimationCoroutine != null) StopCoroutine(helpIconAnimationCoroutine);
            helpIconAnimationCoroutine = StartCoroutine(AnimateHelpIcon());
        }
        else
        {
            if (helpIconAnimationCoroutine != null)
            {
                StopCoroutine(helpIconAnimationCoroutine);
                helpIconAnimationCoroutine = null;
            }
            helpIcon.transform.localScale = originalHelpIconScale;
            helpIcon.transform.rotation = originalHelpIconRotation;
        }
    }

    IEnumerator AnimateHelpIcon()
    {
        if (helpIcon == null)
            yield break;

        while (isHelpActive)
        {
            if (helpIcon == null)
                yield break;

            float sm = Mathf.PingPong(Time.time * helpIconScaleSpeed, helpIconScaleMax - helpIconScaleMin) + helpIconScaleMin;
            helpIcon.transform.localScale = originalHelpIconScale * sm;
            helpIcon.transform.Rotate(0, 0, helpIconRotationSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }

    public void StartFireCooldown()
    {
        if (isFireCooldownActive)
        {
            return;
        }

        if (fireCooldownBar == null || fireCooldownText == null || fireCooldownBarBackground == null || fireCrosshair == null)
        {
            return;
        }

        if (fireCooldownCoroutine != null) StopCoroutine(fireCooldownCoroutine);

        fireCooldownCoroutine = StartCoroutine(FireCooldownProcess());
    }

    private IEnumerator FireCooldownProcess()
    {
        isFireCooldownActive = true; currentFireCooldownTime = 0f;
        fireCooldownBar.gameObject.SetActive(true);
        fireCooldownBarBackground.gameObject.SetActive(true);
        fireCooldownText.gameObject.SetActive(true);
        fireCrosshair.SetActive(false);

        while (currentFireCooldownTime < fireCooldownDuration)
        {
            currentFireCooldownTime += Time.deltaTime;
            float fa = currentFireCooldownTime / fireCooldownDuration;
            fireCooldownBar.fillAmount = fa;
            fireCooldownText.text = string.Format("{0:0}", currentFireCooldownTime);
            yield return null;
        }

        fireCooldownBar.fillAmount = 1f; fireCooldownText.text = $"{(int)fireCooldownDuration}";

        fireCooldownBar.gameObject.SetActive(false);
        fireCooldownBarBackground.gameObject.SetActive(false);
        fireCooldownText.gameObject.SetActive(false);
        fireCrosshair.SetActive(true);

        isFireCooldownActive = false;
        fireCooldownCoroutine = null;
    }

    public void ToggleStationArrowAnimation()
    {
        ToggleArrowAnimation(stationArrowIcon, ref isStationArrowAnimating, ref stationArrowCoroutine, originalStationArrowLocalPos);
        if (isStationArrowAnimating && isRepairArrowAnimating)
        {
            ToggleRepairArrowAnimation();
        }
    }

    public void ToggleRepairArrowAnimation()
    {
        ToggleArrowAnimation(repairArrowIcon, ref isRepairArrowAnimating, ref repairArrowCoroutine, originalRepairArrowLocalPos);
        if (isRepairArrowAnimating && isStationArrowAnimating)
        {
            ToggleStationArrowAnimation();
        }
    }

    private void ToggleArrowAnimation(GameObject arrowIcon, ref bool isAnimatingFlag, ref Coroutine animationCoroutine, Vector3 originalLocalPos)
    {
        if (arrowIcon == null) return;

        isAnimatingFlag = !isAnimatingFlag;

        if (isAnimatingFlag)
        {
            arrowIcon.SetActive(true);
            if (animationCoroutine != null) StopCoroutine(animationCoroutine);
            animationCoroutine = StartCoroutine(AnimateArrowMovement(arrowIcon.transform, originalLocalPos));
        }
        else
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
            arrowIcon.transform.localPosition = originalLocalPos;
            arrowIcon.SetActive(false);
        }
    }

    IEnumerator AnimateArrowMovement(Transform arrowTransform, Vector3 originalLocalPos)
    {
        if (arrowTransform == null) yield break;

        Vector3 normalizedMoveDirection = arrowMoveDirection.normalized;

        while (true)
        {
            if (arrowTransform == null) yield break;

            float movement = (Mathf.PingPong(Time.time * arrowMoveSpeed, arrowMoveDistance)) - (arrowMoveDistance / 2f);
            arrowTransform.localPosition = originalLocalPos + (normalizedMoveDirection * movement);

            yield return null;
        }
    }

    void OnDisable()
    {
        foreach (var entry in activeWarnings)
        {
            if (entry.Value != null)
            {
                if (entry.Value.AnimationCoroutine != null) StopCoroutine(entry.Value.AnimationCoroutine);
                if (entry.Value.IconInstance != null) Destroy(entry.Value.IconInstance);
            }
        }

        activeWarnings.Clear();

        if (isHelpActive && helpIcon != null)
        {
            if (helpIconAnimationCoroutine != null)
            {
                StopCoroutine(helpIconAnimationCoroutine); helpIconAnimationCoroutine = null;
            }

            helpIcon.transform.localScale = originalHelpIconScale;
            helpIcon.transform.rotation = originalHelpIconRotation;
        }

        if (fireCooldownCoroutine != null)
        {
            StopCoroutine(fireCooldownCoroutine);
            isFireCooldownActive = false;
            if (fireCooldownBar != null) fireCooldownBar.gameObject.SetActive(false);
            if (fireCooldownBarBackground != null) fireCooldownBarBackground.gameObject.SetActive(false);
            if (fireCooldownText != null) fireCooldownText.gameObject.SetActive(false);
            if (fireCrosshair != null) fireCrosshair.SetActive(true);
        }
        else if (fireCrosshair != null && !isFireCooldownActive)
        {
            fireCrosshair.SetActive(true);
        }

        if (isStationArrowAnimating && stationArrowIcon != null)
        {
            if (stationArrowCoroutine != null) StopCoroutine(stationArrowCoroutine);
            stationArrowIcon.transform.localPosition = originalStationArrowLocalPos;
            stationArrowIcon.SetActive(false);
        }
        if (isRepairArrowAnimating && repairArrowIcon != null)
        {
            if (repairArrowCoroutine != null) StopCoroutine(repairArrowCoroutine);
            repairArrowIcon.transform.localPosition = originalRepairArrowLocalPos;
            repairArrowIcon.SetActive(false);
        }
    }
}