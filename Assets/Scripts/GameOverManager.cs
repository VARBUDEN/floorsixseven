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
    
    private int lastDays;
    private int lastSalary;
    private int bestSalary;
    private int bestDays;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // ЗАГРУЖАЕМ ДАННЫЕ
        lastDays = PlayerPrefs.GetInt("LastDays", 0);
        lastSalary = PlayerPrefs.GetInt("LastSalary", 0);
        bestSalary = PlayerPrefs.GetInt("BestSalary", 0);
        bestDays = PlayerPrefs.GetInt("BestDays", 0);
        
        Debug.Log($"[GameOver] lastDays={lastDays}, lastSalary={lastSalary}, bestSalary={bestSalary}, bestDays={bestDays}");
        
        // ОТОБРАЖАЕМ ИТОГИ ПОСЛЕДНЕГО ЗАБЕГА
        if (daysText != null)
            daysText.text = $"ДНЕЙ ПРОЖИТО: {lastDays}";
        
        if (salaryText != null)
            salaryText.text = $"ИТОГОВАЯ ЗАРПЛАТА: {lastSalary}";
        
        // ПРОВЕРЯЕМ РЕКОРД
        bool isNewRecord = lastSalary > bestSalary;
        
        if (isNewRecord)
        {
            // СОХРАНЯЕМ НОВЫЙ РЕКОРД
            PlayerPrefs.SetInt("BestSalary", lastSalary);
            PlayerPrefs.SetInt("BestDays", lastDays);
            PlayerPrefs.Save();
            
            if (newRecordText != null)
                newRecordText.text = "НОВЫЙ РЕКОРД!";
            
            if (recordText != null)
                recordText.text = $"РЕКОРД: {lastSalary} монет ({lastDays} дней)";
        }
        else
        {
            if (newRecordText != null)
                newRecordText.text = "";
            
            if (recordText != null)
                recordText.text = $"ЛУЧШИЙ РЕКОРД: {bestSalary} монет ({bestDays} дней)";
        }
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClick);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClick);
    }
    
    public void OnRestartClick()
    {
        GameReset.ResetAndGoToCharacterSelect();
    }
    
    public void OnMenuClick()
    {
        GameReset.ResetAndGoToMainMenu();
    }
}