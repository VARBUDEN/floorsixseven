using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NeutralBot : MonoBehaviour
{
    [Header("Параметры нейтрального бота")]
    public string botName = "Посетитель";
    public bool canWitness = true;      // может ли стать свидетелем
    public float witnessChance = 0.3f;  // шанс сообщить о проебе
    
    [Header("Движение")]
    public float moveSpeed = 2f;
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
            Debug.Log($"[{botName}] Идёт к зоне: {targetZone.zoneName}");
        }
    }
    
    void OnReachZone()
    {
        Debug.Log($"[{botName}] Прибыл в зону: {targetZone?.zoneName}");
        
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
                if (currentZone.neutralBots != null)
                    currentZone.neutralBots.Remove(this);
            }
            
            currentZone = zone;
            if (zone.neutralBots == null)
                zone.neutralBots = new List<NeutralBot>();
            
            if (!zone.neutralBots.Contains(this))
                zone.neutralBots.Add(this);
            
            Debug.Log($"[{botName}] Вошёл в зону: {zone.zoneName}");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        WorkZoneNew zone = other.GetComponent<WorkZoneNew>();
        if (zone != null && currentZone == zone)
        {
            if (zone.neutralBots != null)
                zone.neutralBots.Remove(this);
            
            currentZone = null;
            Debug.Log($"[{botName}] Вышел из зоны: {zone.zoneName}");
        }
    }
}