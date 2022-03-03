using System.Collections.Generic;
using UnityEngine;

public class JobsManager : MonoBehaviour
{
    private static JobsManager instance;
    private static JobUiController jobUi;
    private static LogisticsManager logisticsManager;
    private static VillagerManager villagerManager;

    private readonly List<Villager> idleVillagers = new();
    private readonly Dictionary<Villager, LogisticJob> logisticVillagers = new();
    private readonly List<Villager> idleLogisticVillagers = new();
    private readonly List<Villager> unassignedConstructionVillagers = new();
    private readonly List<ConstructionSite> constructionSites = new();
    private readonly List<Villager> assignedConstructionVillagers = new();


    //Dictionary<Villager, InventoryUser> productionVillager; //shadows of future greatness || wip

    private int neededLogisticVillagers = 0;
    private int neededConstructionVillagers = 0;

    public int NeededLogisticVillagers
    {
        get
        {
            return neededLogisticVillagers;
        }
        set
        {
            int villagerCount = villagerManager.GetVillagerCount();
            if (value > villagerCount)
            {
                neededLogisticVillagers = villagerCount;
            }
            else
            {
                neededLogisticVillagers = value;
            }
        }
    }
    public int NeededConstructionVillagers
    {
        get
        {
            return neededConstructionVillagers;
        }
        set
        {
            int villagerCount = villagerManager.GetVillagerCount();
            if (value > villagerCount)
            {
                neededConstructionVillagers = villagerCount;
            }
            else
            {
                neededConstructionVillagers = value;
            }
        }
    }
    public static JobsManager GetInstance()
    {
        return instance;
    }

    public static void Initialize(VillagerManager newVillagerManager)
    {
        jobUi = JobUiController.GetInstance();
        villagerManager = newVillagerManager;
        logisticsManager = LogisticsManager.GetInstance();
    }

    public List<Villager> GetIdleLogisticVillagers()
    {
        return idleLogisticVillagers;
    }

    private void Awake()
    {
        instance = this;
    }

    public void AssignJoblessVillager(Villager villager)
    {
        if (idleLogisticVillagers.Count + logisticVillagers.Count < NeededLogisticVillagers)
        {
            if (!logisticsManager.TryAssignJob(villager))
            {
                idleLogisticVillagers.Add(villager);
            }
        }
        else if (assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count < NeededConstructionVillagers)
        {
            unassignedConstructionVillagers.Add(villager);
        }
        else
        {
            idleVillagers.Add(villager);
        }
        jobUi.UpdateUi(idleVillagers.Count, 
                       logisticVillagers.Count + idleLogisticVillagers.Count, NeededLogisticVillagers,
                       assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count, NeededConstructionVillagers);
    }

    #region Logistic

    public void LogisticVillagerIdleToBusy(Villager villager, LogisticJob logisticJob)
    {
        if (logisticVillagers.ContainsKey(villager))
        {
            logisticVillagers[villager] = logisticJob;
        }
        else
        {
            logisticVillagers.Add(villager, logisticJob);
        }
    }

    public void LogisticVillagerBusyToIdle(Villager villager, bool completed = true)
    {
        if (!completed)
        {
            Debug.LogWarning("Logistic Job not finished");
            //drop it for now
        }
        if (idleLogisticVillagers.Count + logisticVillagers.Count > NeededLogisticVillagers)
        {
            logisticVillagers.Remove(villager);
            AssignJoblessVillager(villager);
        }
        //make villager idle Logistic if no job is found
        else if (!logisticsManager.TryAssignJob(villager))
        {
            logisticVillagers.Remove(villager);
            idleLogisticVillagers.Add(villager);
        }
        jobUi.UpdateUi(idleVillagers.Count, 
               logisticVillagers.Count + idleLogisticVillagers.Count, NeededLogisticVillagers,
               assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count, NeededConstructionVillagers);
    }

    public void IdleVillagerToLogistickVillager() // UI called
    {
        NeededLogisticVillagers++;
        if (idleLogisticVillagers.Count + logisticVillagers.Count < NeededLogisticVillagers && idleVillagers.Count > 0)
        {
            if (!logisticsManager.TryAssignJob(idleVillagers[0]))
            {
                idleLogisticVillagers.Add(idleVillagers[0]);
            }
            idleVillagers.RemoveAt(0);
        }
        jobUi.UpdateUi(idleVillagers.Count, 
               logisticVillagers.Count + idleLogisticVillagers.Count, NeededLogisticVillagers,
               assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count, NeededConstructionVillagers);
    }

    public void LogisticVillagerToIdleVillager() // UI called
    {
        if (NeededLogisticVillagers > 0)
        {
            NeededLogisticVillagers--;
        }
        if (idleLogisticVillagers.Count + logisticVillagers.Count > NeededLogisticVillagers && idleLogisticVillagers.Count > 0)
        {
            AssignJoblessVillager(idleLogisticVillagers[^1]);
            idleLogisticVillagers.RemoveAt(idleLogisticVillagers.Count - 1);
        }
        jobUi.UpdateUi(idleVillagers.Count, 
               logisticVillagers.Count + idleLogisticVillagers.Count, NeededLogisticVillagers,
               assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count, NeededConstructionVillagers);
    }

