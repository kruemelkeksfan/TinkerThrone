using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUser : MonoBehaviour
{
    
    [SerializeField] LogisticValue[] specialLogisticValues;
    [SerializeField] int defaultPriorityBeeingEmpty;
    [SerializeField] int defaultPriorityBeeingFull;
    [SerializeField] int defaultTargetAmount;

    public LogisticCommision[] logisticInCommisions;
    public LogisticCommision[] logisticOutCommisions;
    protected Inventory inventory;
    protected Dictionary<string, LogisticValue> logisticsValues;


    protected void SetLogisticsValues()
    {
        if (inventory == null) return;

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
        this.logisticsValues = logisticsValues;
        return;
    }

    protected void UpdateLogisticCommissions()
    {
        Dictionary<string,uint> storedGoods = inventory.GetStoredGoods();

        List<LogisticCommision> inCommisions = new List<LogisticCommision>();
        List<LogisticCommision> outCommisions = new List<LogisticCommision>();

        foreach(LogisticValue logisticValue in logisticsValues.Values)
        {
            uint amount = storedGoods[logisticValue.goodName];

        }


    }

}
