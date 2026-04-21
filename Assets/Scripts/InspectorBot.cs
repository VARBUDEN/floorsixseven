using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class InspectorBot : MonoBehaviour
{
    [Header("Параметры бота")]
    public string botName = "Арман";
    public float detectionChance = 0.7f;
    public int staminaPenalty = 10;
    public int angerPenalty = 20;
    
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
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
        }
        
        // Находим все зоны
        allZones = FindObjectsOfType<WorkZoneNew>();
        ChooseRandomTargetZone();
    }
    
    void Update()
    {
        if (agent == null || !isMoving) return;
        
        // Если достиг целевой зоны
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
            Debug.Log($"[{botName}] Идёт к зоне: {targetZone.zoneName}");
        }
    }
    
    void OnReachZone()
    {
        Debug.Log($"[{botName}] Прибыл в зону: {targetZone?.zoneName}");
        
        // Останавливаемся на несколько секунд
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
            // Выходим из старой зоны
            if (currentZone != null && currentZone != zone)
            {
                if (currentZone.inspectorBots != null)
                    currentZone.inspectorBots.Remove(this);
            }
            
            // Входим в новую зону
            currentZone = zone;
            if (zone.inspectorBots == null)
                zone.inspectorBots = new List<InspectorBot>();
            
            if (!zone.inspectorBots.Contains(this))
                zone.inspectorBots.Add(this);
            
            Debug.Log($"[{botName}] Вошёл в зону: {zone.zoneName}");
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
            Debug.Log($"[{botName}] Вышел из зоны: {zone.zoneName}");
        }
    }
    
    public void CatchPlayer()
    {
        Debug.Log($"[{botName}] ПОЙМАЛ ИГРОКА! Штраф: {staminaPenalty} стамины, {angerPenalty} гнева");
    }
}