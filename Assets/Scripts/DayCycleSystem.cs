using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayCycleSystem : MonoBehaviour
{
    public static int LastDays = 1;
    public static int LastSalary = 0;
    
    [Header("=== НАСТРОЙКИ ДНЯ ===")]
    public int currentDay = 1;
    public int startHour = 10;
    public int endHour = 22;
    public float secondsPerHour = 60f;
    
    [Header("=== ЗАРПЛАТА ===")]
    public float baseHourlyRate = 10f;
    public float fullDayBonus = 50f;
    
    [Header("=== БОНУСЫ ЗА ТОЧКИ ===")]
    public float liftBonus = 0f;
    public float eightBonus = 15f;
    public float promoBonus = 10f;
    public float perekBonus = 5f;
    public float navigBonus = 0f;
    public float kalizeumBonus = 20f;
    
    [Header("=== ТЕКУЩИЕ ЗНАЧЕНИЯ ===")]
    public float currentTime = 10f;
    public float currentDaySalary = 0f;
    public float totalSalary = 0f;
    public bool isDayActive = true;
    
    [Header("=== UI ===")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public Slider timeProgressSlider;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI hourlyRateText;
    
    [Header("=== UI КОНЦА ДНЯ ===")]
    public GameObject endDayPanel;
    public TextMeshProUGUI endDayText;
    public Button nextDayButton;
    
    private StaminaNew stamina;
    private AngerSystem angerSystem;
    private float timePerSecond;
    private int currentHour = 10;
    private int currentMinute = 0;
    
void Start()
{
    // СБРАСЫВАЕМ СТАТИЧЕСКИЕ ПЕРЕМЕННЫЕ ПРИ НОВОМ ЗАБЕГЕ
    LastDays = 1;
    LastSalary = 0;
    totalSalary = 0f;
    currentDaySalary = 0f;
    currentDay = 1;
    
    stamina = FindAnyObjectByType<StaminaNew>();
    angerSystem = FindAnyObjectByType<AngerSystem>();
    timePerSecond = 1f / secondsPerHour;
    
    currentTime = startHour;
    currentHour = startHour;
    currentMinute = 0;
    isDayActive = true;
    
    if (endDayPanel != null)
        endDayPanel.SetActive(false);
    
    if (nextDayButton != null)
        nextDayButton.onClick.AddListener(OnNextDayButton);
    
    UpdateAllUI();
    UpdateStaticData();
}
    
    void Update()
    {
        if (!isDayActive) return;
        if (stamina != null && stamina.currentStamina <= 0) return;
        
        currentTime += timePerSecond * Time.deltaTime;
        
        if (currentTime >= endHour)
        {
            currentTime = endHour;
            EndDay();
            return;
        }
        
        UpdateTimeDisplay();
        
        int newHour = Mathf.FloorToInt(currentTime);
        if (newHour > currentHour)
        {
            currentHour = newHour;
            AddHourlySalary();
        }
        
        currentMinute = Mathf.FloorToInt((currentTime - currentHour) * 60);
    }
    
    public void EndDayEarly()
    {
        isDayActive = false;
        Debug.Log($"[День {currentDay}] День закончен досрочно из-за гнева!");
        ShowEndDayPanel("ДОСРОЧНО");
    }
    
    void ShowEndDayPanel(string reason)
    {
        if (endDayPanel != null)
        {
            endDayPanel.SetActive(true);
            
            if (endDayText != null)
            {
                endDayText.text = $"ДЕНЬ {currentDay} ЗАКОНЧЕН {reason}\n\n"
                                + $"Зарплата за день: {LastSalary}\n"
                                + $"Общая зарплата: {totalSalary + currentDaySalary}";
            }
            
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    public void OnNextDayButton()
    {
        if (endDayPanel != null)
            endDayPanel.SetActive(false);
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        NextDay();
    }
    
    void AddHourlySalary()
    {
        float bonus = GetZoneBonus();
        float hourlySalary = baseHourlyRate + bonus;
        
        currentDaySalary += hourlySalary;
        LastDays = currentDay;
        LastSalary = Mathf.RoundToInt(currentDaySalary);
        
        Debug.Log($"[День {currentDay}] Час {currentHour}:00 +{hourlySalary:F0}. Всего за день: {LastSalary}");
        
        UpdateSalaryUI();
        UpdateHourlyRateUI();
    }
    
    float GetZoneBonus()
    {
        if (stamina == null) return 0f;
        
        switch (stamina.currentZoneType)
        {
            case "lift": return liftBonus;
            case "eight": return eightBonus;
            case "promo": return promoBonus;
            case "perek": return perekBonus;
            case "navig": return navigBonus;
            case "kalizeum": return kalizeumBonus;
            default: return 0f;
        }
    }
    
    void EndDay()
    {
        isDayActive = false;
        currentDaySalary += fullDayBonus;
        LastSalary = Mathf.RoundToInt(currentDaySalary);
        
        Debug.Log($"[День {currentDay}] День окончен! Итого за день: {LastSalary}");
        UpdateSalaryUI();
        ShowEndDayPanel("");
    }
    
    void UpdateStaticData()
    {
        LastDays = currentDay;
        LastSalary = Mathf.RoundToInt(currentDaySalary);
    }
    
    public void NextDay()
    {
        currentDay++;
        currentDaySalary = 0f;
        currentTime = startHour;
        currentHour = startHour;
        currentMinute = 0;
        isDayActive = true;
        
        if (stamina != null)
        {
            stamina.currentStamina = stamina.maxStamina * 0.8f;
            stamina.currentBattery = stamina.maxBattery * 0.8f;
            stamina.UpdateUI();
            stamina.UpdateBatteryUI();
        }
        
        if (angerSystem != null)
        {
            angerSystem.ResetDailyAnger();
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(0, 1, 0);
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = new Vector3(0, 1, 0);
                controller.enabled = true;
            }
        }
        
        UpdateAllUI();
        UpdateStaticData();
        Debug.Log($"[День {currentDay}] Новый день начался!");
    }
    
    void UpdateTimeDisplay()
    {
        if (timeText != null)
            timeText.text = $"{currentHour:00}:{currentMinute:00}";
        
        if (timeProgressSlider != null)
        {
            float progress = (currentTime - startHour) / (endHour - startHour);
            timeProgressSlider.value = Mathf.Clamp01(progress);
        }
    }
    
    void UpdateSalaryUI()
    {
        if (salaryText != null)
            salaryText.text = $"$ {currentDaySalary:F0}";
    }
    
    void UpdateHourlyRateUI()
    {
        if (hourlyRateText != null)
        {
            float totalRate = baseHourlyRate + GetZoneBonus();
            hourlyRateText.text = $"$/ч: {totalRate:F0}";
        }
    }
    
    void UpdateAllUI()
    {
        if (dayText != null)
            dayText.text = $"ДЕНЬ {currentDay}";
        
        UpdateTimeDisplay();
        UpdateSalaryUI();
        UpdateHourlyRateUI();
    }
    
    public void ResetForNewDay()
    {
        currentDaySalary = 0f;
        currentTime = startHour;
        currentHour = startHour;
        currentMinute = 0;
        isDayActive = true;
        UpdateAllUI();
        UpdateStaticData();
    }
    
    public float GetCurrentDaySalary()
    {
        return currentDaySalary;
    }
    
    public float GetTotalSalary()
    {
        return totalSalary;
    }
}