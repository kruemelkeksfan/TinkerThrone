using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Building : MonoBehaviour
{
	[Tooltip("The Name of this Building.")]
	public string buildingName = "Unnamed Building";
	[Tooltip("Available Production Recipes for this Building.")]
	public  Recipe[] recipes = { };									// Actual Production should not be handled by the Building, but by the Worker Pawn
	[Tooltip("The 3D-Model for this Building.")]
	public  GameObject buildingModel = null;
	[SerializeField] Stack[] neededMaterials;
    ConstructionSpace constructionSpace;
    UpgradeSpace upgradeSpace;

    private void Awake()
    {
        constructionSpace = GetComponentInChildren<ConstructionSpace>();
        upgradeSpace = GetComponentInChildren<UpgradeSpace>();
    }
    public ConstructionSpace GetConstructionSpace()
    {
        return constructionSpace;
    }
    public UpgradeSpace GetUpgradeSpace()
    {
        return upgradeSpace;
    }
}
