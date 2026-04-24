using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    
    void Start()
    {
        ResetDailyAnger();
        UpdateTotalAngerUI();
    }
    
    public void AddDailyAnger(float amount)
    {
        dailyAnger += amount;
        dailyAnger = Mathf.Clamp(dailyAnger, 0f, maxDailyAnger);
        
        Debug.Log($"[Гнев] +{amount}. Дневной гнев: {dailyAnger}/{maxDailyAnger}");
        
        UpdateDailyAngerUI();
        
        if (dailyAnger >= maxDailyAnger)
        {
            Debug.Log("[Гнев] ДОСРОЧНЫЙ КОНЕЦ ДНЯ!");
            AddTotalAnger(1);
        }
    }
    
    public void AddTotalAnger(int amount)
    {
        totalAnger += amount;
        totalAnger = Mathf.Clamp(totalAnger, 0, maxTotalAnger);
        
        UpdateTotalAngerUI();
        
        if (totalAnger >= maxTotalAnger)
        {
            Debug.Log("[Гнев] ВАС УВОЛИЛИ!");
        }
    }
    
    public void ResetDailyAnger()
    {
        dailyAnger = 0f;
        UpdateDailyAngerUI();
    }
    
    void UpdateDailyAngerUI()
    {
        if (dailyAngerSlider != null)
        {
            dailyAngerSlider.value = dailyAnger;
        }
        
        if (dailyAngerText != null)
        {
            dailyAngerText.text = $"ГНЕВ: {dailyAnger:F0}%";
        }
    }
    
    void UpdateTotalAngerUI()
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