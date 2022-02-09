using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] Transform goal;
    [SerializeField] float agentRadius;
    NavMeshAgent navMeshAgent;
    
    public void UpdateGoal(Transform goal)
    {
        this.goal = goal;
        navMeshAgent.destination = goal.position;
    }

    public bool HasGoal()
    {
        return goal != null;
    }

    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if(Vector3.Distance(transform.position, goal.position) <=  agentRadius)
        {
            UpdateGoal(null);
        }
    }
}
