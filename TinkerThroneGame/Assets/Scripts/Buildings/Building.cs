using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Building : InventoryUser
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
            LogisticsManager.GetInstance().AddInventory(this);
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


    public void ActivateBuilding()
    {
        active = true;
        SetLogisticsValues();
    }
   
}
