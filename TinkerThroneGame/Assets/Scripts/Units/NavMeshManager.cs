using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface navMeshSurface;

    public void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    private void Awake()
    {
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
    }
}