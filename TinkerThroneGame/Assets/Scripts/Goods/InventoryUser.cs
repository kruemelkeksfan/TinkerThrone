using UnityEngine;

public class InventoryUser : MonoBehaviour
{
    [SerializeField] protected Stack[] testStacks = new Stack[0];
    [SerializeField] protected bool hasInventory = false;
    [SerializeField] protected Capacity inventoryCapacity;

    protected Inventory inventory;

    public Capacity GetCapacity()
    {
        return inventoryCapacity;
    }


    public Inventory GetInventory()
    {
        return inventory;
    }

    protected void InitializeInventory()
    {
        inventory = new Inventory(inventoryCapacity);
        foreach (Stack stack in testStacks)
        {
            inventory.ReserveDeposit(stack);
            inventory.Deposit(stack);
        }
    }
}