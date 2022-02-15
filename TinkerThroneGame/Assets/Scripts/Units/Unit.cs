using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Unit : InventoryUser
{
    [SerializeField] Vector3 goal;
    [SerializeField] float agentRadius;
    bool hasGoal;
    NavMeshAgent navMeshAgent;
    
    public void UpdateGoal(Vector3 goal)
    {
        this.goal = goal;
        navMeshAgent.destination = goal;
        navMeshAgent.stoppingDistance = agentRadius * 0.5f;
        hasGoal = true;
    }

    public bool HasGoal()
    {
        return hasGoal;
    }

    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        //Debug.Log(Vector3.Distance(transform.position, goal));
        if(hasGoal && Vector3.Distance(transform.position, goal) <=  agentRadius)
        {
            hasGoal = false;
        }
    }
}
