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
    public Button resetProgressButton; 

    [Header("Панель рекордов")]
    public GameObject recordsPanel;
    public TextMeshProUGUI recordsText;

    [Header("Кнопка закрытия панели")]
    public Button closeRecordsButton;

    void Start()
    {
        GameManager.currentDay = 1;
        GameManager.totalSalary = 0f;
        AngerSystem.StaticTotalAnger = 0;
        GameManager.burnoutMultiplier = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (resetProgressButton != null)
            resetProgressButton.onClick.AddListener(OnResetProgressClick);

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

    public void OnResetProgressClick()
    {
        // Подтверждение сброса
        if (Application.isEditor)
        {
            // В редакторе без подтверждения
            ResetAllProgress();
        }
        else
        {
            // В билде спрашиваем
            Debug.Log("[MainMenu] Сброс прогресса (в билде нужно подтверждение)");
            ResetAllProgress();
        }
    }

    private void ResetAllProgress()
    {
        // Очищаем все сохранения
        PlayerPrefs.DeleteAll();

        // Сбрасываем статические переменные
        GameManager.currentDay = 1;
        GameManager.totalSalary = 0f;
        GameManager.burnoutMultiplier = 1f;
        AngerSystem.StaticTotalAnger = 0;

        Debug.Log("[MainMenu] Весь прогресс сброшен!");

        // Перезагружаем сцену, чтобы обновить рекорды на экране
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}