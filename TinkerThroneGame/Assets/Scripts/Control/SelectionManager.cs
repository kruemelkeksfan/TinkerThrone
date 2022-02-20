using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BuildingInfoDisplayer buildingInfo;
    [SerializeField] private LayerMask hittableLayers;

    private Building selectedBuilding = null;
    private ConstructionPlacementManager constructionPlacementManager;

    private void Start()
    {
        constructionPlacementManager = ConstructionPlacementManager.GetInstance();
        StartCoroutine(UpdateSelectionUI());
    }

    void Update()
    {
        if (!constructionPlacementManager.isBuilding && Input.GetButtonDown("Fire1") && !EventSystem.current.IsPointerOverGameObject())
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new(Vector3.down, new Vector3(0, 2.27f, 0));
            if (plane.Raycast(ray, out float ent))
            {
                //Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
                if (Physics.Raycast(mainCamera.transform.position, ray.GetPoint(ent) - mainCamera.transform.position, out RaycastHit hit, ent, hittableLayers.value))
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

    private IEnumerator UpdateSelectionUI()
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