using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSpace : BuildingSpace
{
    public bool isBlocked { get; private set; }
    
    private void Update()
    {
        int colorEscelation = 0;
        isBlocked = false;
        for (int i = colliders.Count - 1; i >= 0; i--)
        {
            if (colliders[i] == null)
            {
                colliders.Remove(colliders[i]);
                continue;
            }
            UpgradeSpace upgradeSpace = colliders[i].gameObject.GetComponent<UpgradeSpace>();
            if (colorEscelation < 2 && upgradeSpace)
            {
                colorEscelation = 2;
            }
            else if (colorEscelation < 3 && !upgradeSpace)
            {
                colorEscelation = 3;
                isBlocked = true;
                break;
            }
        }
        meshRenderer.material.SetColor("_EmissionColor", WorldConsts.BUILDING_SPACE_COLORS[colorEscelation]);
    }
}
