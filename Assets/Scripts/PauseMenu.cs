using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("=== КАНВАС ПАУЗЫ ===")]
    public GameObject pauseCanvas;
    
    [Header("=== КНОПКИ ===")]
    public Button resumeButton;
    public Button restartButton;
    public Button menuButton;
    
    private bool isPaused = false;
    
    void Start()
    {
        // Проверяем, назначен ли канвас
        if (pauseCanvas == null)
        {
            Debug.LogError("[Пауза] pauseCanvas не назначен в инспекторе!");
            return;
        }
        
        // Скрываем канвас при старте
        pauseCanvas.SetActive(false);
        
        // Назначаем обработчики кнопок
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(MainMenu);
        
        // Настройка курсора для игры
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Нажатие Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[Пауза] Нажата Escape");
            
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    public void PauseGame()
    {
        Debug.Log("[Пауза] Включение паузы");
        
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void ResumeGame()
    {
        Debug.Log("[Пауза] Выключение паузы");
        
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void RestartGame()
    {
        Debug.Log("[Пауза] Перезапуск игры");
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("CharacterSelect");
    }
    
    public void MainMenu()
    {
        Debug.Log("[Пауза] Выход в главное меню");
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}