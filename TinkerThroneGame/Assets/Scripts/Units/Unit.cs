using UnityEngine;
using UnityEngine.AI;

public class Unit : InventoryUser
{
    [SerializeField] private float agentRadius;

    private NavMeshAgent navMeshAgent;
    [SerializeField] private Vector3 goal;
    [SerializeField] float distance;
    private bool hasGoal;

    public void UpdateGoal(Vector3 goal)
    {
        this.goal = goal;
        navMeshAgent.destination = goal;
        navMeshAgent.stoppingDistance = agentRadius;
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
        distance = Vector3.Distance(transform.position, new Vector3(goal.x, transform.position.y, goal.z));
        if (hasGoal && distance <= agentRadius*2)
        {
            hasGoal = false;
        }
    }
}