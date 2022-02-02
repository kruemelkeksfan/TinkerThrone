using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpace : BuildingSpace
{
    private void Update()
    {
        int colorEscelation = 0;
        for (int i = colliders.Count - 1; i >= 0; i--)
        {
            if(colliders[i] == null)
            {
                colliders.Remove(colliders[i]);
                continue;
            }
            if (colorEscelation < 2 && colliders[i].gameObject.GetComponent<ConstructionSpace>())
            {
                colorEscelation = 2;
                break;
            }
            else if (colorEscelation < 1)
            {
                colorEscelation = 1;
            }
        }
        meshRenderer.material.SetColor("_EmissionColor", WorldConsts.BUILDING_SPACE_COLORS[colorEscelation]);
    }
}
