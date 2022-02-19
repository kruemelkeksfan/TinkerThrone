using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiNavigation : MonoBehaviour
{
    [SerializeField] GameObject currentPanel;
    [SerializeField] GameObject buildingUI;
    [SerializeField] GameObject buildingMainPanel;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject jobPanel;
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
        if (Input.GetButtonUp("Job Menu"))
        {
            ToggleJobMenu();
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

    public void ToggleJobMenu()
    {
        if (jobPanel.activeSelf)
        {
            MoveToPanel(mainPanel);
        }
        else
        {
            if (buildingUI.activeSelf)
            {
                placementManager.ToggleBuildingMode();
                buildingUI.SetActive(false);
            }
            MoveToPanel(jobPanel);
        }
    }
}
