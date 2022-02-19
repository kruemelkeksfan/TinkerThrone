using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    ConstructionPlacementManager constructionPlacementManager;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask hittableLayers;

    [SerializeField] BuildingInfoDisplayer buildingInfo;
    private Building selectedBuilding = null;


    private void Start()
    {
        constructionPlacementManager = ConstructionPlacementManager.GetInstance();
        StartCoroutine(UpdateSelectionUI());
    }

    // Update is called once per frame
    void Update()
    {
        if (!constructionPlacementManager.isBuilding && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.down, new Vector3(0, 2.27f, 0));
            float ent = 100.0f;
            if (plane.Raycast(ray, out ent))
            {
                //Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
                RaycastHit hit;
                if (Physics.Raycast(mainCamera.transform.position, ray.GetPoint(ent) - mainCamera.transform.position, out hit, ent, hittableLayers.value))
                {
                    if (hit.collider.gameObject.layer == 7)
                    {
                        selectedBuilding = hit.collider.gameObject.GetComponent<Building>();
                        if (selectedBuilding)
                        {
                            buildingInfo.gameObject.SetActive(true);
                            buildingInfo.DisplayInformation(selectedBuilding);
                        }
                        else
                        {
                            buildingInfo.gameObject.SetActive(false);
                        }
                    }

                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * ent, Color.red);
                return;
            }
        }
        else if (selectedBuilding && !constructionPlacementManager.isBuilding && Input.GetButtonDown("Fire2") && !EventSystem.current.IsPointerOverGameObject())
        {
            selectedBuilding = null;
            buildingInfo.gameObject.SetActive(false);
        }
    }

    IEnumerator UpdateSelectionUI()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            if (selectedBuilding)
            {
                buildingInfo.DisplayInformation(selectedBuilding);
            }
        }
    }
}
