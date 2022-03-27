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
        NavMeshHit hit;
        Vector3 result;
        hasGoal = false;
        int hight = 10;
        while (!hasGoal)
        {
            if (NavMesh.SamplePosition(goal, out hit, hight, NavMesh.AllAreas))
            {
                result = hit.position;
                this.goal = result;
                navMeshAgent.destination = result;
                navMeshAgent.stoppingDistance = agentRadius;
                hasGoal = true;
            }
            else
            {
                hight += hight;
                if(hight >= 10000)
                {
                    break;
                }
            }
        }
        
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