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

    private void UpdateLogisticsJobs()
    {
        List<LogisticCommission> inCommissions = new();
        List<LogisticCommission> outCommissions = new();
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

        LogisticCommission usedCommission = new();
        foreach (LogisticCommission outCommission in outCommissions)
        {
            bool foundCommission = false;
            foreach (LogisticCommission inCommission in inCommissions)
            {
                if (inCommission.goodName == outCommission.goodName)
                {
                    if (!foundCommission)
                    {
                        foundCommission = true;
                        usedCommission = inCommission;
                    }
                    else if (inCommission.Amount > usedCommission.Amount)
                    {
                        usedCommission = inCommission;
                    }
                    else
                    {
                        continue;
                    }

                    if (usedCommission.Amount >= outCommission.Amount)
                    {
                        break;
                    }
                }
            }
            if (foundCommission)
            {
                LogisticJob logisticJob = new LogisticJob(usedCommission, outCommission);
                if (logisticJob.Priority < 0)
                {
                    continue;
                }
                availableJobs.Add(logisticJob);
                if (usedCommission.Amount > outCommission.Amount)
                {
                    int index = inCommissions.IndexOf(usedCommission);
                    inCommissions[index].ReduceAmount(outCommission.Amount);
                }
                else
                {
                    inCommissions.Remove(usedCommission);
                }
            }
        }
        availableJobs.Sort();
    }

    private void AssignJobs()
    {
        foreach (Villager villager in jobsManager.GetIdleLogisticVillagers())
        {
            if (!TryAssignJob(villager)) break;
        }
    }
}
