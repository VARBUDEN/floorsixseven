using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    
    [Header("=== РАСПИСАНИЕ ===")]
    public List<ScheduleSlot> dailySchedule = new List<ScheduleSlot>();
    public ScheduleSlot currentSlot;
    public ScheduleSlot nextSlot;
    
    [Header("=== UI ===")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public Slider timeProgressSlider;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI hourlyRateText;
    
    [Header("=== UI РАСПИСАНИЯ (HUD) ===")]
    public TextMeshProUGUI currentZoneText;
    public TextMeshProUGUI currentZoneTimerText;
    public TextMeshProUGUI nextZoneText;
    
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
        
        GenerateSchedule();
        UpdateAllUI();
        UpdateStaticData();
    }
    
    void GenerateSchedule()
    {
        dailySchedule.Clear();
        
        List<TimeSlot> timeSlots = new List<TimeSlot>
        {
            new TimeSlot(10.0f, 11.333f, "Рабочая точка"),
            new TimeSlot(11.333f, 11.667f, "Перерыв"),
            new TimeSlot(11.667f, 12.333f, "Рабочая точка"),
            new TimeSlot(12.333f, 13.0f, "Рабочая точка"),
            new TimeSlot(13.0f, 13.667f, "Обед"),
            new TimeSlot(13.667f, 14.333f, "Рабочая точка"),
            new TimeSlot(14.333f, 15.667f, "Рабочая точка"),
            new TimeSlot(15.667f, 16.333f, "Рабочая точка"),
            new TimeSlot(16.333f, 16.667f, "Перерыв"),
            new TimeSlot(16.667f, 17.333f, "Рабочая точка"),
            new TimeSlot(17.333f, 18.0f, "Рабочая точка"),
            new TimeSlot(18.0f, 18.667f, "Ужин"),
            new TimeSlot(18.667f, 19.333f, "Рабочая точка"),
            new TimeSlot(19.333f, 20.0f, "Рабочая точка"),
            new TimeSlot(20.0f, 20.333f, "Перерыв"),
            new TimeSlot(20.333f, 22.0f, "Рабочая точка")
        };
        
        string[] workZones = { "lift", "perek", "promo", "eight", "navig" };
        string[] workZoneNames = { "Лифт", "Перекрёсток", "Промо", "8-ка", "Навигация" };
        int[] zoneWeights = { 10, 8, 7, 3, 2 };
        
        float avgReputation = 0;
        ReputationSystem repSystem = FindAnyObjectByType<ReputationSystem>();
        if (repSystem != null)
        {
            avgReputation = repSystem.GetAverageReputation();
        }
        
        foreach (TimeSlot slot in timeSlots)
        {
            ScheduleSlot scheduleSlot = new ScheduleSlot();
            scheduleSlot.startTime = slot.startTime;
            scheduleSlot.endTime = slot.endTime;
            scheduleSlot.slotType = slot.slotType;
            
            if (slot.slotType == "Рабочая точка")
            {
                int zoneIndex = GetZoneIndexByReputation(zoneWeights, avgReputation);
                scheduleSlot.zoneType = workZones[zoneIndex];
                scheduleSlot.zoneName = workZoneNames[zoneIndex];
            }
            else
            {
                scheduleSlot.zoneType = "break";
                scheduleSlot.zoneName = slot.slotType;
            }
            
            dailySchedule.Add(scheduleSlot);
        }
        
        UpdateCurrentAndNextSlot();
    }
    
    int GetZoneIndexByReputation(int[] weights, float reputation)
    {
        float repFactor = Mathf.Clamp(reputation / 100f, -1f, 1f);
        
        float[] adjustedWeights = new float[weights.Length];
        for (int i = 0; i < weights.Length; i++)
        {
            if (i < 3)
                adjustedWeights[i] = weights[i] * (1 + repFactor);
            else
                adjustedWeights[i] = weights[i] * (1 - repFactor);
            
            adjustedWeights[i] = Mathf.Max(0.1f, adjustedWeights[i]);
        }
        
        float totalWeight = 0;
        foreach (float w in adjustedWeights)
            totalWeight += w;
        
        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0;
        
        for (int i = 0; i < adjustedWeights.Length; i++)
        {
            currentSum += adjustedWeights[i];
            if (randomValue <= currentSum)
                return i;
        }
        
        return 0;
    }
    
