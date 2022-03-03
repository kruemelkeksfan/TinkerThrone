using System.Collections.Generic;
using UnityEngine;

public class BuildingSpace : MonoBehaviour
{
    [SerializeField] private Collider otherSpaceCollider;
    [SerializeField] protected MeshRenderer meshRenderer;
    protected List<Collider> colliders = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other != otherSpaceCollider && !colliders.Contains(other))
        {
            colliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (colliders.Contains(other))
        {
            colliders.Remove(other);
        }
    }
}