using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AngerSystem : MonoBehaviour
{
    public static int StaticTotalAnger = 0;

    [Header("=== ДНЕВНОЙ ГНЕВ (0-100) ===")]
    public float dailyAnger = 0f;
    public float maxDailyAnger = 100f;

    [Header("=== ОБЩИЙ ГНЕВ (0-5) ===")]
    public int totalAnger = 0;
    public int maxTotalAnger = 5;

    [Header("=== UI ===")]
    public Slider dailyAngerSlider;
    public TextMeshProUGUI dailyAngerText;
    public TextMeshProUGUI totalAngerText;

    [Header("=== ЦВЕТА ===")]
    public Color normalColor = Color.white;
    public Color warningColor = new Color(1f, 0.5f, 0f);
    public Color dangerColor = Color.red;

    void Start()
    {
        totalAnger = StaticTotalAnger;
        totalAnger = Mathf.Clamp(totalAnger, 0, maxTotalAnger);
        ResetDailyAnger();
        UpdateTotalAngerUI();
    }

    public void AddDailyAnger(float amount)
    {
        dailyAnger += amount;
        dailyAnger = Mathf.Clamp(dailyAnger, 0f, maxDailyAnger);

        Debug.Log($"[Гнев] +{amount:F1}. Дневной гнев: {dailyAnger:F0}/{maxDailyAnger}");

        UpdateDailyAngerUI();

        if (dailyAnger >= maxDailyAnger)
        {
            Debug.Log("[Гнев] ДОСРОЧНЫЙ КОНЕЦ ДНЯ!");
            totalAnger++;
            totalAnger = Mathf.Clamp(totalAnger, 0, maxTotalAnger);
            StaticTotalAnger = totalAnger;
            UpdateTotalAngerUI();

            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null) gameManager.EndDayEarly();

            dailyAnger = 0f;
            UpdateDailyAngerUI();
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
            StaticTotalAnger = 0;
            PlayerPrefs.SetInt("LastDays", GameManager.currentDay);
            PlayerPrefs.SetInt("LastSalary", Mathf.RoundToInt(GameManager.totalSalary));
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameOver");
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
            dailyAngerSlider.maxValue = maxDailyAnger;
            dailyAngerSlider.value = dailyAnger;
        }

        if (dailyAngerText != null)
            dailyAngerText.text = $"ГНЕВ: {dailyAnger:F0}%";

        if (dailyAngerSlider != null)
        {
            Image fillImage = dailyAngerSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                if (dailyAnger < 30f) fillImage.color = Color.green;
                else if (dailyAnger < 70f) fillImage.color = Color.yellow;
                else fillImage.color = Color.red;
            }
        }
    }

    void UpdateTotalAngerUI()
    {
        if (totalAngerText != null)
        {
            totalAngerText.text = $"ВЫГОВОРОВ: {totalAnger}/{maxTotalAnger}";
            if (totalAnger >= 4) totalAngerText.color = dangerColor;
            else if (totalAnger >= 3) totalAngerText.color = warningColor;
            else totalAngerText.color = normalColor;
        }

        if (totalAnger >= maxTotalAnger)
            SceneManager.LoadScene("GameOver");
    }
}