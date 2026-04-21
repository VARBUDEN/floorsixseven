using UnityEngine;
using System.Collections.Generic;

public class WorkZoneNew : MonoBehaviour
{
    [Header("Настройки зоны")]
    public string zoneName = "Рабочая точка";
    public string zoneType = "work";
    
    [Header("Состояние зоны")]
    public List<InspectorBot> inspectorBots = new List<InspectorBot>();
    public bool isOccupiedByPlayer = false;
    public List<InspectorBot> botsInZone = new List<InspectorBot>();
    public List<NeutralBot> neutralBots = new List<NeutralBot>();
    
    private StaminaNew stamina;
    
    void Start()
    {
        stamina = FindObjectOfType<StaminaNew>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOccupiedByPlayer = true;
            Debug.Log($"[WorkZone] Игрок вошёл в зону: {zoneName}");
            
            if (stamina != null)
            {
                stamina.isAtWork = true;
                stamina.SetZoneType(zoneType);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOccupiedByPlayer = false;
            Debug.Log($"[WorkZone] Игрок вышел из зоны: {zoneName}");
            
            if (stamina != null)
            {
                stamina.isAtWork = false;
            }
        }
    }
}