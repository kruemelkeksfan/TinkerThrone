using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticsManager : MonoBehaviour
{
    public static LogisticsManager instance = null;

    Dictionary<Inventory, Dictionary<string, LogisticValue>> inventoryDictionary = new Dictionary<Inventory, Dictionary<string, LogisticValue>>();
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

    //TODO Replace with order system
    public void AddInventory(Inventory inventory, Dictionary<string, LogisticValue> logisticValues)
    {
        if (inventoryDictionary.ContainsKey(inventory))
        {
            inventoryDictionary[inventory] = logisticValues;
        }
        else
        {
            inventoryDictionary.Add(inventory, logisticValues);
        }

    }

    public void RemoveInventory(Inventory inventory)
    {
        //TODO check for running jobs
        if (inventoryDictionary.ContainsKey(inventory))
        {
            inventoryDictionary.Remove(inventory);
        }
    }

    IEnumerator UpdateCycle()
    {
        yield return new WaitForSeconds(0.5f);
        UpdateLogisticsJobs();
    }

    private void UpdateLogisticsJobs()
    {
        
    }
}
