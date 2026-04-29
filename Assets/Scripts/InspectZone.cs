using UnityEngine;

public class InspectZone : MonoBehaviour
{
    [Header("Настройки зоны проверки")]
    public string zoneName = "Зона проверки";
    public float angerPerSecond = 10f;
    public float staminaPenaltyPerSecond = 5f;
    
    private StaminaNew playerStamina;
    private bool isPlayerInside = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerStamina = other.GetComponent<StaminaNew>();
            isPlayerInside = true;
            Debug.Log($"[InspectZone] Игрок вошёл в зону {zoneName}");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerStamina = null;
            Debug.Log($"[InspectZone] Игрок вышел из зоны {zoneName}");
        }
    }
    
    void Update()
    {
        if (!isPlayerInside) return;
        if (playerStamina == null) return;
        
        if (playerStamina.isLookingDown)
        {
            // Тратим стамину
            playerStamina.AddStamina(-staminaPenaltyPerSecond * Time.deltaTime);
            
            // Добавляем гнев
            AngerSystem anger = FindAnyObjectByType<AngerSystem>();
            if (anger != null)
            {
                anger.AddDailyAnger(angerPerSecond * Time.deltaTime);
            }
        }
    }
}