using UnityEngine;
using System.Collections.Generic;

public class WorkZoneNew : MonoBehaviour
{
    [Header("Настройки зоны")]
    public string zoneName = "Рабочая точка";
    public string zoneType = "work";

    [Header("Состояние зоны")]
    public bool isOccupiedByPlayer = false;
    public List<InspectorBot> inspectorBots = new List<InspectorBot>();  // проверяющие в зоне
    public List<NeutralBot> neutralBots = new List<NeutralBot>();        // нейтральные в зоне

    private StaminaNew stamina;
    private bool isPlayerInZone = false;  // для отслеживания входа/выхода

    void Start()
    {
        stamina = FindObjectOfType<StaminaNew>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOccupiedByPlayer = true;
            isPlayerInZone = true;
            Debug.Log($"[WorkZone] Игрок вошёл в зону: {zoneName}");

            if (stamina != null)
            {
                stamina.isAtWork = true;
                stamina.SetZoneType(zoneType);
                
                // ★★★ СВИСТИК: 8-ка работает как перерыв ★★★
                if (CharacterSelect.selectedCharacter == CharacterSelect.Character.Svistik && zoneType == "eight")
                {
                    stamina.isOnBreak = true;
                    Debug.Log($"[WorkZone] Свистик на 8-ке! Режим перерыва активирован.");
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOccupiedByPlayer = false;
            isPlayerInZone = false;
            Debug.Log($"[WorkZone] Игрок вышел из зоны: {zoneName}");

            if (stamina != null)
            {
                stamina.isAtWork = false;
                
                // ★★★ ВЫКЛЮЧАЕМ РЕЖИМ ПЕРЕРЫВА, если вышли из 8-ки ★★★
                if (CharacterSelect.selectedCharacter == CharacterSelect.Character.Svistik && zoneType == "eight")
                {
                    stamina.isOnBreak = false;
                    Debug.Log($"[WorkZone] Свистик покинул 8-ку. Режим перерыва выключен.");
                }
            }
        }
    }

    // Этот метод можно удалить, если не нужен (логика Свистика теперь в OnTriggerEnter)
    // void OnTriggerStay(Collider other) { ... }
}