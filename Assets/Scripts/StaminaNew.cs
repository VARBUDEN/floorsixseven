using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StaminaNew : MonoBehaviour
{
    [Header("Основные параметры")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Постоянная трата (всегда)")]
    public float constantDrain = 0.2f;      // усталость даже вне точек

    [Header("Телефон")]
    public float maxBattery = 100f;           // максимальный заряд
    public float currentBattery;              // текущий заряд
    public float batteryDrainRate = 5f;       // трата заряда в секунду при взгляде вниз
    public float batteryRestoreRate = 10f;    // восстановление заряда на перерыве

    [Header("UI Телефона")]
    public Slider batterySlider;
    public TextMeshProUGUI batteryText;

    [Header("Трата на разных точках")]
    public float liftDrain = 0.5f;           // Лифт
    public float eightDrain = 1.3f;          // 8-ка
    public float promoDrain = 0f;            // Промо (траты нет)
    public float perekDrain = 1.0f;          // Перекрёсток
    public float navigDrain = 1.0f;          // Навигация
    public float kalizeumDrain = 1.0f;       // Кализеум

    [Header("Восстановление")]
    public float breakRestore = 5f;          // восстановление на перерыве

    [Header("Восстановление при взгляде вниз")]
    public float lookDownRestoreRate = 10f;     // сколько восстанавливаем в секунду
    public float lookDownDelay = 3f;            // задержка перед восстановлением (сек)
    public float lookDownAngleThreshold = 80f;   // угол при котором считается "взгляд вниз" (80 градусов)

    [Header("Состояния")]
    public bool isLookingDown = false;           // смотрит ли игрок вниз
    private float lookDownTimer = 0f;            // таймер для задержки
    public bool isAtWork = false;
    public bool isOnBreak = false;
    public string currentZoneType = "none";

    [Header("UI")]
    public Slider staminaSlider;
    public TextMeshProUGUI staminaText;

    void Start()
    {
        currentStamina = maxStamina;
        currentBattery = maxBattery;  // телефон полностью заряжен

        UpdateUI();
        UpdateBatteryUI();

        Debug.Log($"[Stamina] Запущена. Стамина: {currentStamina}/{maxStamina}, Заряд: {currentBattery}/{maxBattery}");
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
    void Update()
    {
        // --- ПРОВЕРКА ВЗГЛЯДА ВНИЗ ---
        CheckLookDown();

        // --- ПОСТОЯННАЯ ТРАТА СТАМИНЫ ---
        float totalDrain = constantDrain * Time.deltaTime;

        if (isAtWork && !isOnBreak)
        {
            float zoneDrain = GetZoneDrain();
            totalDrain += zoneDrain * Time.deltaTime;
        }

        // --- ВОССТАНОВЛЕНИЕ НА ПЕРЕРЫВЕ ---
        if (isOnBreak)
        {
            // Восстанавливаем стамину
            currentStamina += breakRestore * Time.deltaTime;

            // Восстанавливаем заряд телефона на перерыве
            currentBattery += batteryRestoreRate * Time.deltaTime;
        }
        // --- ВОССТАНОВЛЕНИЕ ПРИ ВЗГЛЯДЕ ВНИЗ ---
        else if (isLookingDown && currentBattery > 0)
        {
            // Задержка перед началом восстановления
            if (lookDownTimer < lookDownDelay)
            {
                lookDownTimer += Time.deltaTime;
            }
            else
            {
                // Восстанавливаем стамину
                currentStamina += lookDownRestoreRate * Time.deltaTime;

                // Тратим заряд телефона
                currentBattery -= batteryDrainRate * Time.deltaTime;

                if (currentBattery <= 0)
                {
                    Debug.Log("[Телефон] Телефон разрядился! Нужно зарядить на перерыве.");
                    currentBattery = 0;
                    isLookingDown = false;  // больше не смотрим вниз, пока не зарядим
                }
            }
        }
        else
        {
            // Сбрасываем таймер, если не смотрим вниз
            lookDownTimer = 0f;
            currentStamina -= totalDrain;
        }

        // Ограничиваем значения
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

        // Обновляем UI
        UpdateUI();
        UpdateBatteryUI();

        // Проверка на конец дня
        if (currentStamina <= 0f)
        {
            Debug.Log("[Stamina] СТАМИНА КОНЧИЛАСЬ! День окончен.");
            enabled = false;
        }
    }

    /// <summary>
    /// Проверяет, смотрит ли игрок вниз (угол камеры по X)
    /// </summary>
void CheckLookDown()
{
    if (Camera.main != null)
    {
        float cameraAngleX = Camera.main.transform.localEulerAngles.x;
        
        if (cameraAngleX > 180f)
            cameraAngleX = 360f - cameraAngleX;
        
        // Смотрим вниз только если есть заряд телефона
        bool canLookDown = currentBattery > 0;
        isLookingDown = cameraAngleX >= lookDownAngleThreshold && canLookDown;
        
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
            default: return 1f; // если тип не задан — стандартная трата
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

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        UpdateUI();
        Debug.Log($"[Stamina] +{amount}, стало: {currentStamina:F0}");
    }
}