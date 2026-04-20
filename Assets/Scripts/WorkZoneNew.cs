using UnityEngine;

public class WorkZoneNew : MonoBehaviour
{
    [Header("Настройки зоны")]
    public string zoneName = "Рабочая точка";
    public string zoneType = "work";  // lift, eight, promo, perek, navig, kalizeum
    
    private StaminaNew stamina;
    
    void Start()
    {
        stamina = FindObjectOfType<StaminaNew>();
        if (stamina == null)
        {
            Debug.LogError("[WorkZone] StaminaNew не найден!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[WorkZone] Вошёл в зону: {zoneName} (тип: {zoneType})");
            
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
            Debug.Log($"[WorkZone] Вышел из зоны: {zoneName}");
            
            if (stamina != null)
            {
                stamina.isAtWork = false;
            }
        }
    }
}