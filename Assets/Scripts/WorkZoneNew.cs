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
    private DayCycleSystem dayCycle;

    void Start()
    {
        stamina = FindAnyObjectByType<StaminaNew>();
        dayCycle = FindAnyObjectByType<DayCycleSystem>();
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
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (stamina == null) return;
        if (dayCycle == null) return;
        
        string requiredZone = dayCycle.GetCurrentRequiredZone();
        
        // Проверяем, на своей ли точке игрок по расписанию
        bool isCorrectZone = (zoneType == requiredZone);
        
        if (isCorrectZone && requiredZone != "break")
        {
            // На своей точке - работаем, тратим стамину
            stamina.isAtWork = true;
            stamina.SetZoneType(zoneType);
        }
        else
        {
            // Не на своей точке - не работаем, стамина не тратится
            stamina.isAtWork = false;
        }
    }
}