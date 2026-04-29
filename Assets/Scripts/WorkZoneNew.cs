using UnityEngine;
using System.Collections.Generic;

public class WorkZoneNew : MonoBehaviour
{
    [Header("Настройки зоны")]
    public string zoneName = "Рабочая точка";
    public string zoneType = "work";

    [Header("Состояние зоны")]
    public bool isOccupiedByPlayer = false;
    public List<SimpleBot> inspectorBots = new List<SimpleBot>();

    private StaminaNew stamina;

    void Start()
    {
        stamina = FindAnyObjectByType<StaminaNew>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOccupiedByPlayer = true;
            Debug.Log($"[WorkZone] Игрок вошёл в зону: {zoneName}");
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
            stamina.isAtWork = false;  // ← ДОБАВИТЬ ЭТУ СТРОКУ
        }
    }
}

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (stamina == null) return;
        
        stamina.isAtWork = true;
        stamina.SetZoneType(zoneType);
    }
}