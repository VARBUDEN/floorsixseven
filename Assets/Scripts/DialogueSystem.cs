using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour
{
    [Header("=== UI ===")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Button agreeButton;
    public Button refuseButton;
    
    [Header("=== НАСТРОЙКИ ДИАЛОГОВ ===")]
    public float dialogueDelay = 0.5f;
    
    private List<int> npcQueue = new List<int>();
    private int currentNPCIndex = 0;
    private NPCData[] allNPCs;
    private ReputationSystem reputationSystem;
    private DailyBuffSystem dailyBuff;
    private ChoiceHistory choiceHistory;
    private DayCycleSystem dayCycle;
    
    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        if (agreeButton != null)
            agreeButton.onClick.AddListener(OnAgree);
        
        if (refuseButton != null)
            refuseButton.onClick.AddListener(OnRefuse);
        
        InitializeNPCs();
        reputationSystem = FindAnyObjectByType<ReputationSystem>();
        dailyBuff = FindAnyObjectByType<DailyBuffSystem>();
        choiceHistory = FindAnyObjectByType<ChoiceHistory>();
        dayCycle = FindAnyObjectByType<DayCycleSystem>();
        
        Invoke("StartDialogue", dialogueDelay);
    }
    
    void InitializeNPCs()
    {
        allNPCs = new NPCData[]
        {
            new NPCData("Свистик", "8-ка как перерыв", null),
            new NPCData("Шелли", "+20% к промо", null),
            new NPCData("Дырка", "-8% траты", null),
            new NPCData("Мистер Пи", "Шкала ярости", null),
            new NPCData("Кулич", "Ивенты x2", null),
            new NPCData("Магомедова", "Меньший штраф", null),
            new NPCData("Мурена", "+20 стамины", null),
            new NPCData("Радмир", "+10 восстановление", null)
        };
        
        npcQueue.Clear();
        for (int i = 0; i < allNPCs.Length; i++)
            npcQueue.Add(i);
        
        ShuffleQueue();
    }
    
    void ShuffleQueue()
    {
        for (int i = 0; i < npcQueue.Count; i++)
        {
            int temp = npcQueue[i];
            int randomIndex = Random.Range(i, npcQueue.Count);
            npcQueue[i] = npcQueue[randomIndex];
            npcQueue[randomIndex] = temp;
        }
    }
    
    void StartDialogue()
    {
        if (npcQueue.Count == 0)
        {
            InitializeNPCs();
        }
        
        currentNPCIndex = npcQueue[0];
        npcQueue.RemoveAt(0);
        
        ShowDialogue();
    }
    
    void ShowDialogue()
    {
        Time.timeScale = 0f;
        
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        
        NPCData npc = allNPCs[currentNPCIndex];
        
        if (nameText != null)
            nameText.text = npc.npcName;
        
        if (dialogueText != null)
            dialogueText.text = $"Привет! {GetRandomRequest(npc.npcName)}";
        
        if (portraitImage != null && npc.portrait != null)
            portraitImage.sprite = npc.portrait;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    string GetRandomRequest(string npcName)
    {
        string[] requests = {
            "Можешь помочь мне сегодня?",
            "Есть одно дело... поможешь?",
            "Подмени меня на точке?",
            "У меня важный звонок, постоишь за меня?"
        };
        return requests[Random.Range(0, requests.Length)];
    }
    
    void OnAgree()
    {
        string npcName = allNPCs[currentNPCIndex].npcName;
        float change = reputationSystem.GetAgreeChange(npcName);
        
        if (reputationSystem != null)
        {
            reputationSystem.AddReputationFromDialogue(npcName, true);
        }
        
        if (choiceHistory != null)
        {
            choiceHistory.AddEntry(dayCycle.currentDay, npcName, "СОГЛАСИЕ", change);
        }
        
        ApplyDebuff(npcName);
        EndDialogue();
    }
    
    void OnRefuse()
    {
        string npcName = allNPCs[currentNPCIndex].npcName;
        float change = reputationSystem.GetRefuseChange(npcName);
        
        if (reputationSystem != null)
        {
            reputationSystem.AddReputationFromDialogue(npcName, false);
        }
        
        if (choiceHistory != null)
        {
            choiceHistory.AddEntry(dayCycle.currentDay, npcName, "ОТКАЗ", change);
        }
        
        ApplyBuff(npcName);
        EndDialogue();
    }
    
    void ApplyBuff(string npcName)
    {
        if (dailyBuff == null) return;
        
        switch (npcName)
        {
            case "Мурена":
                dailyBuff.SetBuff("StaminaMax", 20f);
                break;
            case "Радмир":
                dailyBuff.SetBuff("LookDownRestore", 5f);
                dailyBuff.SetBuff("BreakRestore", 2f);
                break;
            case "Свистик":
                dailyBuff.SetBuff("BreakRestore", 2f);
                break;
            case "Шелли":
                dailyBuff.SetBuff("PromoBonus", 0.4f);
                break;
            default:
                Debug.Log($"[Диалог] Нет баффа для {npcName}");
                break;
        }
    }
    
    void ApplyDebuff(string npcName)
    {
        if (dailyBuff == null) return;
        
        switch (npcName)
        {
            case "Мурена":
                dailyBuff.SetBuff("StaminaMax", -20f);
                break;
            case "Радмир":
                dailyBuff.SetBuff("LookDownRestore", -5f);
                break;
            case "Свистик":
                dailyBuff.SetBuff("BreakRestore", -2f);
                break;
            default:
                Debug.Log($"[Диалог] Нет дебаффа для {npcName}");
                break;
        }
    }
    
    void EndDialogue()
    {
        Time.timeScale = 1f;
        
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    [System.Serializable]
    public class NPCData
    {
        public string npcName;
        public string buffDescription;
        public Sprite portrait;
        
        public NPCData(string name, string buff, Sprite spr)
        {
            npcName = name;
            buffDescription = buff;
            portrait = spr;
        }
    }
}