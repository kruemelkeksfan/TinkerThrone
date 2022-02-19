using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticsManager : MonoBehaviour
{
    static LogisticsManager instance = null;

    public List<LogisticsUser> logisticUsers = new List<LogisticsUser>();
    public List<LogisticJob> availableJobs = new List<LogisticJob>();

    JobsManager jobsManager;

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

    IEnumerator UpdateCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            UpdateLogisticsJobs();
            AssignJobs();
        }
    }

    private void UpdateLogisticsJobs()
    {
        List<LogisticCommission> inCommissions = new List<LogisticCommission>();
        List<LogisticCommission> outCommissions = new List<LogisticCommission>();
        availableJobs = new List<LogisticJob>();

        foreach (LogisticsUser logisticUser in logisticUsers)
        {
            List<LogisticCommission>[] inventoryLogisticCommissions = logisticUser.UpdateLogisticCommissions();
            inCommissions.AddRange(inventoryLogisticCommissions[0]);
            outCommissions.AddRange(inventoryLogisticCommissions[1]);
        }
        if (inCommissions.Count == 0 || outCommissions.Count == 0) return;
        inCommissions.Sort();
        outCommissions.Sort();
        outCommissions.Reverse();
        LogisticCommission usedCommission = new LogisticCommission();
        bool foundCommission = false;
        foreach (LogisticCommission outCommission in outCommissions)
        {
            foundCommission = false;
            foreach(LogisticCommission inCommission in inCommissions)
            {
                if (inCommission.goodName == outCommission.goodName)
                {
                    if (!foundCommission)
                    {
                        foundCommission = true;
                        usedCommission = inCommission;
                    }
                    else if(inCommission.amount > usedCommission.amount)
                    {
                        usedCommission = inCommission;
                    }
                    else
                    {
                        continue;
                    }

                    if (usedCommission.amount >= outCommission.amount)
                    {
                        break;
                    }
                }
            }
            if (foundCommission)
            {
                availableJobs.Add(new LogisticJob(usedCommission, outCommission));
                if (usedCommission.amount > outCommission.amount)
                {
                    int index = inCommissions.IndexOf(usedCommission);
                    inCommissions[index].ReduceAmount(outCommission.amount);
                }
                else
                {
                    inCommissions.Remove(usedCommission);
                }
            }
        }
        availableJobs.Sort();
    }

    void AssignJobs()
    {
        foreach (Villager villager in jobsManager.GetIdleLogisticVillagers())
        {
            if (!TryAssignJob(villager)) break;
        }
    }

    public bool TryAssignJob(Villager villager)
    {
        if (availableJobs.Count == 0) return false;
        //TODO avoid problem where the first job has to be compoletly assigned bevor the next can be assigned

        int index = availableJobs.Count - 1;
        while(index >= 0)
        {
            LogisticJob newJob = availableJobs[index].GetJobPart(villager, out bool completedAssignment);
            if (newJob.stack.amount == 0)
            {
                index--;
                continue;
            }
            villager.StartCoroutine(villager.DoLogisticJob(newJob));
            if (completedAssignment)
            {
                availableJobs.RemoveAt(availableJobs.Count - 1);
            }
            jobsManager.LogisticVillagerIdleToBusy(villager, newJob);
            return true;
        }
        return false;
    }
}
