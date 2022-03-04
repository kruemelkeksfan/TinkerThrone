using System.Collections.Generic;
using UnityEngine;

public class BuildingSpaceHolder : MonoBehaviour
{
    private static BuildingSpaceHolder instance;

    private readonly List<BuildingSpace> buildingSpaceList = new();
    private bool isActive = false;

    public static BuildingSpaceHolder GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        buildingSpaceList.AddRange(gameObject.GetComponentsInChildren<BuildingSpace>());
        foreach(BuildingSpace buildingSpace in buildingSpaceList) 
        {
            buildingSpace.gameObject.SetActive(isActive);
        }
    }

    public void AddBuildingSpaces(BuildingSpace[] buildingSpaces)
    {
        foreach (BuildingSpace buildingSpace in buildingSpaces)
        {
            buildingSpaceList.Add(buildingSpace);
            buildingSpace.transform.parent = transform;
            buildingSpace.gameObject.SetActive(isActive);
        }
    }

    public void RemoveBuildingSpaces(BuildingSpace[] buildingSpaces)
    {
        for (int i = buildingSpaces.Length - 1; i >= 0; i--)
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