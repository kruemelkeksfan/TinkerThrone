using System.Collections.Generic;
using UnityEngine;

public class LogisticsUser : InventoryUser
{
    protected Dictionary<string, LogisticValue> logisticValues;

    [SerializeField] protected LogisticValue[] specialLogisticValues;
    [SerializeField] private int defaultPriorityAtTargetAmount;
    [SerializeField] private int defaultPriorityFillAlteration;
    [SerializeField] private uint defaultTargetAmount;
    [SerializeField] private bool defaultIsTargetAmountMax;


    public virtual Vector3 GetLogisticPosition()
    {
        return transform.position;
    }
    public List<LogisticCommission> UpdateLogisticCommissions()
    {
        Dictionary<string, uint> storedGoods = inventory.GetPlanedStoredGoods();

        List<LogisticCommission> logisticCommissions = new();
        Capacity freeCapacity = inventory.GetFreeCapacity();

        foreach (LogisticValue logisticValue in logisticValues.Values)
        {
            uint currentAmount = storedGoods[logisticValue.goodName];

            uint freeAmount = freeCapacity.ToAmount(logisticValue.goodName);

            if (!logisticValue.targetAmountIsMax)
            {
                 logisticCommissions.Add(new LogisticCommission(this, logisticValue.goodName, freeAmount, currentAmount, logisticValue.GetPriority(currentAmount, inventoryCapacity)));
            }
            else
            {
                if (currentAmount > logisticValue.targetAmount)
                {
                    logisticCommissions.Add(new LogisticCommission(this, logisticValue.goodName, 0, currentAmount, logisticValue.GetPriority(currentAmount, inventoryCapacity)));
                }
                else
                {
                    logisticCommissions.Add(new LogisticCommission(this, logisticValue.goodName, logisticValue.targetAmount - currentAmount, currentAmount, logisticValue.GetPriority(currentAmount, inventoryCapacity)));
                }
            }
        }
        return logisticCommissions;
    }

    protected void SetLogisticsValues()
    {
        if (inventory == null)
        {
            InitializeInventory();
        }

        //TODO switch to only specialized for produktion/construction or default values for storage
        Dictionary<string, LogisticValue> logisticsValues = new();
        if (specialLogisticValues.Length <= 0)
        {
            foreach (Good good in GoodManager.GetInstance().GetGoodDictionary().Values)
            {
                logisticsValues.Add(good.goodName, new LogisticValue(good.goodName, defaultPriorityAtTargetAmount, defaultPriorityFillAlteration, defaultTargetAmount, defaultIsTargetAmountMax));
            }
        }
        else
        {
            foreach (LogisticValue logisticsValue in specialLogisticValues)
            {
                logisticsValues[logisticsValue.goodName] = logisticsValue;
            }
        }
        this.logisticValues = logisticsValues;
        return;
    }

    public List<StackDisplay> GetRelevantStacks()
    {
        if (inventory == null) return null;
        List<StackDisplay> relevantStacks = new();
        Dictionary<string, uint> storedGoods = inventory.GetStoredGoods();
        Dictionary<string, uint> reservedGoods = inventory.GetReservedGoods();
        Dictionary<string, uint> reservedCapacities = inventory.GetReservedCapacities();
        foreach (LogisticValue relevantLogisticValue in logisticValues.Values)
        {
            string goodName = relevantLogisticValue.goodName;
            relevantStacks.Add(new StackDisplay(goodName, storedGoods[goodName], (int)reservedCapacities[goodName] - (int)reservedGoods[goodName], relevantLogisticValue.targetAmount));
        }
        return relevantStacks;
    }
}