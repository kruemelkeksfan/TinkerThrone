using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUser : MonoBehaviour
{
    [SerializeField] protected Stack[] testStacks;

    [SerializeField]protected bool hasInventory = false;
    protected Inventory inventory;
    [SerializeField] protected Capacity inventoryCapacity;

    protected void InitializeInventory()
    {
        inventory = new Inventory(inventoryCapacity);
        foreach (Stack stack in testStacks)
        {
            inventory.ReserveDeposit(stack);
            inventory.Deposit(stack);
        }
    }

    public Capacity GetCapacity()
    {
        return inventoryCapacity;
    }


    public Inventory GetInventory()
    {
        return inventory;
    }
}
