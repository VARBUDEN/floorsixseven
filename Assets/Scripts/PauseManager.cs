using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("=== UI ПАУЗЫ ===")]
    public GameObject pauseCanvas;
    
    [Header("=== КНОПКИ ===")]
    public Button resumeButton;
    public Button restartButton;
    public Button menuButton;
    public Button nextDayButton;  // ← НОВАЯ КНОПКА
    
    private bool isPaused = false;
    
    void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        
        // Назначаем кнопки
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(MenuGame);
        
        if (nextDayButton != null)
            nextDayButton.onClick.AddListener(NextDay);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Пауза по Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void RestartGame()
    {
        Time.timeScale = 1f;
        GameReset.ResetAndGoToCharacterSelect();
    }
    
    void MenuGame()
    {
        Time.timeScale = 1f;
        GameReset.ResetAndGoToMainMenu();
    }
    
    void NextDay()
    {
        // Выходим из паузы
        ResumeGame();
        
        // Завершаем текущий день
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.EndDayEarly();
        }
        else
        {
            // Если GameManager не найден, просто перезагружаем сцену
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}