using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [Header("Канвас паузы")]
    public GameObject pauseCanvas;
    
    [Header("Кнопки")]
    public Button resumeButton;
    public Button restartButton;
    public Button menuButton;
    public Button nextDayButton;  // ← НОВАЯ КНОПКА
    
    private bool isPaused = false;
    
    void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(Menu);
        
        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(NextDay);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    
    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CharacterSelect");
    }
    
    void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    void NextDay()
    {
        Debug.Log("[Пауза] Нажат Следующий день");
        
        // Находим DayCycleSystem и запускаем следующий день
        DayCycleSystem dayCycle = FindAnyObjectByType<DayCycleSystem>();
        if (dayCycle != null)
        {
            dayCycle.NextDay();
        }
        else
        {
            Debug.LogError("[Пауза] DayCycleSystem не найден!");
        }
        
        // Выходим из паузы
        Resume();
    }
}