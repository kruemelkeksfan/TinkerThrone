using System.Collections.Generic;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    private readonly List<Villager> villagers = new();
    private JobsManager jobsManager;

    private void Start()
    {
        JobsManager.Initialize();
        jobsManager = JobsManager.GetInstance();
        villagers.AddRange(GetComponentsInChildren<Villager>());
        foreach (Villager villager in villagers)
        {
            jobsManager.AddIdleVillager(villager);
        }
    }
}