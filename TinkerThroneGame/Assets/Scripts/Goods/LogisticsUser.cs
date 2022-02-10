using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticsUser : InventoryUser
{
    
    [SerializeField] LogisticValue[] specialLogisticValues;
    [SerializeField] int defaultPriorityBeeingEmpty;
    [SerializeField] int defaultPriorityBeeingFull;
    [SerializeField] uint defaultTargetAmount;

    public List<LogisticCommission> logisticInCommissions;
    public List<LogisticCommission> logisticOutCommissions;
    protected Dictionary<string, LogisticValue> logisticValues;

    protected void SetLogisticsValues()
    {
        if (inventory == null)
        {
            inventory = new Inventory(inventoryCapacity);
            foreach(Stack stack in testStacks)
            {
                inventory.ReserveDeposit(stack);
                inventory.Deposit(stack);
            }
        }

        //TODO switch to only specialized for produktion/construction or default values for storage
        Dictionary<string, LogisticValue> logisticsValues = new Dictionary<string, LogisticValue>();
        if (specialLogisticValues.Length <= 0)
        {
            foreach (Good good in GoodManager.GetInstance().GetGoodDictionary().Values)
            {
                logisticsValues.Add(good.goodName, new LogisticValue(good.goodName, defaultPriorityBeeingEmpty, defaultPriorityBeeingFull, defaultTargetAmount));
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

    public List<LogisticCommission>[] UpdateLogisticCommissions()
    {
        Dictionary<string,uint> storedGoods = inventory.GetStoredGoods();

        logisticInCommissions = new List<LogisticCommission>();
        logisticOutCommissions = new List<LogisticCommission>();

        foreach(LogisticValue logisticValue in logisticValues.Values)
        {
            uint currentAmount = storedGoods[logisticValue.goodName];

            if (currentAmount == logisticValue.targetAmount) continue;
            else if(currentAmount > logisticValue.targetAmount)
            {
                logisticOutCommissions.Add(new LogisticCommission(this, logisticValue.goodName, currentAmount-logisticValue.targetAmount, logisticValue.PriorityForHigherAmount(currentAmount, inventoryCapacity)));
            }
            else
            {
                logisticInCommissions.Add(new LogisticCommission(this, logisticValue.goodName, currentAmount, logisticValue.PriorityForLowerAmount(currentAmount)));
            }
        }
        return new List<LogisticCommission>[] { logisticInCommissions, logisticOutCommissions };
    }

}
