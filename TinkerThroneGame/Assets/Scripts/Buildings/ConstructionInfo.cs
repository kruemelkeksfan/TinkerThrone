using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionInfo : MonoBehaviour
{
    [SerializeField] Vector2Int size;
    [SerializeField] Stack[] neededMaterials;
    [SerializeField] ConstructionSpace constructionSpace;
    [SerializeField] UpgradeSpace upgradeSpace;

    public ConstructionSpace GetConstructionSpace()
    {
        return constructionSpace;
    }
    public UpgradeSpace GetUpgradeSpace()
    {
        return upgradeSpace;
    }
}
