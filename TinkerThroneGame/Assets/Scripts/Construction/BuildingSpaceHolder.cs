using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpaceHolder : MonoBehaviour
{
    List<BuildingSpace> buildingSpaceList = new List<BuildingSpace>();
    bool isActive = false;

    public void AddBuildingSpaces(BuildingSpace[] buildingSpaces)
    {
        foreach (BuildingSpace buildingSpace in buildingSpaces)
        {
            buildingSpaceList.Add(buildingSpace);
            buildingSpace.transform.parent = transform;
        }
    }

    public void RemoveBuildingSpaces(BuildingSpace[] buildingSpaces)
    {
        for (int i = buildingSpaces.Length + 1; i >= 0; i--)
        {
            buildingSpaceList.Remove(buildingSpaces[i]);
            GameObject.Destroy(buildingSpaces[i]);
        }
    }

    public void ToggleBuildingSpaces()
    {
        isActive = !isActive;
        foreach (BuildingSpace buildingSpace in buildingSpaceList)
        {
            buildingSpace.gameObject.SetActive(isActive);
        }
    }
}
