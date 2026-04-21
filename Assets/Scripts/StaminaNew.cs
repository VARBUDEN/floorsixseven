using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaNew : MonoBehaviour
{
    // ==================== ОСНОВНЫЕ ПАРАМЕТРЫ ====================
    [Header("=== ОСНОВНЫЕ ПАРАМЕТРЫ ===")]
    public float maxStamina = 100f;
    public float currentStamina;

    // ==================== СТАМИНА (ТРАТА) ====================
    [Header("=== СТАМИНА: ПОСТОЯННАЯ ТРАТА ===")]
    public float constantDrain = 0.2f;

    [Header("=== СТАМИНА: ТРАТА НА ТОЧКАХ ===")]
    public float liftDrain = 0.5f;           // Лифт
    public float eightDrain = 1.3f;          // 8-ка
    public float promoDrain = 0f;            // Промо
    public float perekDrain = 1.0f;          // Перекрёсток
    public float navigDrain = 1.0f;          // Навигация
    public float kalizeumDrain = 1.0f;       // Кализеум

    // ==================== СТАМИНА (ВОССТАНОВЛЕНИЕ) ====================
    [Header("=== СТАМИНА: ВОССТАНОВЛЕНИЕ ===")]
    public float breakRestore = 5f;                      // на перерыве
    public float lookDownRestoreRate = 10f;              // при взгляде вниз

    // ==================== ВЗГЛЯД ВНИЗ (ЗАДЕРЖКА) ====================
    [Header("=== ВЗГЛЯД ВНИЗ: ЗАДЕРЖКА ===")]
    public float lookDownDelay = 3f;                     // задержка перед восстановлением
    public float lookDownAngleThreshold = 80f;           // угол для "взгляда вниз"
    private float lookDownTimer = 0f;                    // таймер задержки

    // ==================== ТЕЛЕФОН ====================
    [Header("=== ТЕЛЕФОН ===")]
    public float maxBattery = 100f;
    public float currentBattery;
    public float batteryDrainRate = 5f;                  // трата при взгляде вниз
    public float batteryRestoreRate = 10f;               // восстановление на перерыве

    // ==================== СОСТОЯНИЯ ====================
    [Header("=== СОСТОЯНИЯ (НЕ ТРОГАТЬ, ТОЛЬКО ДЛЯ ПРОГРАММИСТА) ===")]
    public bool isLookingDown = false;
    public bool isAtWork = false;
    public bool isOnBreak = false;
    public string currentZoneType = "none";

    // ==================== UI (ИНТЕРФЕЙС) ====================
    [Header("=== UI: СТАМИНА ===")]
    public Slider staminaSlider;
    public TextMeshProUGUI staminaText;

    [Header("=== UI: ТЕЛЕФОН ===")]
    public Slider batterySlider;
    public TextMeshProUGUI batteryText;

    [Header("=== UI: ЗАДЕРЖКА ВЗГЛЯДА ВНИЗ ===")]
    public Slider lookDownDelaySlider;

    // ==================== МЕТОДЫ ====================
    
    void Start()
    {
        currentStamina = maxStamina;
        currentBattery = maxBattery;

        UpdateUI();
        UpdateBatteryUI();

        Debug.Log($"[Stamina] Запущена. Стамина: {currentStamina}/{maxStamina}, Заряд: {currentBattery}/{maxBattery}");
    }

    void Update()
    {
        CheckLookDown();
        
        float totalDrain = constantDrain * Time.deltaTime;
        
        if (isAtWork && !isOnBreak)
        {
            float zoneDrain = GetZoneDrain();
            totalDrain += zoneDrain * Time.deltaTime;
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
        
        if (currentStamina <= 0f)
        {
            Debug.Log("[Stamina] СТАМИНА КОНЧИЛАСЬ! День окончен.");
            enabled = false;
        }
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
            
            if (cameraAngleX >= lookDownAngleThreshold && !canLookDown)
            {
                Debug.Log("[Телефон] Телефон разряжен! Зарядите его на перерыве.");
            }
        }
    }

    float GetZoneDrain()
    {
        switch (currentZoneType)
        {
            case "lift": return liftDrain;
            case "eight": return eightDrain;
            case "promo": return promoDrain;
            case "perek": return perekDrain;
            case "navig": return navigDrain;
            case "kalizeum": return kalizeumDrain;
            default: return 1f;
        }
    }

    public void SetZoneType(string zoneType)
    {
        currentZoneType = zoneType;
        Debug.Log($"[Stamina] Точка: {zoneType}, трата: {GetZoneDrain()}/сек");
    }

    void UpdateUI()
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

    void UpdateBatteryUI()
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
}