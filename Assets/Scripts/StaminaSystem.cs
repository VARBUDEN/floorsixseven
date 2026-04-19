using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [Header("Параметры стамины")]
    public float maxStamina = 100f;
    public float currentStamina;
    
    [Header("Трата и восстановление")]
    public float drainRate = 1f;           // трата в секунду на точке
    public float breakRestoreRate = 5f;     // восстановление в секунду на перерыве
    
    private float defaultDrainRate;

    [Header("UI")]
    public Slider staminaSlider;
    public Image staminaFillImage;

    [Header("Цвета для полоски")]
    public Color healthyColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;
    
    [Header("Состояния")]
    public bool isOnBreak = false;          // на перерыве?
    public bool isAtWork = false;           // на рабочей точке?
    
    // События для других систем
    public System.Action OnStaminaEmpty;
    public System.Action<float> OnStaminaChanged;
    
    void Start()
    {
        defaultDrainRate = drainRate;

        // Начинаем день с полной стамины
        currentStamina = maxStamina;
        
        // Настроить UI, если он есть
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
        
        UpdateStaminaColor();
        Debug.Log($"[StaminaSystem] Инициализирован. Стамина: {currentStamina}/{maxStamina}");
    }

    public void SetWorkState(bool active, float zoneDrainRate = 0f)
    {
        isAtWork = active;
        if (active)
        {
            drainRate = zoneDrainRate > 0f ? zoneDrainRate : defaultDrainRate;
            Debug.Log($"[StaminaSystem] Игрок в рабочей зоне. Трата стамины: {drainRate}/сек");
        }
        else
        {
            drainRate = defaultDrainRate;
            Debug.Log("[StaminaSystem] Игрок вышел из рабочей зоны.");
        }
    }
    
    void Update()
    {
        // Трата стамины (на рабочей точке, не на перерыве)
        if (isAtWork && !isOnBreak)
        {
            SpendStamina(drainRate * Time.deltaTime);
            Debug.Log($"[StaminaSystem] Трата стамины: {drainRate * Time.deltaTime:F3}, текущая: {currentStamina:F1}");
        }
        else
        {
            Debug.Log($"[StaminaSystem] Нет траты. isAtWork: {isAtWork}, isOnBreak: {isOnBreak}, drainRate: {drainRate}");
        }
        
        // Восстановление стамины (на перерыве)
        if (isOnBreak)
        {
            RestoreStamina(breakRestoreRate * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Потратить стамину
    /// </summary>
    public void SpendStamina(float amount)
    {
        if (amount <= 0) return;
        
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        
        UpdateUI();
        
        // Если стамина кончилась
        if (currentStamina <= 0f)
        {
            Debug.Log("[StaminaSystem] СТАМИНА КОНЧИЛАСЬ! День окончен.");
            OnStaminaEmpty?.Invoke();
        }
    }
    
    /// <summary>
    /// Восстановить стамину
    /// </summary>
    public void RestoreStamina(float amount)
    {
        if (amount <= 0) return;
        
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        
        UpdateUI();
    }
    
    /// <summary>
    /// Мгновенное восстановление (для проебов)
    /// </summary>
    public void RestoreStaminaInstant(float amount)
    {
        RestoreStamina(amount);
        Debug.Log($"[StaminaSystem] Восстановлено +{amount}. Текущая: {currentStamina:F0}/{maxStamina}");
    }
    
    /// <summary>
    /// Обновить UI
    /// </summary>
    void UpdateUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
        
        UpdateStaminaColor();
        OnStaminaChanged?.Invoke(currentStamina);
    }
    
    /// <summary>
    /// Обновить цвет полоски в зависимости от уровня стамины
    /// </summary>
    void UpdateStaminaColor()
    {
        if (staminaFillImage == null) return;
        
        float percent = currentStamina / maxStamina;
        
        if (percent > 0.5f)
            staminaFillImage.color = healthyColor;
        else if (percent > 0.2f)
            staminaFillImage.color = warningColor;
        else
            staminaFillImage.color = dangerColor;
    }
    
    /// <summary>
    /// Сбросить стамину в начале дня
    /// </summary>
    public void ResetStamina()
    {
        currentStamina = maxStamina;
        UpdateUI();
        Debug.Log("[StaminaSystem] Стамина сброшена до максимума");
    }
    
    /// <summary>
    /// Изменить максимальную стамину (для персонажа Мурена)
    /// </summary>
    public void SetMaxStamina(float newMax)
    {
        maxStamina = newMax;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
        }
        
        UpdateUI();
        Debug.Log($"[StaminaSystem] Максимум стамины изменён на {maxStamina}");
    }
}