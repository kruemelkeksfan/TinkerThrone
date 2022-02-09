using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    List<Villager> villagers = new List<Villager>();
    JobsManager jobsManager;

    private void Start()
    {
        JobsManager.Initialize();
        jobsManager = JobsManager.GetInstance();
        villagers.AddRange(GetComponentsInChildren<Villager>());
        foreach(Villager villager in villagers)
        {
            jobsManager.AddIdleVillager(villager);
        }
    }
}
