using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticsManager : MonoBehaviour
{
    private static LogisticsManager instance = null;

    [SerializeField] private int villagerToPoolMulti = 3;
    public List<LogisticsUser> logisticUsers = new();
    public Dictionary<float, List<LogisticJob>> availableJobs = new();
    private JobsManager jobsManager;

    public static LogisticsManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        jobsManager = JobsManager.GetInstance();
        StartCoroutine(UpdateCycle());
    }

    public void AddInventory(LogisticsUser inventory)
    {
        if (!logisticUsers.Contains(inventory))
        {
            logisticUsers.Add(inventory);
            UpdateLogisticsJobs();
        }
    }

    public void RemoveInventory(LogisticsUser inventory)
    {
        //TODO check for running jobs
        if (logisticUsers.Contains(inventory))
        {
            logisticUsers.Remove(inventory);
        }
    }

    public bool TryAssignJob(Villager villager)
    {
        if (availableJobs.Count == 0) return false;
        if (villager.HasJob) return false;

        float currentPriority = WorldConsts.PRIORITY_CAP;

        while (availableJobs.Count != 0)
        {
            while (!availableJobs.ContainsKey(currentPriority) && currentPriority > 0)
            {
                currentPriority -= WorldConsts.PRIORITY_SETP_SIZE;
            }

            if (!availableJobs.ContainsKey(currentPriority)) return false;

            LogisticJob selectedJob;
            if (availableJobs[currentPriority].Count > 1)
            {
                selectedJob = availableJobs[currentPriority][Random.Range(0, availableJobs[currentPriority].Count - 1)];
            }
            else
            {
                selectedJob  = availableJobs[currentPriority][0];
            }

            if (selectedJob.ReserveStack())
            {
                if (availableJobs[currentPriority].Count > 1)
                {
                    availableJobs[currentPriority].Remove(selectedJob);
                }
                else
                {
                    availableJobs.Remove(currentPriority);
                }
                villager.StartCoroutine(villager.DoLogisticJob(selectedJob));
                jobsManager.LogisticVillagerIdleToBusy(villager, selectedJob);
                return true;
            }
        }
        return false;
    }

    private IEnumerator UpdateCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            UpdateLogisticsJobs();
            AssignJobs();
        }
    }

    public void UpdateLogisticsJobs()
    {
        List<LogisticCommission> commissions = new();
        availableJobs = new();

        foreach (LogisticsUser logisticUser in logisticUsers)
        {
            commissions.AddRange(logisticUser.UpdateLogisticCommissions());
        }

        if (commissions.Count <= 1) return;
        commissions.Sort();

        LogisticCommission usedCommission = new();
        for(int i = commissions.Count - 1; i >= 0 && availableJobs.Count < jobsManager.NeededLogisticVillagers * villagerToPoolMulti; i--)
        {
            if (commissions[i].TakeableAmount == 0) continue;
            uint targetMoveAmount = WorldConsts.STANDART_UNIT_CAPACITY.ToAmount(commissions[i].goodName);
            if(targetMoveAmount > commissions[i].TakeableAmount)
            {
                targetMoveAmount = commissions[i].TakeableAmount;
            }
            bool foundCommission = false;
            foreach(LogisticCommission inCommission in commissions)
            {
                if (inCommission.StoreableAmount == 0 || (!foundCommission && inCommission.Priority >= commissions[i].Priority) || (foundCommission && inCommission.Priority > usedCommission.Priority))
                {
                    continue;
                }
                if (inCommission.goodName == commissions[i].goodName)
                {
                    if (!foundCommission)
                    {
                        foundCommission = true;
                        usedCommission = inCommission;
                    }
                    else if (inCommission.Priority == usedCommission.Priority)
                    {
                        if (inCommission.StoreableAmount < targetMoveAmount && inCommission.StoreableAmount < usedCommission.StoreableAmount) return;


                        if (usedCommission.StoreableAmount < targetMoveAmount && inCommission.StoreableAmount > targetMoveAmount)
                        {
                            usedCommission = inCommission;
                        }
                        else if (Vector3.Distance(inCommission.sourceInventory.GetLogisticPosition(), commissions[i].sourceInventory.GetLogisticPosition())
                                    < Vector3.Distance(usedCommission.sourceInventory.GetLogisticPosition(), commissions[i].sourceInventory.GetLogisticPosition()))
                        {
                            usedCommission = inCommission;
                        }
                    }
                }
            }
            if (foundCommission) 
            { 
                int index = commissions.IndexOf(usedCommission);
                uint amount;
                if (usedCommission.StoreableAmount > commissions[i].TakeableAmount)
                {
                    if(commissions[i].TakeableAmount > targetMoveAmount)
                    {
                        amount = targetMoveAmount;
                    }
                    else
                    {
                        amount = commissions[i].TakeableAmount;
                    }
                }
                else
                {
                    if (usedCommission.StoreableAmount > targetMoveAmount)
                    {
                        amount = targetMoveAmount;
                    }
                    else
                    {
                        amount = usedCommission.StoreableAmount;
                    }
                }
                commissions[index].StoreAmount(commissions[i].TakeableAmount);
                commissions[i].TakeAmount(commissions[i].TakeableAmount);
                LogisticJob logisticJob = new(usedCommission, commissions[i], amount);
                if (availableJobs.ContainsKey(logisticJob.Priority))
                {
                    availableJobs[logisticJob.Priority].Add(logisticJob);
                }
                else
                {
                    availableJobs.Add(logisticJob.Priority, new List<LogisticJob>() { logisticJob });
                }
            }
        }
        Debug.Log("JobList");
        foreach(float prio in availableJobs.Keys)
        {
            Debug.Log("Prio: " + prio);
            foreach(LogisticJob job in availableJobs[prio])
            {
                Debug.Log("LogisticJob: " + job.Stack.amount + " " + job.Stack.goodName + " from: " + job.SourceInventory.name + " to " + job.TargetInventory.name);
            }
        }
    }

    private void AssignJobs()
    {
        List<Villager> idleLogisticVillagers = jobsManager.GetIdleLogisticVillagers();
        for (int i = idleLogisticVillagers.Count - 1; i >= 0; i-- )
        {
            if (!TryAssignJob(idleLogisticVillagers[i])) break;
        }
    }
}
