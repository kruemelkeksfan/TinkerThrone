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
    public GameObject currentModel = null;
    public GameObject constructionModelPrefab = null;
    public GameObject finalModelPrefab = null;
    [SerializeField] Stack[] neededMaterials;
    [SerializeField] ConstructionSpace constructionSpace;
    [SerializeField] UpgradeSpace upgradeSpace;
    [SerializeField] private LogisticValue[] specialConstructionLogisticValues;
    [SerializeField] Transform inventoryLocation;
    ConstructionSite constructionSite;

    [SerializeField] bool active = false;

    public void SetCurrentModdel(GameObject model)
    {
        currentModel = model;
    }

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

    public void StartConstruction()
    {
        constructionSite = gameObject.AddComponent<ConstructionSite>();
        constructionSite.StartConstruction(currentModel, finalModelPrefab, inventoryLocation, specialConstructionLogisticValues);
    }

    public bool UnderConstruction(out ConstructionSite constructionSite)
    {
        constructionSite = this.constructionSite;
        return constructionSite != null;
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
