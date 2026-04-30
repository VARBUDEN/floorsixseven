using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReset : MonoBehaviour
{
    public static void ResetGame()
    {
        Debug.Log("[GameReset] Полный сброс игры");
        
        GameManager.currentDay = 1;
        GameManager.totalSalary = 0f;
        GameManager.burnoutMultiplier = 1f;
        AngerSystem.StaticTotalAnger = 0;
        
        PlayerPrefs.DeleteKey("TotalAnger");
        PlayerPrefs.DeleteKey("CurrentDay");
        PlayerPrefs.DeleteKey("TotalSalary");
        
        Debug.Log("[GameReset] Сброс выполнен");
    }
    
    public static void ResetAndGoToCharacterSelect()
    {
        ResetGame();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("CharacterSelect");
    }
    
    public static void ResetAndGoToMainMenu()
    {
        ResetGame();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }
}