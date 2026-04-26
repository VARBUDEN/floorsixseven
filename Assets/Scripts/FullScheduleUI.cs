using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class FullScheduleUI : MonoBehaviour
{
    [Header("=== UI ===")]
    public GameObject scheduleCanvas;
    public TextMeshProUGUI scheduleText;
    public TextMeshProUGUI reputationText;
    public TextMeshProUGUI historyText;
    
    [Header("=== ЦВЕТА ДЛЯ РЕПУТАЦИИ ===")]
    public Color colorPredev = new Color(1f, 0.84f, 0f);
    public Color colorFriend = new Color(0.2f, 0.8f, 0.2f);
    public Color colorGood = new Color(0.5f, 0.9f, 0.5f);
    public Color colorNormal = Color.gray;
    public Color colorBad = new Color(1f, 0.6f, 0.6f);
    public Color colorEnemy = new Color(1f, 0.3f, 0.3f);
    public Color colorHate = new Color(0.6f, 0f, 0f);
    
    [Header("=== НАСТРОЙКИ ===")]
    public KeyCode openKey = KeyCode.Tab;
    
    private DayCycleSystem dayCycle;
    private ReputationSystem reputationSystem;
    private ChoiceHistory choiceHistory;
    private bool isOpen = false;
    
    void Start()
    {
        dayCycle = FindAnyObjectByType<DayCycleSystem>();
        reputationSystem = FindAnyObjectByType<ReputationSystem>();
        choiceHistory = FindAnyObjectByType<ChoiceHistory>();
        
        if (scheduleCanvas != null)
            scheduleCanvas.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(openKey))
            OpenSchedule();
        
        if (Input.GetKeyUp(openKey))
            CloseSchedule();
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
        if (scheduleText == null || dayCycle == null) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("РАСПИСАНИЕ НА ДЕНЬ\n");
        sb.AppendLine("═══════════════════════════════");
        
        foreach (var slot in dayCycle.dailySchedule)
        {
            string timeRange = $"{FormatTime(slot.startTime)} - {FormatTime(slot.endTime)}";
            string zone = slot.slotType == "Рабочая точка" ? slot.zoneName : $"=== {slot.zoneName.ToUpper()} ===";
            sb.AppendLine($"{timeRange}  {zone}");
        }
        
        scheduleText.text = sb.ToString();
    }
    
    void UpdateReputationText()
    {
        if (reputationText == null || reputationSystem == null) return;
        
        StringBuilder sb = new StringBuilder();
        
        string playerName = GetPlayerCharacterName();
        sb.AppendLine($"ВЫ: {playerName}");
        sb.AppendLine("");
        sb.AppendLine("РЕПУТАЦИЯ NPC");
        sb.AppendLine("");
        
        string[] allNPCs = { "Свистик", "Шелли", "Дырка", "Мистер Пи", "Кулич", "Магомедова", "Мурена", "Радмир" };
        
        foreach (string npcName in allNPCs)
        {
            if (npcName == playerName) continue;
            
            float rep = reputationSystem.GetReputation(npcName);
            string level = GetRelationshipLevel(rep);
            Color color = GetRelationshipColor(rep);
            
            string coloredLine = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{npcName}:  {rep,3:F0}  ({level})</color>";
            sb.AppendLine(coloredLine);
        }
        
        sb.AppendLine("");
        sb.AppendLine("-= ЛЕГЕНДА =-");
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(colorPredev)}>100-81: ПРЕДАН</color>");
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(colorFriend)}>80-61: ДРУГ</color>");
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(colorGood)}>60-31: ХОРОШО</color>");
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(colorNormal)}>30 - -30: НОРМА</color>");
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(colorBad)}>-31 - -60: ПЛОХО</color>");
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(colorEnemy)}>-61 - -80: ВРАГ</color>");
        sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(colorHate)}>-81 - -100: НЕНАВИСТЬ</color>");
        
        reputationText.text = sb.ToString();
    }
    
    void UpdateHistoryText()
    {
        if (historyText == null || choiceHistory == null) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("-= ИСТОРИЯ ВЫБОРОВ =-");
        sb.AppendLine("");
        
        foreach (var entry in choiceHistory.GetHistory())
        {
            string color = entry.choice == "СОГЛАСИЕ" ? "#88ff88" : "#ff8888";
            sb.AppendLine($"<color={color}>День {entry.day}: {entry.npcName} → {entry.choice} ({entry.reputationChange:+0;-0})</color>");
        }
        
        if (choiceHistory.GetHistory().Count == 0)
        {
            sb.AppendLine("Пока нет выборов...");
        }
        
        historyText.text = sb.ToString();
    }
    
    string GetRelationshipLevel(float rep)
    {
        if (rep >= 81) return "ПРЕДАН";
        if (rep >= 61) return "ДРУГ";
        if (rep >= 31) return "ХОРОШО";
        if (rep >= -30) return "НОРМА";
        if (rep >= -60) return "ПЛОХО";
        if (rep >= -80) return "ВРАГ";
        return "НЕНАВИСТЬ";
    }
    
    Color GetRelationshipColor(float rep)
    {
        if (rep >= 81) return colorPredev;
        if (rep >= 61) return colorFriend;
        if (rep >= 31) return colorGood;
        if (rep >= -30) return colorNormal;
        if (rep >= -60) return colorBad;
        if (rep >= -80) return colorEnemy;
        return colorHate;
    }
    
    string GetPlayerCharacterName()
    {
        switch (CharacterSelect.selectedCharacter)
        {
            case CharacterSelect.Character.Svistik: return "Свистик";
            case CharacterSelect.Character.Shell: return "Шелли";
            case CharacterSelect.Character.Dyrka: return "Дырка";
            case CharacterSelect.Character.MrPi: return "Мистер Пи";
            case CharacterSelect.Character.Kulich: return "Кулич";
            case CharacterSelect.Character.Magomedova: return "Магомедова";
            case CharacterSelect.Character.Morena: return "Мурена";
            case CharacterSelect.Character.Radmir: return "Радмир";
            default: return "Неизвестный";
        }
    }
    
    string FormatTime(float time)
    {
        int hour = Mathf.FloorToInt(time);
        int minute = Mathf.FloorToInt((time - hour) * 60);
        return $"{hour:00}:{minute:00}";
    }
}