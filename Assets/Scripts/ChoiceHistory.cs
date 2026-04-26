using UnityEngine;
using System.Collections.Generic;

public class ChoiceHistory : MonoBehaviour
{
    [System.Serializable]
    public class HistoryEntry
    {
        public int day;
        public string npcName;
        public string choice;      // "СОГЛАСИЕ" или "ОТКАЗ"
        public float reputationChange;
    }
    
    private List<HistoryEntry> history = new List<HistoryEntry>();
    
    public void AddEntry(int day, string npcName, string choice, float reputationChange)
    {
        HistoryEntry entry = new HistoryEntry
        {
            day = day,
            npcName = npcName,
            choice = choice,
            reputationChange = reputationChange
        };
        history.Add(entry);
        Debug.Log($"[История] День {day}: {npcName} → {choice} ({reputationChange:+0;-0})");
    }
    
    public List<HistoryEntry> GetHistory()
    {
        return history;
    }
    
    public void ClearHistory()
    {
        history.Clear();
    }
}