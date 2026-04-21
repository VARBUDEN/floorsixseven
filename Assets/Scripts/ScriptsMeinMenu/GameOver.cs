using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    [Header("Тексты")]
    public TextMeshProUGUI daysText;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI recordText;
    public TextMeshProUGUI newRecordText;
    
    private int totalDays;
    private int totalSalary;
    
    void Start()
    {
        totalDays = PlayerPrefs.GetInt("LastDays", 0);
        totalSalary = PlayerPrefs.GetInt("LastSalary", 0);
        
        int bestSalary = PlayerPrefs.GetInt("BestSalary", 0);
        int bestDays = PlayerPrefs.GetInt("BestDays", 0);
        
        daysText.text = $"Дней прожито: {totalDays}";
        salaryText.text = $"Итоговая зарплата: {totalSalary}";
        
        bool isNewRecord = totalSalary > bestSalary;
        
        if (isNewRecord)
        {
            PlayerPrefs.SetInt("BestSalary", totalSalary);
            PlayerPrefs.SetInt("BestDays", totalDays);
            PlayerPrefs.Save();
            
            newRecordText.text = "НОВЫЙ РЕКОРД!";
            recordText.text = $"Рекорд: {totalSalary}";
        }
        else
        {
            newRecordText.text = "";
            recordText.text = $"Лучший рекорд: {bestSalary} (дней: {bestDays})";
        }
    }
    
    public void OnRestartClick()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
    
    public void OnMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}