using UnityEngine;

public class UiNavigation : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject buildingUI;
    [SerializeField] private GameObject buildingMainPanel;
    [SerializeField] private GameObject jobPanel;

    private ConstructionPlacementManager placementManager;
    private GameObject currentPanel;

    private void Start()
    {
        placementManager = ConstructionPlacementManager.GetInstance();
        currentPanel = mainPanel;
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