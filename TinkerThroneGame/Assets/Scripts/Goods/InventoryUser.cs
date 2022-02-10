using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUser : MonoBehaviour
{
    [SerializeField] protected Stack[] testStacks;

    protected bool hasInventory = false;
    protected Inventory inventory;
    [SerializeField] protected Capacity inventoryCapacity;

    public Inventory GetInventory()
    {
        return inventory;
    }
}
