using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingInfoDisplayer : MonoBehaviour
{
    [Header("Linked UI-Elements")]
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI buildingTypeText;
    [SerializeField] private FourColorFillTextBar capacityUnitDisplay;
    [SerializeField] private FourColorFillTextBar capacityMassDisplay;
    [SerializeField] private FourColorFillTextBar capacityVolumeDisplay;
    [Header("Prefabs")]
    [SerializeField] private GoodDisplayer goodDisplayerPrefab;

    readonly List<GoodDisplayer> goodDisplayers = new();

    public void DisplayInformation(Building building)
    {
        if (goodDisplayers.Count > 0)
        {
            for (int i = goodDisplayers.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(goodDisplayers[i].gameObject);
                goodDisplayers.RemoveAt(i);
            }
        }
        buildingNameText.text = building.buildingName;
        buildingTypeText.text = building.GetBuildingType();
        List<StackDisplay> relevantStacks;
        bool underConstruction = building.IsUnderConstruction(out ConstructionSite constructionSite);
        if(underConstruction)
        {
            relevantStacks = constructionSite.GetRelevantStacks();
        }
        else
        {
            relevantStacks = building.GetRelevantStacks();
        }
        if (relevantStacks == null) return;

        int count = 0;
        foreach (StackDisplay stack in relevantStacks)
        {
            GoodDisplayer newGoodDisplayer = Instantiate(goodDisplayerPrefab, this.transform);
            newGoodDisplayer.DisplayInformation(stack);
            RectTransform rectTransform = newGoodDisplayer.GetComponent<RectTransform>();
            if (count % 2 == 1)
            {
                rectTransform.position += new Vector3(rectTransform.sizeDelta.x, (count - 1) * 0.5f * -rectTransform.sizeDelta.y, 0);
            }
            else
            {
                rectTransform.position += new Vector3(0, count * 0.5f * -rectTransform.sizeDelta.y, 0);
            }
            count++;
            goodDisplayers.Add(newGoodDisplayer);
        }

        Capacity capacity;
        Inventory buildingInventory;
        if (underConstruction)
        {
            capacity = constructionSite.GetCapacity();
            buildingInventory = constructionSite.GetInventory();
        }
        else
        {
            capacity = building.GetCapacity();
            buildingInventory = building.GetInventory();
        }
        Capacity freeCapacity = buildingInventory.GetFreeCapacity();
        Capacity reservedCapacity = buildingInventory.GetReservedCapacity();
        Capacity tempOccupiedCapacity = buildingInventory.GetTemporarilyOccupiedCapacity();

        Capacity reserveFilledCapacity = capacity - freeCapacity;//capacity after only active depositing logistic changes
        Capacity tempFilledCapacity = capacity - (freeCapacity + reservedCapacity);//capacity currently in the inventory
        Capacity filledCapacity = capacity - (freeCapacity + reservedCapacity + tempOccupiedCapacity);//capacity after only active withdrawing logistic changes
        Capacity reserveCapacities = reservedCapacity - tempOccupiedCapacity;

        if (capacity.unitCapacity > 0)
        {
            capacityUnitDisplay.gameObject.SetActive(true);
            string unitText;
            if (reserveCapacities.unitCapacity > 0)
            {
                unitText = filledCapacity.unitCapacity + " (+" + reserveCapacities.unitCapacity + ") / " + capacity.unitCapacity;
            }
            else
            {
                unitText = filledCapacity.unitCapacity + " (" + reserveCapacities.unitCapacity + ") / " + capacity.unitCapacity;
            }
            capacityUnitDisplay.UpdateBar(
            (float)reserveFilledCapacity.unitCapacity / capacity.unitCapacity,
            (float)tempFilledCapacity.unitCapacity / capacity.unitCapacity,
            (float)filledCapacity.unitCapacity / capacity.unitCapacity,
            unitText);
        }
        else
        {
            capacityUnitDisplay.gameObject.SetActive(false);
        }

        if (capacity.massCapacity > 0)
        {
            capacityMassDisplay.gameObject.SetActive(true);
            string unitText;
            if (reserveCapacities.massCapacity > 0)
            {
                unitText = Math.Round(filledCapacity.massCapacity, 1) + " (+" + Math.Round(reserveCapacities.massCapacity, 1) + ") / " + capacity.massCapacity;
            }
            else
            {
                unitText = Math.Round(filledCapacity.massCapacity, 1) + " (" + Math.Round(reserveCapacities.massCapacity, 1) + ") / " + capacity.massCapacity;
            }
            capacityMassDisplay.UpdateBar(
            reserveFilledCapacity.massCapacity / capacity.massCapacity,
            tempFilledCapacity.massCapacity / capacity.massCapacity,
            filledCapacity.massCapacity / capacity.massCapacity,
            unitText);
        }
        else
        {
            capacityMassDisplay.gameObject.SetActive(false);
        }

        if (capacity.volumeCapacity > 0)
        {
            capacityVolumeDisplay.gameObject.SetActive(true);
            string unitText;
            if (reserveCapacities.volumeCapacity > 0)
            {
                unitText = Math.Round(filledCapacity.volumeCapacity, 1) + " (+" + Math.Round(reserveCapacities.volumeCapacity, 1) + ") / " + capacity.volumeCapacity;
            }
            else
            {
                unitText = Math.Round(filledCapacity.volumeCapacity, 1) + " (" + Math.Round(reserveCapacities.volumeCapacity, 1) + ") / " + capacity.volumeCapacity;
            }
            capacityVolumeDisplay.UpdateBar(
            reserveFilledCapacity.volumeCapacity / capacity.volumeCapacity,
            tempFilledCapacity.volumeCapacity / capacity.volumeCapacity,
            filledCapacity.volumeCapacity / capacity.volumeCapacity,
            unitText);
        }
        else
        {
            capacityVolumeDisplay.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (goodDisplayers.Count > 0)
        {
            for (int i = goodDisplayers.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(goodDisplayers[i].gameObject);
                goodDisplayers.RemoveAt(i);
            }
        }
    }
}
