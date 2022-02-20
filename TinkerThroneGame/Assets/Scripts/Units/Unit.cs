using UnityEngine;
using UnityEngine.AI;

public class Unit : InventoryUser
{
    [SerializeField] private float agentRadius;

    private NavMeshAgent navMeshAgent;
    private Vector3 goal;
    private bool hasGoal;

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
        if (hasGoal && Vector3.Distance(transform.position, goal) <= agentRadius)
        {
            hasGoal = false;
        }
    }
}