    #endregion

    #region Construction

    public void AddConstructionSite(ConstructionSite constructionSite)
    {
        constructionSites.Add(constructionSite);
        AssignConstructionVillagers();
    }
    public void RemoveConstructionSite(ConstructionSite constructionSite)
    {
        constructionSites.Remove(constructionSite);
        AssignConstructionVillagers();
    }

    public void AssignConstructionVillagers()
    {
        int maxVillagerPerSite;
        int villagersToDistribute;

        if(unassignedConstructionVillagers.Count + assignedConstructionVillagers.Count > NeededConstructionVillagers)
        {
            villagersToDistribute = NeededConstructionVillagers;
        }
        else
        {
            villagersToDistribute = unassignedConstructionVillagers.Count + assignedConstructionVillagers.Count;
        }

        int distributedVillagerCount = 0;
        int coveredSiteCount = 0;
        List<ConstructionSite> workingConstructionSites = new();
        foreach (ConstructionSite constructionSite in constructionSites)
        {
            maxVillagerPerSite = Mathf.CeilToInt((villagersToDistribute - distributedVillagerCount) / (float)(constructionSites.Count - coveredSiteCount));
            distributedVillagerCount += maxVillagerPerSite;

            int maxToCurrentDiff = constructionSite.GetAssignedVillagers() - maxVillagerPerSite;
            if (maxToCurrentDiff > 0)
            {
                constructionSite.RequestVillagers(maxToCurrentDiff);
            }
            if (!constructionSite.IsFinishedAssigningJobs())
            {
                workingConstructionSites.Add(constructionSite);
            }
        }

        if(unassignedConstructionVillagers.Count == 0)
        {
            return;
        }

        distributedVillagerCount = 0;
        coveredSiteCount = 0;
        foreach (ConstructionSite constructionSite in workingConstructionSites)
        {
            maxVillagerPerSite = Mathf.CeilToInt((villagersToDistribute - distributedVillagerCount) / (float)(workingConstructionSites.Count - coveredSiteCount));

            int maxToCurrentDiff = maxVillagerPerSite - constructionSite.GetAssignedVillagers();
            if (maxToCurrentDiff > 0)
            {
                if (maxToCurrentDiff > unassignedConstructionVillagers.Count)
                {
                    if (unassignedConstructionVillagers.Count <= 0)
                    {
                        break;
                    }
                    maxToCurrentDiff = unassignedConstructionVillagers.Count;
                }
                for (int i = 0; i < maxToCurrentDiff; i++)
                {
                    if (constructionSite.AssignVillager(unassignedConstructionVillagers[^1]))
                    {
                        distributedVillagerCount++;
                        assignedConstructionVillagers.Add(unassignedConstructionVillagers[^1]);
                        unassignedConstructionVillagers.RemoveAt(unassignedConstructionVillagers.Count - 1);
                    }
                }
            }
        }
    }

    public void UnassignVillager(Villager villager, bool preventReassigning = false)
    {
        if (assignedConstructionVillagers.Contains(villager))
        {
            if (NeededConstructionVillagers < assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count)
            {
                AssignJoblessVillager(villager);
            }
            else
            {
                unassignedConstructionVillagers.Add(villager);
                if (!preventReassigning)
                {
                    AssignConstructionVillagers();
                }
            }
            assignedConstructionVillagers.Remove(villager);
        }
        jobUi.UpdateUi(idleVillagers.Count, 
               logisticVillagers.Count + idleLogisticVillagers.Count, NeededLogisticVillagers,
               assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count, NeededConstructionVillagers);
    }

    public void IdleVillagerToConstructionVillager() // UI called
    {
        NeededConstructionVillagers++;

        if (assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count < NeededConstructionVillagers && idleVillagers.Count > 0)
        {
            unassignedConstructionVillagers.Add(idleVillagers[^1]);
            idleVillagers.RemoveAt(idleVillagers.Count - 1);
            AssignConstructionVillagers();
        }
        jobUi.UpdateUi(idleVillagers.Count, 
               logisticVillagers.Count + idleLogisticVillagers.Count, NeededLogisticVillagers,
               assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count, NeededConstructionVillagers);
    }

    public void ConstructionVillagerToIdleVillager() // UI called
    {
        if (NeededConstructionVillagers > 0)
        {
            NeededConstructionVillagers--;
        }
        if (assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count > NeededConstructionVillagers
            && unassignedConstructionVillagers.Count > 0)
        {
            AssignJoblessVillager(unassignedConstructionVillagers[^1]);
            unassignedConstructionVillagers.RemoveAt(unassignedConstructionVillagers.Count - 1);
        }
        else
        {
            AssignConstructionVillagers();
        }
        jobUi.UpdateUi(idleVillagers.Count, 
               logisticVillagers.Count + idleLogisticVillagers.Count, NeededLogisticVillagers,
               assignedConstructionVillagers.Count + unassignedConstructionVillagers.Count, NeededConstructionVillagers);
    }

    #endregion
}