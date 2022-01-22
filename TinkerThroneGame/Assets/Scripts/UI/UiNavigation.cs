using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiNavigation : MonoBehaviour
{
    [SerializeField] GameObject currentPanel;
    public void MoveToPanel(GameObject nextPanel)
    {
        nextPanel.SetActive(true);
        currentPanel.SetActive(false);
        currentPanel = nextPanel;
    }
}
