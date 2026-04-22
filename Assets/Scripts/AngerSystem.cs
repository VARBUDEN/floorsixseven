using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AngerSystem : MonoBehaviour
{
    [Header("=== ДНЕВНОЙ ГНЕВ (0-100) ===")]
    public float dailyAnger = 0f;
    public float maxDailyAnger = 100f;
    
    [Header("=== ОБЩИЙ ГНЕВ (0-5) ===")]
    public int totalAnger = 0;
    public int maxTotalAnger = 5;
    
    [Header("=== ПОПОЛНЕНИЕ ГНЕВА ===")]
    public float angerPerCatch = 20f;
    
    [Header("=== UI ===")]
    public Slider dailyAngerSlider;
    public TextMeshProUGUI dailyAngerText;
    public Image[] totalAngerIcons;  // массив иконок (звёзды)
    
    [Header("=== ЦВЕТА ===")]
    public Color normalColor = Color.white;
    public Color filledColor = Color.red;
    
    [Header("=== СОБЫТИЯ ===")]
    public System.Action OnDayEndEarly;
    
    void Start()
    {
        ResetDailyAnger();
        UpdateTotalAngerUI();
    }
    
    void Update()
    {
        UpdateDailyAngerUI();
    }
    
    public void AddDailyAnger(float amount)
    {
        dailyAnger += amount;
        dailyAnger = Mathf.Clamp(dailyAnger, 0f, maxDailyAnger);
        
        Debug.Log($"[Гнев] +{amount} дневного гнева. Всего: {dailyAnger}/{maxDailyAnger}");
        
        UpdateDailyAngerUI();
        
        // ПРОВЕРКА НА ДОСРОЧНЫЙ КОНЕЦ ДНЯ
        if (dailyAnger >= maxDailyAnger)
        {
            Debug.Log("[Гнев] ДОСТИГНУТ 100% ГНЕВА! День окончен досрочно.");
            EndDayEarly();
        }
    }
    
    void EndDayEarly()
    {
        // Добавляем +1 к общему гневу
        AddTotalAnger(1);
        
        // Вызываем событие
        OnDayEndEarly?.Invoke();
        
        // Останавливаем время или показываем сообщение
        Debug.Log("[Гнев] ДЕНЬ ЗАКОНЧЕН ДОСРОЧНО!");
    }
    
    public void AddTotalAnger(int amount)
    {
        totalAnger += amount;
        totalAnger = Mathf.Clamp(totalAnger, 0, maxTotalAnger);
        
        Debug.Log($"[Гнев] Общий гнев +{amount}. Теперь: {totalAnger}/{maxTotalAnger}");
        
        UpdateTotalAngerUI();
        
        if (totalAnger >= maxTotalAnger)
        {
            Debug.Log("[Гнев] ВАС УВОЛИЛИ!");
            // Здесь будет переход на экран увольнения
        }
    }
    
    public void ResetDailyAnger()
    {
        dailyAnger = 0f;
        UpdateDailyAngerUI();
        Debug.Log("[Гнев] Дневной гнев сброшен");
    }
    
    void UpdateDailyAngerUI()
    {
        if (dailyAngerSlider != null)
        {
            dailyAngerSlider.maxValue = maxDailyAnger;
            dailyAngerSlider.value = dailyAnger;
        }
        
        if (dailyAngerText != null)
        {
            dailyAngerText.text = $"ГНЕВ: {dailyAnger:F0}%";
        }
        
        // Меняем цвет полоски
        if (dailyAngerSlider != null)
        {
            Image fillImage = dailyAngerSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                if (dailyAnger < 30f)
                    fillImage.color = Color.green;
                else if (dailyAnger < 70f)
                    fillImage.color = Color.yellow;
                else
                    fillImage.color = Color.red;
            }
        }
    }
    
    void UpdateTotalAngerUI()
    {
        if (totalAngerIcons == null) return;
        
        for (int i = 0; i < totalAngerIcons.Length; i++)
        {
            if (totalAngerIcons[i] != null)
            {
                // Меняем цвет: красный если гнев есть, белый если нет
                totalAngerIcons[i].color = (i < totalAnger) ? filledColor : normalColor;
            }
        }
    }
}