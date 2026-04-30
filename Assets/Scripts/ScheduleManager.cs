using UnityEngine;
using TMPro;
using System.Text;

public class ScheduleManager : MonoBehaviour
{
    [Header("=== UI ===")]
    public GameObject scheduleCanvas;
    public TextMeshProUGUI scheduleText;
    public TextMeshProUGUI reputationText;
    public TextMeshProUGUI historyText;

    [Header("=== НАСТРОЙКИ ===")]
    public KeyCode openKey = KeyCode.Tab;

    private bool isOpen = false;

    void Start()
    {
        if (scheduleCanvas != null)
            scheduleCanvas.SetActive(false);
    }

    void Update()
    {
        // Проверка паузы и конца дня
        GameManager gm = FindAnyObjectByType<GameManager>();
        bool isEndDayActive = gm != null && gm.IsEndDayActive();

        if (Time.timeScale == 0f && !isOpen) return;

        if (isEndDayActive) return;

        if (Input.GetKey(openKey))
        {
            if (!isOpen)
            {
                OpenSchedule();
            }
        }
        else
        {
            if (isOpen)
            {
                CloseSchedule();
            }
        }
    }

    void OpenSchedule()
    {
        if (isOpen) return;

        isOpen = true;
        Time.timeScale = 0f;

        if (scheduleCanvas != null)
            scheduleCanvas.SetActive(true);

        UpdateScheduleText();
        UpdateReputationText();
        UpdateHistoryText();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CloseSchedule()
    {
        if (!isOpen) return;

        isOpen = false;
        Time.timeScale = 1f;

        if (scheduleCanvas != null)
            scheduleCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UpdateScheduleText()
    {
        if (scheduleText == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("РАСПИСАНИЕ НА ДЕНЬ\n");
        sb.AppendLine("═══════════════════════════════");
        sb.AppendLine("10:00 - 11:20    Лифт");
        sb.AppendLine("11:20 - 11:40    === ПЕРЕРЫВ ===");
        sb.AppendLine("11:40 - 12:20    Навигация");
        sb.AppendLine("12:20 - 13:00    Перекрёсток");
        sb.AppendLine("13:00 - 13:40    === ОБЕД ===");
        sb.AppendLine("13:40 - 14:20    Промо");
        sb.AppendLine("14:20 - 15:40    8-ка");
        sb.AppendLine("15:40 - 16:20    Лифт");
        sb.AppendLine("16:20 - 16:40    === ПЕРЕРЫВ ===");
        sb.AppendLine("16:40 - 17:20    Навигация");
        sb.AppendLine("17:20 - 18:00    Перекрёсток");
        sb.AppendLine("18:00 - 18:40    === УЖИН ===");
        sb.AppendLine("18:40 - 19:20    Промо");
        sb.AppendLine("19:20 - 20:00    8-ка");
        sb.AppendLine("20:00 - 20:20    === ПЕРЕРЫВ ===");
        sb.AppendLine("20:20 - 22:00    Лифт");

        scheduleText.text = sb.ToString();
    }

    void UpdateReputationText()
    {
        if (reputationText == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("РЕПУТАЦИЯ NPC");
        sb.AppendLine("══════════════════");
        sb.AppendLine("Свистик:      0  (НОРМА)");
        sb.AppendLine("Шелли:       15  (ХОРОШО)");
        sb.AppendLine("Дырка:       15  (ХОРОШО)");
        sb.AppendLine("Мистер Пи:  -10  (НОРМА)");
        sb.AppendLine("Кулич:        0  (НОРМА)");
        sb.AppendLine("Магомедова:   5  (НОРМА)");
        sb.AppendLine("Радмир:     -10  (НОРМА)");

        reputationText.text = sb.ToString();
    }

    void UpdateHistoryText()
    {
        if (historyText == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("ИСТОРИЯ ВЫБОРОВ");
        sb.AppendLine("══════════════════");
        sb.AppendLine("Пока нет выборов...");

        historyText.text = sb.ToString();
    }
}