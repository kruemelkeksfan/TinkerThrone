using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Building : LogisticsUser
{
	[Tooltip("The Name of this Building.")]
	public string buildingName = "Unnamed Building";
	[Tooltip("Available Production Recipes for this Building.")]
	public  Recipe[] recipes = { };									// Actual Production should not be handled by the Building, but by the Worker Pawn
	[Tooltip("The 3D-Model for this Building.")]
	public  GameObject buildingModel = null;
	[SerializeField] Stack[] neededMaterials;
    [SerializeField] ConstructionSpace constructionSpace;
    [SerializeField] UpgradeSpace upgradeSpace;

    [SerializeField] bool active = false;


    private void Start()
    {
        
        if (active)
        {
            ActivateBuilding();
            return;
        }
        constructionSpace = GetComponentInChildren<ConstructionSpace>();
        upgradeSpace = GetComponentInChildren<UpgradeSpace>();
    }
    public ConstructionSpace GetConstructionSpace()
    {
        if (!constructionSpace)
        {
            constructionSpace = GetComponentInChildren<ConstructionSpace>();
        }
        return constructionSpace;
    }
    public UpgradeSpace GetUpgradeSpace()
    {
        return upgradeSpace;
    }

    public List<StackDisplay> GetRelevantStacks()
    {
        if (inventory == null) return null;
        List<StackDisplay> relevantStacks = new List<StackDisplay>();
        Dictionary<string,uint> storedGoods = inventory.GetStoredGoods();
        Dictionary<string, uint> reservedGoods = inventory.GetReservedGoods();
        Dictionary<string, uint> reservedCapacities = inventory.GetReservedCapacities();
        foreach (LogisticValue relevantLogisticValue in logisticValues.Values)
        {
            string goodName = relevantLogisticValue.goodName;
            relevantStacks.Add(new StackDisplay(goodName, storedGoods[goodName], (int)reservedCapacities[goodName] - (int)reservedGoods[goodName]));
        }
        return relevantStacks;
    }

    public void ActivateBuilding()
    {
        active = true;
        if (hasInventory)
        {
            SetLogisticsValues();
            LogisticsManager.GetInstance().AddInventory(this);
        }
    }
   
}
