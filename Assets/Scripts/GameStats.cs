using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStats : MonoBehaviour
{
    [Header("=== ТЕКУЩИЕ ДАННЫЕ ЗАБЕГА ===")]
    public int currentDay = 1;
    public float totalSalary = 0f;
    public int totalAnger = 0;
    
    [Header("=== РЕКОРДЫ ===")]
    public int bestSalary = 0;
    public int bestDays = 0;
    
    [Header("=== UI (опционально) ===")]
    public TextMeshProUGUI bestSalaryText;
    public TextMeshProUGUI bestDaysText;
    
    private static GameStats instance;
    
    void Awake()
    {
        // Singleton паттерн — один экземпляр на всю игру
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UpdateUI();
    }
    
    /// <summary>
    /// Загрузить рекорды из PlayerPrefs
    /// </summary>
    public void LoadStats()
    {
        bestSalary = PlayerPrefs.GetInt("BestSalary", 0);
        bestDays = PlayerPrefs.GetInt("BestDays", 0);
        
        Debug.Log($"[GameStats] Загружены рекорды: зарплата={bestSalary}, дни={bestDays}");
    }
    
    /// <summary>
    /// Сохранить рекорды в PlayerPrefs
    /// </summary>
    public void SaveStats()
    {
        PlayerPrefs.SetInt("BestSalary", bestSalary);
        PlayerPrefs.SetInt("BestDays", bestDays);
        PlayerPrefs.Save();
        
        Debug.Log($"[GameStats] Сохранены рекорды: зарплата={bestSalary}, дни={bestDays}");
    }
    
    /// <summary>
    /// Проверить и обновить рекорд по итогам забега
    /// </summary>
    public bool CheckAndUpdateRecord(int days, int salary)
    {
        bool isNewRecord = salary > bestSalary;
        
        if (isNewRecord)
        {
            bestSalary = salary;
            bestDays = days;
            SaveStats();
            Debug.Log($"[GameStats] НОВЫЙ РЕКОРД! {salary} (дней: {days})");
        }
        
        UpdateUI();
        return isNewRecord;
    }
    
    /// <summary>
    /// Сбросить текущий забег (но не рекорды!)
    /// </summary>
    public void ResetRun()
    {
        currentDay = 1;
        totalSalary = 0f;
        totalAnger = 0;
        
        Debug.Log("[GameStats] Текущий забег сброшен");
    }
    
    /// <summary>
    /// Полный сброс всех данных (рекорды + забег)
    /// </summary>
    public void ResetAll()
    {
        currentDay = 1;
        totalSalary = 0f;
        totalAnger = 0;
        bestSalary = 0;
        bestDays = 0;
        
        PlayerPrefs.DeleteKey("BestSalary");
        PlayerPrefs.DeleteKey("BestDays");
        
        Debug.Log("[GameStats] Полный сброс всех данных");
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (bestSalaryText != null)
            bestSalaryText.text = $"{bestSalary}";
        
        if (bestDaysText != null)
            bestDaysText.text = $"{bestDays}";
    }
    
    // Публичные статические методы для доступа
    public static int GetBestSalary() => instance?.bestSalary ?? 0;
    public static int GetBestDays() => instance?.bestDays ?? 0;
    public static int GetCurrentDay() => instance?.currentDay ?? 1;
    public static float GetTotalSalary() => instance?.totalSalary ?? 0f;
    
    public static void SetCurrentDay(int day)
    {
        if (instance != null) instance.currentDay = day;
    }
    
    public static void AddSalary(float amount)
    {
        if (instance != null) instance.totalSalary += amount;
    }
}