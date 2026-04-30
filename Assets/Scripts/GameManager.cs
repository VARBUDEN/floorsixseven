using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static int currentDay = 1;
    public static float totalSalary = 0f;
    public static float burnoutMultiplier = 1f;

    [Header("Время")]
    public float dayDuration = 720f;

    [Header("UI")]
    public TextMeshProUGUI dayText;
    public GameObject endDayPanel;
    public Button nextDayButton;
    public TextMeshProUGUI endDayText;

    [Header("UI Времени")]
    public TextMeshProUGUI timeText;
    public Slider timeProgressSlider;

    [Header("Зарплата")]
    public float baseHourlyRate = 10f;
    public float liftBonus = 0f;
    public float eightBonus = 15f;
    public float promoBonus = 10f;
    public float perekBonus = 5f;
    public float navigBonus = 0f;

    [Header("UI Зарплаты")]
    public TextMeshProUGUI totalSalaryText;
    public TextMeshProUGUI hourlyRateText;
    public TextMeshProUGUI floatingBonusText;

    private float accumulatedBonus = 0f;
    private float bonusLeaveTimer = 0f;
    private bool isOnBonusZone = false;
    private string lastBonusZone = "";

    private float dayTimer;
    private bool isDayActive = true;

    private StaminaNew stamina;
    private float hourlyTimer = 0f;

    public float burnoutIncreasePerDay = 0.2f;

    [Header("=== ТЕКУЩИЕ ЗНАЧЕНИЯ ===")]
    public float currentTime = 10f;
    public float currentDaySalary = 0f;

    void Start()
    {
        dayTimer = 0f;
        isDayActive = true;

        if (endDayPanel != null)
            endDayPanel.SetActive(false);

        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(NextDay);

        UpdateUI();
        UpdateTimeDisplay();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateSalaryUI();
        stamina = FindAnyObjectByType<StaminaNew>();  // ← ДОБАВИТЬ
        UpdateHourlyRateUI();

    }

    void Update()
    {
        if (!isDayActive) return;

        dayTimer += Time.deltaTime;
        hourlyTimer += Time.deltaTime;
        UpdateTimeDisplay();

        if (hourlyTimer >= 10f)  // ← 10 секунд на игровой час
        {
            hourlyTimer = 0f;
            AddHourlySalary();
        }

        if (dayTimer >= dayDuration)
        {
            EndDay();
        }

        UpdateBonusAccumulation();
    }

    void UpdateBonusAccumulation()
    {
        if (stamina == null) return;

        string currentZone = stamina.currentZoneType;
        float zoneBonus = GetZoneBonus(currentZone);
        bool isOnWorkZone = stamina.isAtWork && !stamina.isOnBreak;

        if (isOnWorkZone && zoneBonus > 0)
        {
            if (!isOnBonusZone || lastBonusZone != currentZone)
            {
                bonusLeaveTimer = 0f;
            }

            isOnBonusZone = true;
            lastBonusZone = currentZone;

            float accumulationRate = zoneBonus / 60f;
            accumulatedBonus += accumulationRate * Time.deltaTime;

            UpdateFloatingBonusText(currentZone, zoneBonus, accumulatedBonus);
        }
        else
        {
            if (isOnBonusZone)
            {
                bonusLeaveTimer += Time.deltaTime;

                if (bonusLeaveTimer >= 5f)
                {
                    if (accumulatedBonus > 0)
                    {
                        float bonusToAdd = accumulatedBonus;
                        totalSalary += bonusToAdd;
                        currentDaySalary += bonusToAdd;
                        accumulatedBonus = 0f;
                        UpdateSalaryUI();
                        Debug.Log($"[Зарплата] Зачислен бонус: {bonusToAdd:F2}");
                    }
                    isOnBonusZone = false;
                    lastBonusZone = "";

                    if (floatingBonusText != null)
                        floatingBonusText.gameObject.SetActive(false);
                }
                else
                {
                    UpdateFloatingBonusText(lastBonusZone, GetZoneBonus(lastBonusZone), accumulatedBonus, true);
                }
            }
        }
    }

    float GetZoneBonus(string zoneType)
    {
        switch (zoneType)
        {
            case "lift": return liftBonus;
            case "eight": return eightBonus;
            case "promo": return promoBonus;
            case "perek": return perekBonus;
            case "navig": return navigBonus;
            default: return 0f;
        }
    }

    string GetZoneDisplayName(string zoneType)
    {
        switch (zoneType)
        {
            case "lift": return "Лифт";
            case "eight": return "8-ка";
            case "promo": return "Промо";
            case "perek": return "Перекрёсток";
            case "navig": return "Навигация";
            default: return "";
        }
    }

    void UpdateSalaryUI()
    {
        if (totalSalaryText != null)
            totalSalaryText.text = $"$ {totalSalary:F0}";
    }

    void UpdateHourlyRateUI()
    {
        if (hourlyRateText == null) return;

        string currentZone = stamina != null ? stamina.currentZoneType : "";
        float bonus = GetZoneBonus(currentZone);
        hourlyRateText.text = $"$/ч: {baseHourlyRate + bonus:F0}";
    }
    void AddHourlySalary()
    {
        float total = baseHourlyRate;  // без бонуса точек
        totalSalary += total;
        currentDaySalary += total;
        UpdateSalaryUI();
        Debug.Log($"[Зарплата] Час +{total} (база={baseHourlyRate})");
    }

    void UpdateFloatingBonusText(string zoneName, float bonusPerHour, float accumulated, bool isFrozen = false)
    {
        if (floatingBonusText == null) return;

        string zoneDisplayName = GetZoneDisplayName(zoneName);
        string frozenMark = isFrozen ? "" : "";

        floatingBonusText.text = $"+{bonusPerHour:F0} {zoneDisplayName}{frozenMark}\n{accumulated:F2} монет";
        floatingBonusText.gameObject.SetActive(true);
    }
    void UpdateTimeDisplay()
    {
        float progress = dayTimer / dayDuration;

        if (timeProgressSlider != null)
            timeProgressSlider.value = progress;

        if (timeText != null)
        {
            float currentHour = 10f + progress * 12f;
            int hour = Mathf.FloorToInt(currentHour);
            int minute = Mathf.FloorToInt((currentHour - hour) * 60);
            timeText.text = $"{hour:00}:{minute:00}";
        }
    }

    void EndDay()
    {
        isDayActive = false;

        if (accumulatedBonus > 0)
        {
            totalSalary += accumulatedBonus;
            accumulatedBonus = 0f;
            UpdateSalaryUI();
            Debug.Log($"[GameManager] Бонус зачислен при завершении дня: {accumulatedBonus:F2}");
        }

        Time.timeScale = 0f;

        // СОХРАНЯЕМ ТЕКУЩИЙ ПРОГРЕСС
        PlayerPrefs.SetInt("LastDays", currentDay);
        PlayerPrefs.SetInt("LastSalary", Mathf.RoundToInt(totalSalary));
        PlayerPrefs.Save();

        if (endDayText != null)
        {
            endDayText.text = $"ДЕНЬ {currentDay} ЗАКОНЧЕН\n\n"
                            + $"Зарплата за день: {Mathf.RoundToInt(currentDaySalary)}\n"
                            + $"Общая зарплата: {Mathf.RoundToInt(totalSalary)}";
        }

        if (endDayPanel != null)
            endDayPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public bool IsEndDayActive()
    {
        return endDayPanel != null && endDayPanel.activeSelf;
    }
    public void ForceAddBonus()
    {
        if (accumulatedBonus > 0)
        {
            totalSalary += accumulatedBonus;
            currentDaySalary += accumulatedBonus;
            accumulatedBonus = 0f;
            UpdateSalaryUI();
            Debug.Log($"[GameManager] Бонус принудительно зачислен: {accumulatedBonus:F2}");
        }
    }
    void NextDay()
    {
        currentDay++;
        // Увеличиваем выгорание
        burnoutMultiplier += burnoutIncreasePerDay;

        // Восстанавливаем стамину и заряд до 80%
        StaminaNew stamina = FindAnyObjectByType<StaminaNew>();
        if (stamina != null)
        {
            stamina.currentStamina = stamina.maxStamina * 0.8f;
            stamina.currentBattery = stamina.maxBattery * 0.8f;
            stamina.UpdateUI();
            stamina.UpdateBatteryUI();
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateUI()
    {
        if (dayText != null)
            dayText.text = $"ДЕНЬ {currentDay}";
    }

    public void EndDayEarly()
    {
        isDayActive = false;

        if (accumulatedBonus > 0)
        {
            totalSalary += accumulatedBonus;
            accumulatedBonus = 0f;
            UpdateSalaryUI();
            Debug.Log($"[GameManager] Бонус зачислен при досрочном завершении: {accumulatedBonus:F2}");
        }

        Time.timeScale = 0f;

        // СОХРАНЯЕМ ТЕКУЩИЙ ПРОГРЕСС
        PlayerPrefs.SetInt("LastDays", currentDay);
        PlayerPrefs.SetInt("LastSalary", Mathf.RoundToInt(totalSalary));
        PlayerPrefs.Save();

        if (endDayText != null)
        {
            endDayText.text = $"ДЕНЬ {currentDay} ЗАКОНЧЕН ДОСРОЧНО\n\n"
                            + $"Зарплата за день: {Mathf.RoundToInt(currentDaySalary)}\n"
                            + $"Общая зарплата: {Mathf.RoundToInt(totalSalary)}";
        }

        if (endDayPanel != null)
            endDayPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[GameManager] День закончен досрочно из-за гнева!");
    }
}