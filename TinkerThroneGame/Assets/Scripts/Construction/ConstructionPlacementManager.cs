using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionPlacementManager : MonoBehaviour
{
    [SerializeField] GameObject buildingUi;
    [SerializeField] GameObject preBuildingModule;
    [SerializeField] MeshRenderer buildableDisplay;
    ConstructionInfo prefab;
    [SerializeField] Transform building;
    public bool isBuilding = false;
    Vector3 modulePos = Vector3.zero;

    void Update()
    {
        if (Input.GetButtonUp("Building Menu"))
        {
            ToggleBuildingMode();
        }
        if (Input.GetButtonUp("Fire2") && !EventSystem.current.IsPointerOverGameObject())
        {
            SelectBuildingType(null);
        }
        if (isBuilding && prefab != null)
        {
            
            Vector3 hitPoint = Vector3.zero;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.down, new Vector3 (0, 2.27f,0));
            float ent = 100.0f;
            if (plane.Raycast(ray, out ent))
            {
                hitPoint = ray.GetPoint(ent);

                Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * ent, Color.red);
                return;
            }

            //Vector3 gridPos = hitPoint - building.transform.position;

            Vector3 gridPos = new Vector3(Mathf.RoundToInt(hitPoint.x * WorldConsts.GRID_SIZE_RECIPROCAL) * WorldConsts.GRID_SIZE, WorldConsts.BUILDING_HIGHT, Mathf.RoundToInt(hitPoint.z * WorldConsts.GRID_SIZE_RECIPROCAL)* WorldConsts.GRID_SIZE);

            Vector3 newModulePos = gridPos;

            if (newModulePos == modulePos)
            {
                if (Input.GetButtonUp("Fire1") && !EventSystem.current.IsPointerOverGameObject())
                {
                    PlaceBuilding();
                }
                return;
            }

            modulePos = newModulePos;

            if (IsFreePosition(modulePos))
            {
                //Debug.Log("ok : " + modulePos.ToString());
                buildableDisplay.material.SetColor("_EmissionColor", Color.green);
            }
            else
            {
                //Debug.Log("nok : " + modulePos.ToString());
                buildableDisplay.material.SetColor("_EmissionColor", Color.red);
            }

            building.transform.position = gridPos;
            building.transform.rotation = Quaternion.identity;

            if (Input.GetButtonUp("Fire1") && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceBuilding();
            }
        }
        
    }

    private bool IsFreePosition(Vector3 modulePos)
    {
        return true;
        //add pos check
    }

    public void ToggleBuildingMode(bool destroy = true)
    {
        isBuilding = !isBuilding;
        buildingUi.SetActive(isBuilding);

        if (!isBuilding)
        {
            SelectBuildingType(null);
        }
    }

    public void SelectBuildingType(ConstructionInfo constructionInfo)
    {
        prefab = constructionInfo;
        if(building != null) 
        {
            GameObject.Destroy(building.gameObject);
        }
        if (prefab == null)
        {
            preBuildingModule.transform.parent = this.transform;
            preBuildingModule.SetActive(false);
            return;
        }
        building = Instantiate<ConstructionInfo>(prefab).GetComponent<Transform>();
        preBuildingModule.SetActive(true);
        preBuildingModule.transform.parent = building.transform;
        preBuildingModule.transform.localPosition = Vector3.zero;
        preBuildingModule.transform.localRotation = Quaternion.identity;
    }

    public void PlaceBuilding()
    {
        building = null;
        SelectBuildingType(null); //releases Building
    }

    /*public void Rotate(int direction)
    {
        if (!moduleSelected) return;
        meshType.Rotate(direction);
        preBuildingModule.ChangeModule(meshType);
    }*/
}