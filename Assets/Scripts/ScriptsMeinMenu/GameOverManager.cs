using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI daysText;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI recordText;
    public TextMeshProUGUI newRecordText;
    
    public Button restartButton;
    public Button menuButton;
    
    private int daysSurvived;
    private int totalSalary;
    private int bestSalary;
    private int bestDays;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        daysSurvived = PlayerPrefs.GetInt("LastDays", 0);
        totalSalary = PlayerPrefs.GetInt("LastSalary", 0);
        bestSalary = PlayerPrefs.GetInt("BestSalary", 0);
        bestDays = PlayerPrefs.GetInt("BestDays", 0);
        
        Debug.Log($"[GameOver] Days={daysSurvived}, Salary={totalSalary}, Best={bestSalary}");
        
        if (daysText != null)
            daysText.text = $"ДНЕЙ ПРОЖИТО: {daysSurvived}";
        
        if (salaryText != null)
            salaryText.text = $"ИТОГОВАЯ ЗАРПЛАТА: {totalSalary}";
        
        bool isNewRecord = totalSalary > bestSalary;
        
        if (isNewRecord)
        {
            PlayerPrefs.SetInt("BestSalary", totalSalary);
            PlayerPrefs.SetInt("BestDays", daysSurvived);
            PlayerPrefs.Save();
            
            if (newRecordText != null)
                newRecordText.text = "НОВЫЙ РЕКОРД!";
            
            if (recordText != null)
                recordText.text = $"РЕКОРД: {totalSalary} (дней: {daysSurvived})";
        }
        else
        {
            if (newRecordText != null)
                newRecordText.text = "";
            
            if (recordText != null)
                recordText.text = $"ЛУЧШИЙ РЕКОРД: {bestSalary} (дней: {bestDays})";
        }
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClick);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClick);
    }
    
    public void OnRestartClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("CharacterSelect");
    }
    
    public void OnMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}