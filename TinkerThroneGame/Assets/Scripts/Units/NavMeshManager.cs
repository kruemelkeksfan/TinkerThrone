using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    static NavMeshManager instance;

    [SerializeField] private NavMeshSurface navMeshSurface;

    public static NavMeshManager GetInstance()
    {
        return instance;
    }

    public void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    private void Awake()
    {
        instance = this;
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
    }
}