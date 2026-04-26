using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AngerSystem : MonoBehaviour
{
    public float dailyAnger = 0f;
    public float maxDailyAnger = 100f;
    
    public int totalAnger = 0;
    public int maxTotalAnger = 5;
    
    public Slider dailyAngerSlider;
    public TextMeshProUGUI dailyAngerText;
    public Image[] totalAngerIcons;
    
    public Color normalColor = Color.white;
    public Color filledColor = Color.red;
    
    private bool isDayEnding = false;  // чтобы не вызывать несколько раз
    
    void Start()
    {
        ResetDailyAnger();
        UpdateTotalAngerUI();
    }
    
public void AddDailyAnger(float amount)
{
    if (isDayEnding) return;
    
    dailyAnger += amount;
    dailyAnger = Mathf.Clamp(dailyAnger, 0f, maxDailyAnger);
    
    Debug.Log($"[Гнев] +{amount}. Дневной гнев: {dailyAnger}/{maxDailyAnger}");
    
    UpdateDailyAngerUI();
    
    if (dailyAnger >= maxDailyAnger)
    {
        isDayEnding = true;
        Debug.Log("[Гнев] ДОСРОЧНЫЙ КОНЕЦ ДНЯ!");
        
        // ВЫЗЫВАЕМ AddTotalAnger, который сам проверит увольнение
        AddTotalAnger(1);
        
        DayCycleSystem dayCycle = FindAnyObjectByType<DayCycleSystem>();
        if (dayCycle != null)
        {
            dayCycle.EndDayEarly();
        }
        
        dailyAnger = 0f;
        UpdateDailyAngerUI();
    }
}
    
    public void AddTotalAnger(int amount)
    {
        totalAnger += amount;
        totalAnger = Mathf.Clamp(totalAnger, 0, maxTotalAnger);
        
        UpdateTotalAngerUI();
        
        // УВОЛЬНЕНИЕ ТОЛЬКО ЗДЕСЬ
        if (totalAnger >= maxTotalAnger)
        {
            Debug.Log("[Гнев] ВАС УВОЛИЛИ! Переход на экран Game Over");
            
            DayCycleSystem dayCycle = FindAnyObjectByType<DayCycleSystem>();
            if (dayCycle != null)
            {
                PlayerPrefs.SetInt("LastDays", DayCycleSystem.LastDays);
                PlayerPrefs.SetInt("LastSalary", DayCycleSystem.LastSalary);
                PlayerPrefs.Save();
            }
            
            SceneManager.LoadScene("GameOver");
        }
    }
    
    public void ResetDailyAnger()
    {
        dailyAnger = 0f;
        isDayEnding = false;
        UpdateDailyAngerUI();
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
    
    public void UpdateTotalAngerUI()
    {
        if (totalAngerIcons == null) return;
        
        for (int i = 0; i < totalAngerIcons.Length; i++)
        {
            if (totalAngerIcons[i] != null)
            {
                totalAngerIcons[i].color = (i < totalAnger) ? filledColor : normalColor;
            }
        }
    }
}