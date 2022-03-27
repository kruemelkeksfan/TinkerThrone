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
	public  Recipe[] recipes = { };                                 // Actual Production should not be handled by the Building, but by the Worker Pawn
    [Header("Models")]
    [SerializeField] private GameObject currentModel = null;
    [SerializeField] private GameObject constructionModelPrefab = null;
    [SerializeField] private GameObject finalModelPrefab = null;
    [Header("Construction Options")]
    [SerializeField] private ConstructionSpace constructionSpace; //DEBUG serializeField
    [SerializeField] private UpgradeSpace upgradeSpace; //DEBUG serializeField
    [SerializeField] private Transform inventoryLocation;
    [SerializeField] private Stack[] neededMaterials;
    [SerializeField] private LogisticValue[] specialConstructionLogisticValues;

    private ConstructionSite constructionSite;
    private LogisticValue[] preDeconstructionLogisticValues;

    [SerializeField] private bool active = false; //DEBUG serializeField
    bool beeingDeconstructed = false;

    public bool IsDeconstructing()
    {
        return beeingDeconstructed;
    }

    public void SetCurrentModel(GameObject model)
    {
        currentModel = model;
    }

    public string GetBuildingType()
    {
        return currentModel.name;
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

    public bool IsUnderConstruction(out ConstructionSite constructionSite)
    {
        constructionSite = this.constructionSite;
        return constructionSite != null;
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

    private void OnDestroy()
    {
        BuildingSpaceHolder.GetInstance().RemoveBuildingSpaces(new BuildingSpace[] { constructionSpace, upgradeSpace });
    }

    public void StartConstruction()
    {
        constructionSite = gameObject.AddComponent<ConstructionSite>();
        constructionSite.StartConstruction(currentModel, finalModelPrefab, inventoryLocation);
    }

    public void StartDeconstruction()
    {
        StartCoroutine(PrepareDeconstruction());
    }

    public void CancleDeconstruction()
    {
        if (constructionSite == null)
        {
            StopCoroutine(PrepareDeconstruction());
            specialLogisticValues = preDeconstructionLogisticValues;
            ActivateBuilding();
        }
        else
        {
            constructionSite.StartConstruction(currentModel, finalModelPrefab, inventoryLocation, true);
        }
        beeingDeconstructed = false;
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

    private IEnumerator PrepareDeconstruction()
    {
        if (hasInventory && active)
        {
            preDeconstructionLogisticValues = new List<LogisticValue>(logisticValues.Values).ToArray(); //TODO maybe rework this
            Dictionary<string, LogisticValue> newLogisticValues = new();
            foreach(string good in logisticValues.Keys)
            {
                newLogisticValues.Add(good, new LogisticValue(good, 10, 10, 0));
            }
            logisticValues = newLogisticValues;
            LogisticsManager.GetInstance().UpdateLogisticsJobs();
            active = false;
            yield return new WaitUntil(() => inventory.IsEmpty() == true);
            LogisticsManager.GetInstance().RemoveInventory(this);
        }
        active = false;
        if (constructionSite == null)
        {
            constructionSite = gameObject.AddComponent<ConstructionSite>();
            beeingDeconstructed = true;
            constructionSite.StartDeconstruction(constructionModelPrefab, currentModel, finalModelPrefab, inventoryLocation);
        }
        else
        {
            beeingDeconstructed = true;
            constructionSite.StartDeconstruction(currentModel, currentModel, finalModelPrefab, inventoryLocation, true);
        }
    }
}