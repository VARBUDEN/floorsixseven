using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class InspectorBot : MonoBehaviour
{
    [Header("Параметры бота")]
    public string botName = "Арман";
    public float detectionChance = 0.7f;
    public int staminaPenalty = 10;
    
    [Header("Движение")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 0.5f;
    public float waitTimeAtZone = 3f;

    [Header("Состояния")]
    public WorkZoneNew currentZone;
    public bool isMoving = true;

    private NavMeshAgent agent;
    private WorkZoneNew[] allZones;
    private WorkZoneNew targetZone;
    private bool hasCaughtPlayer = false;  // чтобы не ловить дважды

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        allZones = FindObjectsOfType<WorkZoneNew>();
        ChooseRandomTargetZone();
    }

    void Update()
    {
        if (agent == null || !isMoving) return;

        if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
        {
            OnReachZone();
        }
    }

    void ChooseRandomTargetZone()
    {
        if (allZones == null || allZones.Length == 0) return;

        targetZone = allZones[Random.Range(0, allZones.Length)];

        if (agent != null && targetZone != null)
        {
            agent.SetDestination(targetZone.transform.position);
        }
    }

    void OnReachZone()
    {
        isMoving = false;
        Invoke("ResumeMoving", waitTimeAtZone);
    }

    void ResumeMoving()
    {
        isMoving = true;
        ChooseRandomTargetZone();
    }

    void OnTriggerEnter(Collider other)
    {
        WorkZoneNew zone = other.GetComponent<WorkZoneNew>();
        if (zone != null)
        {
            if (currentZone != null && currentZone != zone)
            {
                if (currentZone.inspectorBots != null)
                    currentZone.inspectorBots.Remove(this);
            }

            currentZone = zone;
            if (zone.inspectorBots == null)
                zone.inspectorBots = new List<InspectorBot>();

            if (!zone.inspectorBots.Contains(this))
                zone.inspectorBots.Add(this);

            // ★★★ НОВАЯ ЛОГИКА: проверяем игрока при входе в зону ★★★
            CheckPlayerInZone();
        }
    }

    void OnTriggerExit(Collider other)
    {
        WorkZoneNew zone = other.GetComponent<WorkZoneNew>();
        if (zone != null && currentZone == zone)
        {
            if (zone.inspectorBots != null)
                zone.inspectorBots.Remove(this);

            currentZone = null;
        }
    }

    /// <summary>
    /// Проверяет, есть ли игрок в этой зоне и смотрит ли он вниз
    /// </summary>
    void CheckPlayerInZone()
    {
        if (currentZone == null) return;
        if (!currentZone.isOccupiedByPlayer) return;  // игрока нет в зоне

        // Находим игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        StaminaNew stamina = player.GetComponent<StaminaNew>();
        if (stamina == null) return;

        // ★★★ ЕСЛИ ИГРОК СМОТРИТ В ТЕЛЕФОН (ВНИЗ) - ЛОВИМ ★★★
        if (stamina.isLookingDown && !hasCaughtPlayer)
        {
            CatchPlayer(stamina);
        }
    }

    void CatchPlayer(StaminaNew stamina)
    {
        hasCaughtPlayer = true;
        
        Debug.Log($"[{botName}] ЗАМЕТИЛ, ЧТО ИГРОК В ТЕЛЕФОНЕ! ЛОВИТ!");

        // Штраф стамины
        stamina.AddStamina(-staminaPenalty);

        // Штраф гнева
        AngerSystem anger = FindObjectOfType<AngerSystem>();
        if (anger != null)
        {
            anger.AddDailyAnger(20f);
        }

        // Не ловим снова в этой зоне
        Invoke("ResetCatchFlag", 5f);
    }

    void ResetCatchFlag()
    {
        hasCaughtPlayer = false;
    }
}