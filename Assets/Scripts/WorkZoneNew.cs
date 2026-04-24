using UnityEngine;

public class WorkZoneNew : MonoBehaviour
{
    [Header("Настройки зоны")]
    public string zoneName = "Рабочая точка";
    public string zoneType = "work";

    private StaminaNew stamina;

    void Start()
    {
        stamina = FindFirstObjectByType<StaminaNew>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[WorkZone] Игрок вошёл в зону: {zoneName}");

            if (stamina != null)
            {
                stamina.isAtWork = true;
                stamina.SetZoneType(zoneType);
                
                if (CharacterSelect.selectedCharacter == CharacterSelect.Character.Svistik && zoneType == "eight")
                {
                    stamina.isOnBreak = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[WorkZone] Игрок вышел из зоны: {zoneName}");

            if (stamina != null)
            {
                stamina.isAtWork = false;
                
                if (CharacterSelect.selectedCharacter == CharacterSelect.Character.Svistik && zoneType == "eight")
                {
                    stamina.isOnBreak = false;
                }
            }
        }
    }
}