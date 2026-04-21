using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;   

public class MainMenu : MonoBehaviour
{
    [Header("Кнопки")]
    public GameObject playButton;
    public GameObject recordsButton;
    public GameObject exitButton;
    
    [Header("Панель рекордов")]
    public GameObject recordsPanel;
    public TextMeshProUGUI recordsText;
    
    [Header("Кнопка закрытия панели")]
    public Button closeRecordsButton;
    
    void Start()
    {
        // Проверка и защита от null
        if (recordsPanel != null)
            recordsPanel.SetActive(false);
        else
            Debug.LogWarning("recordsPanel не назначен в инспекторе!");
        
        LoadRecords();
        
        // Назначаем кнопку закрытия
        if (closeRecordsButton != null)
            closeRecordsButton.onClick.AddListener(OnCloseRecordsClick);
    }
    
    public void OnPlayClick()
    {
        SceneManager.LoadScene("CharacterSelect");
    }
    
    public void OnRecordsClick()
    {
        if (recordsPanel != null)
        {
            recordsPanel.SetActive(true);
            LoadRecords();
        }
        else
        {
            Debug.LogError("recordsPanel не назначен, нельзя показать рекорды!");
        }
    }
    
    public void OnCloseRecordsClick()
    {
        if (recordsPanel != null)
            recordsPanel.SetActive(false);
    }
    
    public void OnExitClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    void LoadRecords()
    {
        if (recordsText == null) return;
        
        int bestSalary = PlayerPrefs.GetInt("BestSalary", 0);
        int bestDays = PlayerPrefs.GetInt("BestDays", 0);
        
        recordsText.text = $"ЛУЧШИЙ РЕЗУЛЬТАТ\n\n" +
                          $"Зарплата: {bestSalary}\n" +
                          $"Дней: {bestDays}";
    }
}