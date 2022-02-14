using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobsManager : MonoBehaviour
{
    static JobsManager instance;
    static JobUiController jobUi;
    static LogisticsManager logisticsManager;

    [SerializeField] List<Villager> idleVillagers = new List<Villager>();
    [SerializeField] Dictionary<Villager, LogisticJob> logisticVillagers = new Dictionary<Villager, LogisticJob>();
    [SerializeField] List<Villager> idleLogisticVillagers = new List<Villager>();
    int neededIdleVillagers = 0;
    int neededLogisticVillager = 0;
    

    //Dictionary<Villager, InventoryUser> productionVillager; //shadows of future greatness || wip

    public static JobsManager GetInstance()
    {
        return instance;
    }

    public static void  Initialize()
    {
        jobUi = JobUiController.GetInstance();
        logisticsManager = LogisticsManager.GetInstance();
    }

    public List<Villager> GetIdleLogisticVillagers()
    {
        return idleLogisticVillagers;
    }

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

    public void LogisticVillagerBusyToIdle(Villager villager, LogisticJob logisticJob, bool completed = true)
    {
        if (!completed)
        {
            Debug.LogWarning("Logistic Job not finished");
            //drop it for now
        }
        if (idleVillagers.Count < neededIdleVillagers && idleLogisticVillagers.Count + logisticVillagers.Count > neededLogisticVillager)
        {
            logisticVillagers.Remove(villager);
            idleVillagers.Add(villager);
        }
        //make villager idle Logistic if no job is found
        else if (!logisticsManager.TryAssignJob(villager))
        {
            logisticVillagers.Remove(villager);
            idleLogisticVillagers.Add(villager);
        }
        jobUi.UpdateUi(idleVillagers.Count, neededIdleVillagers, logisticVillagers.Count + idleLogisticVillagers.Count, neededLogisticVillager);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
    }


    public void AddIdleVillager(Villager villager)
    {
        idleVillagers.Add(villager);
        jobUi.UpdateUi(idleVillagers.Count, neededIdleVillagers, logisticVillagers.Count + idleLogisticVillagers.Count, neededLogisticVillager);
    }

    public void IdleVillagerToLogistickVillager() // UI called
    {
        neededLogisticVillager++;
        if(neededIdleVillagers > 0)
        {
            neededIdleVillagers--;
        }
        if (idleVillagers.Count > neededIdleVillagers && idleLogisticVillagers.Count + logisticVillagers.Count < neededLogisticVillager && idleVillagers.Count > 0)
        {
            if (!logisticsManager.TryAssignJob(idleVillagers[0]))
            {
                idleLogisticVillagers.Add(idleVillagers[0]);
            }
            idleVillagers.RemoveAt(0);
        }
        jobUi.UpdateUi(idleVillagers.Count, neededIdleVillagers, logisticVillagers.Count + idleLogisticVillagers.Count, neededLogisticVillager);
    }

    public void LogisticVillagerToIdleVillager() // UI called
    {
        neededIdleVillagers++;
        if(neededLogisticVillager > 0)
        {
            neededLogisticVillager--;
        }
        if (idleVillagers.Count < neededIdleVillagers && idleLogisticVillagers.Count + logisticVillagers.Count > neededLogisticVillager && idleLogisticVillagers.Count > 0)
        {
            idleVillagers.Add(idleLogisticVillagers[idleLogisticVillagers.Count - 1]);
            idleLogisticVillagers.RemoveAt(idleLogisticVillagers.Count - 1);
        }
        jobUi.UpdateUi(idleVillagers.Count, neededIdleVillagers, logisticVillagers.Count + idleLogisticVillagers.Count, neededLogisticVillager);
    }
}
