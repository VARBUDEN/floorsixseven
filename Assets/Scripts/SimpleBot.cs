using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class SimpleBot : MonoBehaviour
{
    [Header("Параметры")]
    public string botName = "Бот";
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    
    private NavMeshAgent agent;
    private List<Vector3> waypoints = new List<Vector3>();
    private bool isWaiting = false;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed = moveSpeed;
        
        FindAllWaypoints();
        
        if (waypoints.Count > 0) ChooseRandomTarget();
    }
    
    void FindAllWaypoints()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("WayPoint");
        
        foreach (GameObject wp in waypointObjects)
        {
            waypoints.Add(wp.transform.position);
        }
        
        Debug.Log($"[{botName}] Найдено {waypoints.Count} точек WayPoint");
    }
    
    void ChooseRandomTarget()
    {
        if (isWaiting || waypoints.Count == 0) return;
        
        Vector3 targetPoint = waypoints[Random.Range(0, waypoints.Count)];
        if (agent != null) agent.SetDestination(targetPoint);
    }
    
    void Update()
    {
        if (agent == null || isWaiting || waypoints.Count == 0) return;
        
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }
    
    System.Collections.IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        ChooseRandomTarget();
    }
}