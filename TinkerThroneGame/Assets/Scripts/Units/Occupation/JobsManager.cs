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
        {
            logisticVillagers.Add(villager, logisticJob);
        }
    }

    public void LogisticVillagerBusyToIdle(Villager villager, LogisticJob logisticJob, bool completed = true)
    {
        if (!completed)
        {
            //drop it for now
        }
        //make villager idle if no job is found
        if (!logisticsManager.TryAssignJob(villager))
        {
            logisticVillagers.Remove(villager);
            idleLogisticVillagers.Add(villager);
        }
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
        if (logisticVillagers.ContainsKey(villager))
        {
            //stop current work
            logisticVillagers.Remove(villager);
        }
        else if (idleLogisticVillagers.Contains(villager))
        {
            idleLogisticVillagers.Remove(villager);
        }
        idleVillagers.Add(villager);
        jobUi.UpdateUi(idleVillagers.Count, logisticVillagers.Count + idleLogisticVillagers.Count);
    }

    public void IdleVillagerToLogistickVillager() // UI called
    {
        if(idleVillagers.Count > 0)
        {
            if (!logisticsManager.TryAssignJob(idleVillagers[0]))
            {
                idleLogisticVillagers.Add(idleVillagers[0]);
            }
            idleVillagers.RemoveAt(0);
        }
        jobUi.UpdateUi(idleVillagers.Count, logisticVillagers.Count + idleLogisticVillagers.Count);
    }

    public void LogisticVillagerToIdleVillager() // UI called
    {
        if (idleLogisticVillagers.Count > 0)
        {
            idleVillagers.Add(idleLogisticVillagers[idleLogisticVillagers.Count - 1]);
            idleLogisticVillagers.RemoveAt(idleLogisticVillagers.Count - 1);
        }
        else if (logisticVillagers.Count > 0)
        {
            List<LogisticJob> logisticJobs = new List<LogisticJob>();
            logisticJobs.AddRange(logisticVillagers.Values);
            logisticJobs.Sort();
            //stop current work
            idleVillagers.Add(logisticJobs[0].villager);
            logisticVillagers.Remove(logisticJobs[0].villager);
        }
        jobUi.UpdateUi(idleVillagers.Count, logisticVillagers.Count + idleLogisticVillagers.Count);
    }
}
