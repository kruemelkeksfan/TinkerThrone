using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticsManager : MonoBehaviour
{
    static LogisticsManager instance = null;

    public List<InventoryUser> inventoryUsers = new List<InventoryUser>();
    public List<LogisticJob> availableJobs = new List<LogisticJob>();

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
        StartCoroutine(UpdateCycle()); 
    }

    public void AddInventory(InventoryUser inventory)
    {
        if (!inventoryUsers.Contains(inventory))
        {
            inventoryUsers.Add(inventory);
        }
    }

    public void RemoveInventory(InventoryUser inventory)
    {
        //TODO check for running jobs
        if (inventoryUsers.Contains(inventory))
        {
            inventoryUsers.Remove(inventory);
        }
    }

    IEnumerator UpdateCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            UpdateLogisticsJobs();
        }
    }

    private void UpdateLogisticsJobs()
    {
        List<LogisticCommission> inCommissions = new List<LogisticCommission>();
        List<LogisticCommission> outCommissions = new List<LogisticCommission>();
        availableJobs = new List<LogisticJob>();

        foreach (InventoryUser inventoryUser in inventoryUsers)
        {
            List<LogisticCommission>[] inventoryLogisticCommissions = inventoryUser.UpdateLogisticCommissions();
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
    }
}