void UpdateCurrentAndNextSlot()
{
    currentSlot = null;
    nextSlot = null;
    
    for (int i = 0; i < dailySchedule.Count; i++)
    {
        ScheduleSlot slot = dailySchedule[i];
        
        // Находим текущий слот (любой)
        if (currentTime >= slot.startTime && currentTime < slot.endTime)
        {
            currentSlot = slot;
            
            // Находим следующий слот (любой)
            for (int j = i + 1; j < dailySchedule.Count; j++)
            {
                nextSlot = dailySchedule[j];
                break;
            }
            break;
        }
    }
    
    UpdateHUD();
}
    
void UpdateHUD()
{
    if (currentSlot != null)
    {
        // ТЕКУЩАЯ ТОЧКА
        if (currentZoneText != null)
            currentZoneText.text = $"СЕЙЧАС: {currentSlot.zoneName}";
        
        if (currentZoneTimerText != null)
        {
            float timeLeft = currentSlot.endTime - currentTime;
            int minutesLeft = Mathf.FloorToInt(timeLeft * 60);
            currentZoneTimerText.text = $"до {FormatTime(currentSlot.endTime)} ({minutesLeft} мин)";
        }
        
        // СЛЕДУЮЩАЯ ТОЧКА (включая перерывы)
        if (nextZoneText != null)
        {
            // Ищем следующий слот (любой, не только рабочий)
            ScheduleSlot nextAnySlot = GetNextAnySlot();
            if (nextAnySlot != null)
            {
                string nextName = nextAnySlot.slotType == "Рабочая точка" ? nextAnySlot.zoneName : nextAnySlot.zoneName;
                nextZoneText.text = $"СЛЕДУЮЩАЯ: {nextName} в {FormatTime(nextAnySlot.startTime)}";
            }
        }
    }
    else
    {
        // Если сейчас перерыв
        if (currentZoneText != null)
            currentZoneText.text = "СЕЙЧАС: ПЕРЕРЫВ";
        
        if (currentZoneTimerText != null && nextSlot != null)
            currentZoneTimerText.text = $"до {FormatTime(nextSlot.startTime)}";
        
        if (nextZoneText != null && nextSlot != null)
            nextZoneText.text = $"СЛЕДУЮЩАЯ: {nextSlot.zoneName} в {FormatTime(nextSlot.startTime)}";
    }
}

ScheduleSlot GetNextAnySlot()
{
    // Находим следующий слот после текущего времени (любой: рабочий или перерыв)
    for (int i = 0; i < dailySchedule.Count; i++)
    {
        if (dailySchedule[i].startTime > currentTime)
        {
            return dailySchedule[i];
        }
    }
    return null;
}
    
    public string GetCurrentRequiredZone()
    {
        if (currentSlot != null && currentSlot.slotType == "Рабочая точка")
            return currentSlot.zoneType;
        return "break";
    }
    
    public string GetCurrentRequiredZoneName()
    {
        if (currentSlot != null && currentSlot.slotType == "Рабочая точка")
            return currentSlot.zoneName;
        return "Перерыв";
    }
    
    string FormatTime(float time)
    {
        int hour = Mathf.FloorToInt(time);
        int minute = Mathf.FloorToInt((time - hour) * 60);
        return $"{hour:00}:{minute:00}";
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
        UpdateCurrentAndNextSlot();
        
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
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = new Vector3(0, 1, 0);
                controller.enabled = true;
            }
        }
        
        // Генерируем новое расписание
        GenerateSchedule();
        
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
    
    [System.Serializable]
    public class TimeSlot
    {
        public float startTime;
        public float endTime;
        public string slotType;
        
        public TimeSlot(float start, float end, string type)
        {
            startTime = start;
            endTime = end;
            slotType = type;
        }
    }
    
    [System.Serializable]
    public class ScheduleSlot
    {
        public float startTime;
        public float endTime;
        public string slotType;
        public string zoneType;
        public string zoneName;
    }
}