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
    
    private bool isPaused = false;
    
    void Start()
    {
        // Убеждаемся, что канвас выключен
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);
        
        // Назначаем кнопки
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(Menu);
        
        // Настройка курсора
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // По нажатию Tab
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
}