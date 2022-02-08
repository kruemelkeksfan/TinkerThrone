using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticsManager : MonoBehaviour
{
    public static LogisticsManager instance = null;

    public List<InventoryUser> inventoryUsers = new List<InventoryUser>();
    List<LogisticJob> activeJobs = new List<LogisticJob>();
    List<LogisticJob> availableJobs = new List<LogisticJob>();

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

        foreach (InventoryUser inventoryUser in inventoryUsers)
        {
            List<LogisticCommission>[] inventoryLogisticCommissions = inventoryUser.UpdateLogisticCommissions();
            inCommissions.AddRange(inventoryLogisticCommissions[0]);
            outCommissions.AddRange(inventoryLogisticCommissions[1]);
        }
        if (inCommissions.Count == 0 || outCommissions.Count == 0) return;
        inCommissions.Sort();
        outCommissions.Sort();
        /*foreach (LogisticCommission logisticCommission in outCommissions)
        {
            Debug.Log(logisticCommission.priority);
        }*/
    }
}
