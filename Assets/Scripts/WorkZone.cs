using UnityEngine;

public class WorkZone : MonoBehaviour
{
    [Header("Параметры точки")]
    public string zoneName = "Лифт";
    public float staminaDrainRate = 1f;
    public float inspectionChanceBonus = 0f;
    public int moneyBonusPerHour = 0;
    public string playerTag = "Player";
    
    [Header("Индикатор зоны")]
    public Color zoneColor = Color.red;
    public bool showGizmo = true;
    
    private StaminaSystem staminaSystem;
    private bool playerInZone = false;

    void Start()
    {
        staminaSystem = FindObjectOfType<StaminaSystem>();
        Debug.Log($"WorkZone {zoneName} инициализирован. StaminaSystem найден: {staminaSystem != null}");
        
        // Проверить коллайдер
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"WorkZone {zoneName}: Нет коллайдера на объекте!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogError($"WorkZone {zoneName}: Коллайдер не является триггером! Установите Is Trigger = true");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[WorkZone {zoneName}] OnTriggerEnter: объект {other.name}, тег {other.tag}, слой {other.gameObject.layer}");
        
        if (!IsPlayerCollider(other))
        {
            Debug.Log($"[WorkZone {zoneName}] Не игрок: {other.name}");
            return;
        }

        playerInZone = true;
        Debug.Log($"Игрок вошёл в рабочую зону: {zoneName}");
        if (staminaSystem != null)
        {
            staminaSystem.SetWorkState(true, staminaDrainRate);
        }
        else
        {
            Debug.LogError("WorkZone: StaminaSystem не найден!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPlayerCollider(other))
            return;

        playerInZone = false;
        Debug.Log($"Игрок вышел из рабочей зоны: {zoneName}");
        if (staminaSystem != null)
        {
            staminaSystem.SetWorkState(false);
        }
    }

    private bool IsPlayerCollider(Collider other)
    {
        if (!string.IsNullOrEmpty(playerTag) && other.CompareTag(playerTag))
            return true;

        if (other.GetComponentInParent<PlayerMovement>() != null)
            return true;

        if (other.GetComponentInParent<StaminaSystem>() != null)
            return true;

        return false;
    }

    void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        Gizmos.color = playerInZone ? Color.green : zoneColor;
        
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
            }
        }
        
        // Текст с именем зоны
        UnityEditor.Handles.Label(transform.position, $"WorkZone: {zoneName}\nIn Zone: {playerInZone}");
    }
}