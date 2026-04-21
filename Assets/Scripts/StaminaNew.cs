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
        UpdateUI();
        Debug.Log($"[Stamina] Запущена. Стамина: {currentStamina}/{maxStamina}");
    }
    
    void Update()
{
    // --- ПРОВЕРКА ВЗГЛЯДА ВНИЗ ---
    CheckLookDown();
    
    // --- ПОСТОЯННАЯ ТРАТА (всегда) ---
    float totalDrain = constantDrain * Time.deltaTime;
    
    // --- ДОПОЛНИТЕЛЬНАЯ ТРАТА НА РАБОЧЕЙ ТОЧКЕ ---
    if (isAtWork && !isOnBreak)
    {
        float zoneDrain = GetZoneDrain();
        totalDrain += zoneDrain * Time.deltaTime;
    }
    
    // --- ВОССТАНОВЛЕНИЕ НА ПЕРЕРЫВЕ ---
    if (isOnBreak)
    {
        currentStamina += breakRestore * Time.deltaTime;
    }
    // --- НОВОЕ: ВОССТАНОВЛЕНИЕ ПРИ ВЗГЛЯДЕ ВНИЗ ---
    else if (isLookingDown)
    {
        // Задержка перед началом восстановления
        if (lookDownTimer < lookDownDelay)
        {
            lookDownTimer += Time.deltaTime;
        }
        else
        {
            currentStamina += lookDownRestoreRate * Time.deltaTime;
        }
    }
    else
    {
        // Если не смотрит вниз — сбрасываем таймер
        lookDownTimer = 0f;
        currentStamina -= totalDrain;
    }
    
    // Ограничиваем значения
    currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    
    // Обновляем UI
    UpdateUI();
    
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
        // Получаем угол поворота камеры по оси X
        float cameraAngleX = Camera.main.transform.localEulerAngles.x;
        
        // Нормализуем угол (если больше 180, то считаем отрицательный)
        if (cameraAngleX > 180f)
            cameraAngleX = 360f - cameraAngleX;
        
        // Проверяем, достиг ли угол порога
        // Смотрим вниз, когда угол X > lookDownAngleThreshold (например, 80 градусов)
        isLookingDown = cameraAngleX >= lookDownAngleThreshold;
        
        // Для отладки (можно убрать потом)
        if (isLookingDown && lookDownTimer < lookDownDelay)
        {
            Debug.Log($"[Stamina] Смотрю вниз... жду {lookDownDelay - lookDownTimer:F1} сек до восстановления");
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