using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaNew : MonoBehaviour
{
    [Header("=== ОСНОВНЫЕ ПАРАМЕТРЫ ===")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("=== СТАМИНА: ПОСТОЯННАЯ ТРАТА ===")]
    public float constantDrain = 0.2f;

    [Header("=== ТЕЛЕФОН ===")]
    public float maxBattery = 100f;
    public float currentBattery;
    public float batteryDrainRate = 5f;
    public float batteryRestoreRate = 10f;

    [Header("=== СТАМИНА: ТРАТА НА ТОЧКАХ ===")]
    public float liftDrain = 0.5f;
    public float eightDrain = 1.3f;
    public float promoDrain = 0f;
    public float perekDrain = 1.0f;
    public float navigDrain = 1.0f;
    public float kalizeumDrain = 1.0f;

    [Header("=== СТАМИНА: ВОССТАНОВЛЕНИЕ ===")]
    public float breakRestore = 5f;
    public float lookDownRestoreRate = 10f;

    [Header("=== ВЗГЛЯД ВНИЗ: ЗАДЕРЖКА ===")]
    public float lookDownDelay = 3f;
    public float lookDownAngleThreshold = 80f;
    private float lookDownTimer = 0f;

    [Header("=== СОСТОЯНИЯ ===")]
    public bool isLookingDown = false;
    public bool isAtWork = false;
    public bool isOnBreak = false;
    public string currentZoneType = "none";

    [Header("=== UI ===")]
    public Slider staminaSlider;
    public TextMeshProUGUI staminaText;
    public Slider batterySlider;
    public TextMeshProUGUI batteryText;
    public Slider lookDownDelaySlider;
    public TextMeshProUGUI drainRateText;  // ← ТЕКСТ ДЛЯ ТРАТЫ
    private float currentBurnoutMultiplier = 1f;

    void Start()
    {
        currentBurnoutMultiplier = GameManager.burnoutMultiplier;
        currentStamina = maxStamina;
        currentBattery = maxBattery;
        UpdateUI();
        UpdateBatteryUI();
        UpdateDrainRateUI();

        ApplyCharacterBuffs(CharacterSelect.selectedCharacter);

        Debug.Log($"[Stamina] Запущена. Стамина: {currentStamina}/{maxStamina}, Заряд: {currentBattery}/{maxBattery}");
    }


    void Update()
    {
        CheckLookDown();

        float totalDrain = 0f;

        if (isAtWork && !isOnBreak)
        {
            totalDrain = GetZoneDrain() * Time.deltaTime;
        }
        else
        {
            totalDrain = constantDrain * currentBurnoutMultiplier * Time.deltaTime;
        }

        if (isOnBreak)
        {
            currentStamina += breakRestore * Time.deltaTime;
            currentBattery += batteryRestoreRate * Time.deltaTime;
        }
        else if (isLookingDown && currentBattery > 0)
        {
            if (lookDownTimer < lookDownDelay)
            {
                lookDownTimer += Time.deltaTime;
            }
            else
            {
                currentStamina += lookDownRestoreRate * Time.deltaTime;
                currentBattery -= batteryDrainRate * Time.deltaTime;

                if (currentBattery <= 0)
                {
                    Debug.Log("[Телефон] Телефон разрядился!");
                    currentBattery = 0;
                }
            }
        }
        else
        {
            lookDownTimer = 0f;
            currentStamina -= totalDrain;

            if (lookDownDelaySlider != null && lookDownDelaySlider.gameObject.activeSelf)
                lookDownDelaySlider.gameObject.SetActive(false);

            if (lookDownDelaySlider != null)
                lookDownDelaySlider.value = 0;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

        UpdateUI();
        UpdateBatteryUI();
        UpdateDrainRateUI();

        if (currentStamina <= 0f)
        {
            Debug.Log("[Stamina] СТАМИНА КОНЧИЛАСЬ!");

            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                gm.ForceAddBonus();
            }

            // Костыль: вызываем гнев на 100%
            AngerSystem anger = FindAnyObjectByType<AngerSystem>();
            if (anger != null)
            {
                anger.AddDailyAnger(anger.maxDailyAnger);
            }

            enabled = false;
        }
    }

    void UpdateDrainRateUI()
    {
        if (drainRateText == null) return;

        float totalDrain = 0f;

        if (isAtWork && !isOnBreak)
        {
            totalDrain = GetZoneDrain();
        }
        else
        {
            totalDrain = constantDrain * currentBurnoutMultiplier;
        }

        drainRateText.text = $"ТРАТА: {totalDrain:F1}/сек";
    }

    public void SetMaxStamina(float newMax)
    {
        maxStamina = newMax;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (staminaSlider != null)
            staminaSlider.maxValue = maxStamina;

        UpdateUI();
        Debug.Log($"[Stamina] Max стамины изменён на {newMax}");
    }

    void CheckLookDown()
    {
        if (Camera.main != null)
        {
            float cameraAngleX = Camera.main.transform.localEulerAngles.x;

            if (cameraAngleX > 180f)
                cameraAngleX = 360f - cameraAngleX;

            bool canLookDown = currentBattery > 0;
            isLookingDown = cameraAngleX >= lookDownAngleThreshold && canLookDown;

            if (isLookingDown && lookDownTimer < lookDownDelay)
            {
                if (lookDownDelaySlider != null && !lookDownDelaySlider.gameObject.activeSelf)
                    lookDownDelaySlider.gameObject.SetActive(true);

                float progress = lookDownTimer / lookDownDelay;
                if (lookDownDelaySlider != null)
                    lookDownDelaySlider.value = progress;
            }
            else if (isLookingDown && lookDownTimer >= lookDownDelay)
            {
                if (lookDownDelaySlider != null && lookDownDelaySlider.gameObject.activeSelf)
                    lookDownDelaySlider.gameObject.SetActive(false);
            }
            else
            {
                if (lookDownDelaySlider != null && lookDownDelaySlider.gameObject.activeSelf)
                    lookDownDelaySlider.gameObject.SetActive(false);

                if (lookDownDelaySlider != null)
                    lookDownDelaySlider.value = 0;
            }
        }
    }

    float GetZoneDrain()
    {
        float baseDrain;
        switch (currentZoneType)
        {
            case "lift": baseDrain = liftDrain; break;
            case "eight": baseDrain = eightDrain; break;
            case "promo": baseDrain = promoDrain; break;
            case "perek": baseDrain = perekDrain; break;
            case "navig": baseDrain = navigDrain; break;
            case "kalizeum": baseDrain = kalizeumDrain; break;
            default: baseDrain = 1f; break;
        }
        // Умножаем на выгорание
        return baseDrain * currentBurnoutMultiplier;
    }

    public void SetZoneType(string zoneType)
    {
        currentZoneType = zoneType;
        Debug.Log($"[Stamina] Точка: {zoneType}, трата: {GetZoneDrain()}/сек");
    }

    public void UpdateUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        if (staminaText != null)
        {
            staminaText.text = $"Стамина: {currentStamina:F0}/{maxStamina}";
        }
    }

    public void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.maxValue = maxBattery;
            batterySlider.value = currentBattery;
        }

        if (batteryText != null)
        {
            batteryText.text = $"{currentBattery:F0}%";
        }
    }

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        UpdateUI();
        Debug.Log($"[Stamina] +{amount}, стало: {currentStamina:F0}");
    }

    public void ApplyCharacterBuffs(CharacterSelect.Character character)
    {
        ResetToDefault();

        Debug.Log($"[Stamina] Применяю бафф для персонажа: {character}");

        switch (character)
        {
            case CharacterSelect.Character.Morena:
                maxStamina = 120f;
                maxBattery = 120f;
                break;

            case CharacterSelect.Character.Radmir:
                lookDownRestoreRate = 15f;
                breakRestore = 7f;
                break;

            case CharacterSelect.Character.Dyrka:
                liftDrain *= 0.92f;
                eightDrain *= 0.92f;
                perekDrain *= 0.92f;
                navigDrain *= 0.92f;
                kalizeumDrain *= 0.92f;
                break;

            case CharacterSelect.Character.Svistik:
            case CharacterSelect.Character.Shell:
            case CharacterSelect.Character.MrPi:
            case CharacterSelect.Character.Kulich:
            case CharacterSelect.Character.Magomedova:
            default:
                break;
        }

        currentStamina = maxStamina;
        currentBattery = maxBattery;
        UpdateUI();
        UpdateBatteryUI();
    }

    private void ResetToDefault()
    {
        maxStamina = 100f;
        maxBattery = 100f;

        // ↓↓↓ ЗАКОММЕНТИРУЙ ЭТИ СТРОКИ ↓↓↓
        // constantDrain = 0.2f;
        // liftDrain = 0.5f;
        // eightDrain = 1.3f;
        // promoDrain = 0f;
        // perekDrain = 1.0f;
        // navigDrain = 1.0f;
        // kalizeumDrain = 1.0f;

        breakRestore = 5f;
        lookDownRestoreRate = 10f;
        batteryDrainRate = 5f;
        batteryRestoreRate = 10f;
        lookDownDelay = 3f;
    }
}