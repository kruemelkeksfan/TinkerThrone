using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobsManager : MonoBehaviour
{
    static JobsManager instance;
    static JobUiController jobUi;

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

    public void IdleVillagerToLogistickVillager()
    {
        if(idleVillagers.Count > 0)
        {
            idleLogisticVillagers.Add(idleVillagers[0]);
            idleVillagers.RemoveAt(0);
        }
        jobUi.UpdateUi(idleVillagers.Count, logisticVillagers.Count + idleLogisticVillagers.Count);
    }

    public void LogisticVillagerToIdleVillager()
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
