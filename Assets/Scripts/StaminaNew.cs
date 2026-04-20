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
    
    [Header("Состояния")]
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
        // --- ПОСТОЯННАЯ ТРАТА (всегда) ---
        float totalDrain = constantDrain * Time.deltaTime;
        
        // --- ТРАТА НА РАБОЧЕЙ ТОЧКЕ ---
        if (isAtWork && !isOnBreak)
        {
            float zoneDrain = GetZoneDrain();
            totalDrain += zoneDrain * Time.deltaTime;
        }
        
        // --- ВОССТАНОВЛЕНИЕ ---
        if (isOnBreak)
        {
            currentStamina += breakRestore * Time.deltaTime;
        }
        else
        {
            currentStamina -= totalDrain;
        }
        
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        UpdateUI();
        
        if (currentStamina <= 0f)
        {
            Debug.Log("[Stamina] СТАМИНА КОНЧИЛАСЬ!");
            enabled = false;
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