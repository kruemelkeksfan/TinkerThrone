using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiNavigation : MonoBehaviour
{
    [SerializeField] GameObject currentPanel;
    [SerializeField] GameObject buildingUI;
    [SerializeField] GameObject buildingMainPanel;
    [SerializeField] GameObject mainPanel;
    ConstructionPlacementManager placementManager;

    private void Start()
    {
        placementManager = ConstructionPlacementManager.GetInstance();
    }

    void Update()
    {
        if (Input.GetButtonUp("Building Menu"))
        {
            ToggleBuildingUI();
        }
    }

        public void MoveToPanel(GameObject nextPanel)
    {
        nextPanel.SetActive(true);
        currentPanel.SetActive(false);
        currentPanel = nextPanel;
    }

    public void ToggleBuildingUI()
    {
        if (buildingUI.activeSelf)
        {
            buildingUI.SetActive(false);
            mainPanel.SetActive(true);
            currentPanel.SetActive(false);
            currentPanel = mainPanel;
            placementManager.ToggleBuildingMode();
        }
        else
        {
            buildingUI.SetActive(true);
            currentPanel.SetActive(false);
            currentPanel = buildingMainPanel;
            currentPanel.SetActive(true);
            placementManager.ToggleBuildingMode();
        }
    }
}
