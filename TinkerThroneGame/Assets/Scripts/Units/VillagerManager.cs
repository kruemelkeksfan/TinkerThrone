using System.Collections.Generic;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    private readonly List<Villager> villagers = new();
    private JobsManager jobsManager;

    public int GetVillagerCount()
    {
        return villagers.Count;
    }

    private void Start()
    {
        JobsManager.Initialize(this);
        jobsManager = JobsManager.GetInstance();
        villagers.AddRange(GetComponentsInChildren<Villager>());
        foreach (Villager villager in villagers)
        {
            jobsManager.AssignJoblessVillager(villager);
        }
    }
}