using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticsManager : MonoBehaviour
{
    private static LogisticsManager instance = null;

    public List<LogisticsUser> logisticUsers = new();
    public List<LogisticJob> availableJobs = new();
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
        //TODO avoid problem where the first job has to be compoletly assigned bevor the next can be assigned

        int index = availableJobs.Count - 1;
        while (index >= 0)
        {
            if (!availableJobs[index].TryGetJobPart(villager, out LogisticJob logisticJob, out bool completedAssignment))
            {
                index--;
                continue;
            }
            //Debug.Log("new LogisticJob: " + logisticJob.SourceInventory.gameObject.name + " " + logisticJob.TargetInventory.gameObject + " " + logisticJob.Stack.goodName );
            villager.StartCoroutine(villager.DoLogisticJob(logisticJob));
            if (completedAssignment)
            {
                availableJobs.RemoveAt(availableJobs.Count - 1);
            }
            jobsManager.LogisticVillagerIdleToBusy(villager, logisticJob);
            return true;
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
        availableJobs = new List<LogisticJob>();

        foreach (LogisticsUser logisticUser in logisticUsers)
        {
            commissions.AddRange(logisticUser.UpdateLogisticCommissions());
        }

        if (commissions.Count <= 1) return;
        commissions.Sort();

        LogisticCommission usedCommission = new();
        for(int i = commissions.Count - 1; i >= 0; i--)
        {
            if (commissions[i].TakeableAmount == 0) continue;
            bool foundCommission = false;
            foreach(LogisticCommission inCommission in commissions)
            {
                if (inCommission.priority >= commissions[i].priority || (foundCommission && inCommission.priority > usedCommission.priority))
                {
                    break;
                }
                if (inCommission.goodName == commissions[i].goodName && inCommission.StoreableAmount > 0)
                {
                    if (!foundCommission)
                    {
                        foundCommission = true;
                        usedCommission = inCommission;
                    }
                    else if ((inCommission.priority <= usedCommission.priority && inCommission.StoreableAmount > usedCommission.StoreableAmount))
                    {
                        usedCommission = inCommission;
                    }
                    else
                    {
                        continue;
                    }

                    if (usedCommission.StoreableAmount >= commissions[i].TakeableAmount)
                    {
                        break;
                    }
                }
            }
            if (foundCommission) 
            { 
                int index = commissions.IndexOf(usedCommission);
                uint amount;
                if (usedCommission.StoreableAmount >= commissions[i].TakeableAmount)
                {
                    amount = commissions[i].TakeableAmount;
                }
                else
                {
                    amount = commissions[index].StoreableAmount;
                }
                commissions[index].ReduceStoreableAmount(commissions[i].TakeableAmount);
                commissions[i].ReduceTakeableAmount(commissions[i].TakeableAmount);
                LogisticJob logisticJob = new(usedCommission, commissions[i], amount);
                availableJobs.Add(logisticJob);
            }
        }
        availableJobs.Sort();
    }

    private void AssignJobs()
    {
        foreach (Villager villager in jobsManager.GetIdleLogisticVillagers()) //TODO remove idle villagers
        {
            if (!TryAssignJob(villager)) break;
        }
    }
}
