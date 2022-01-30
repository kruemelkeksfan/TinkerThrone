using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;

    private void Awake()
    {
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
    }

    public void UpdateNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}
