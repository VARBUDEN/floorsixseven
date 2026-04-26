using UnityEngine;
using System.Collections.Generic;

public class ReputationSystem : MonoBehaviour
{
    [Header("=== НАСТРОЙКИ РЕПУТАЦИИ ===")]
    public float defaultReputation = 0f;
    public float minReputation = -100f;
    public float maxReputation = 100f;
    
    [Header("=== ИЗМЕНЕНИЯ РЕПУТАЦИИ ЗА ДИАЛОГ ===")]
    public float svistikAgree = 5f;
    public float svistikRefuse = -15f;
    public float shellyAgree = 15f;
    public float shellyRefuse = -5f;
    public float dyrkaAgree = 10f;
    public float dyrkaRefuse = -10f;
    public float mrPiAgree = 5f;
    public float mrPiRefuse = -20f;
    public float kulichAgree = 20f;
    public float kulichRefuse = -3f;
    public float magomedovaAgree = 8f;
    public float magomedovaRefuse = -12f;
    public float morenaAgree = 12f;
    public float morenaRefuse = -8f;
    public float radmirAgree = 10f;
    public float radmirRefuse = -10f;
    
    private Dictionary<string, float> reputationValues = new Dictionary<string, float>();
    private string[] npcNames = {
        "Свистик", "Шелли", "Дырка", "Мистер Пи",
        "Кулич", "Магомедова", "Мурена", "Радмир"
    };
    
    void Start()
    {
        InitializeReputation();
        LoadReputation();
        DisplayAllReputation();
    }
    
    void InitializeReputation()
    {
        foreach (string npcName in npcNames)
        {
            if (!reputationValues.ContainsKey(npcName))
            {
                reputationValues.Add(npcName, defaultReputation);
            }
        }
        
        ApplyCharacterReputationBonuses();
    }
    
    void ApplyCharacterReputationBonuses()
    {
        CharacterSelect.Character selected = CharacterSelect.selectedCharacter;
        
        switch (selected)
        {
            case CharacterSelect.Character.Svistik:
                AddReputation("Свистик", 20);
                AddReputation("Кулич", 15);
                AddReputation("Мистер Пи", -10);
                break;
            case CharacterSelect.Character.Shell:
                AddReputation("Шелли", 20);
                AddReputation("Магомедова", 15);
                AddReputation("Радмир", -10);
                break;
            case CharacterSelect.Character.Dyrka:
                AddReputation("Дырка", 20);
                AddReputation("Мурена", 15);
                AddReputation("Шелли", -10);
                break;
            case CharacterSelect.Character.MrPi:
                AddReputation("Мистер Пи", 20);
                AddReputation("Радмир", 15);
                AddReputation("Свистик", -10);
                break;
            case CharacterSelect.Character.Kulich:
                AddReputation("Кулич", 20);
                AddReputation("Свистик", 15);
                AddReputation("Магомедова", -10);
                break;
            case CharacterSelect.Character.Magomedova:
                AddReputation("Магомедова", 20);
                AddReputation("Шелли", 15);
                AddReputation("Кулич", -10);
                break;
            case CharacterSelect.Character.Morena:
                AddReputation("Мурена", 20);
                AddReputation("Дырка", 15);
                AddReputation("Мистер Пи", -10);
                break;
            case CharacterSelect.Character.Radmir:
                AddReputation("Радмир", 20);
                AddReputation("Мистер Пи", 15);
                AddReputation("Мурена", -10);
                break;
        }
    }
    
    public float GetAgreeChange(string npcName)
    {
        switch (npcName)
        {
            case "Свистик": return svistikAgree;
            case "Шелли": return shellyAgree;
            case "Дырка": return dyrkaAgree;
            case "Мистер Пи": return mrPiAgree;
            case "Кулич": return kulichAgree;
            case "Магомедова": return magomedovaAgree;
            case "Мурена": return morenaAgree;
            case "Радмир": return radmirAgree;
            default: return 10f;
        }
    }
    
    public float GetRefuseChange(string npcName)
    {
        switch (npcName)
        {
            case "Свистик": return svistikRefuse;
            case "Шелли": return shellyRefuse;
            case "Дырка": return dyrkaRefuse;
            case "Мистер Пи": return mrPiRefuse;
            case "Кулич": return kulichRefuse;
            case "Магомедова": return magomedovaRefuse;
            case "Мурена": return morenaRefuse;
            case "Радмир": return radmirRefuse;
            default: return -5f;
        }
    }
    
    public void AddReputationFromDialogue(string npcName, bool agreed)
    {
        float change = agreed ? GetAgreeChange(npcName) : GetRefuseChange(npcName);
        AddReputation(npcName, change);
    }
    
    public void AddReputation(string npcName, float amount)
    {
        if (!reputationValues.ContainsKey(npcName))
        {
            reputationValues.Add(npcName, defaultReputation);
        }
        
        float oldValue = reputationValues[npcName];
        float newValue = Mathf.Clamp(oldValue + amount, minReputation, maxReputation);
        reputationValues[npcName] = newValue;
        
        Debug.Log($"[Репутация] {npcName}: {oldValue:F0} → {newValue:F0} ({amount:+0;-0})");
    }
    
    public float GetReputation(string npcName)
    {
        if (reputationValues.ContainsKey(npcName))
            return reputationValues[npcName];
        
        return defaultReputation;
    }
    
    public float GetReputationForSchedule(string npcName)
    {
        return GetReputation(npcName);
    }
    
public string GetRelationshipLevel(float rep)
{
    if (rep >= 81) return "ПРЕДАН";
    if (rep >= 61) return "ДРУГ";
    if (rep >= 31) return "ХОРОШО";
    if (rep >= -30) return "НОРМА";
    if (rep >= -60) return "ПЛОХО";
    if (rep >= -80) return "ВРАГ";
    return "НЕНАВИСТЬ";
}
    
public Color GetRelationshipColor(float rep)
{
    if (rep >= 81) return new Color(1f, 0.84f, 0f);     // Золотой
    if (rep >= 61) return new Color(0.2f, 0.8f, 0.2f); // Зелёный
    if (rep >= 31) return new Color(0.5f, 0.9f, 0.5f); // Светло-зелёный
    if (rep >= -30) return Color.gray;                  // Серый
    if (rep >= -60) return new Color(1f, 0.6f, 0.6f);   // Светло-красный
    if (rep >= -80) return new Color(1f, 0.3f, 0.3f);   // Красный
    return new Color(0.6f, 0f, 0f);                     // Тёмно-красный
}

    public float GetAverageReputation()
    {
        if (reputationValues.Count == 0) return 0;
        
        float sum = 0;
        foreach (var value in reputationValues.Values)
        {
            sum += value;
        }
        return sum / reputationValues.Count;
    }
    
    void DisplayAllReputation()
    {
        Debug.Log("=== РЕПУТАЦИЯ В НАЧАЛЕ ИГРЫ ===");
        foreach (string npcName in npcNames)
        {
            float rep = GetReputation(npcName);
            Debug.Log($"{npcName}: {rep} ({GetRelationshipLevel(rep)})");
        }
    }
    
    void LoadReputation()
    {
        // Для будущего сохранения
    }
    
    public void SaveReputation()
    {
        // Для будущего сохранения
    }
}