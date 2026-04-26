using UnityEngine;

public class DailyBuffSystem : MonoBehaviour
{
    [Header("=== ТЕКУЩИЙ БАФФ ===")]
    public bool hasActiveBuff = false;
    public string buffType = "";
    public float buffValue = 0f;
    
    private StaminaNew stamina;
    private ReputationSystem reputation;
    
    void Start()
    {
        stamina = FindAnyObjectByType<StaminaNew>();
        reputation = FindAnyObjectByType<ReputationSystem>();
        
        // Применяем бафф в начале дня
        ApplyBuffIfExists();
    }
    
    public void SetBuff(string type, float value)
    {
        hasActiveBuff = true;
        buffType = type;
        buffValue = value;
        
        Debug.Log($"[Бафф] Установлен: {type} = {value:+0;-0}");
        ApplyBuffIfExists();
    }
    
    public void ClearBuff()
    {
        hasActiveBuff = false;
        buffType = "";
        buffValue = 0f;
        Debug.Log("[Бафф] Очищен");
    }
    
    void ApplyBuffIfExists()
    {
        if (!hasActiveBuff) return;
        
        switch (buffType)
        {
            case "StaminaMax":
                if (stamina != null)
                    stamina.SetMaxStamina(stamina.maxStamina + buffValue);
                break;
            case "BreakRestore":
                if (stamina != null)
                    stamina.breakRestore += buffValue;
                break;
            case "LookDownRestore":
                if (stamina != null)
                    stamina.lookDownRestoreRate += buffValue;
                break;
            case "PromoBonus":
                Debug.Log($"[Бафф] Промо бонус: +{buffValue}%");
                break;
            default:
                Debug.Log($"[Бафф] Неизвестный тип: {buffType}");
                break;
        }
    }
}