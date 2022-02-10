using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text text = null;
    [SerializeField] private TMPro.TMP_Dropdown dropdown = null;
    [SerializeField] private TMPro.TMP_InputField input = null;
    [SerializeField] private Capacity inventoryCapacity = new Capacity(6, 12000.0f, 10.0f);
    private Inventory inventory = null;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory(inventoryCapacity);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Wood: " + inventory.GetGoodAmount("Wood") + " Lumber: " + inventory.GetGoodAmount("Lumber") + "\n";
        foreach(string goodName in inventory.GetStoredGoods().Keys)
		{
            text.text += goodName + ": " + (inventory.GetStoredGoods()[goodName]) + " ";
		}
        text.text += "\n";
        text.text += "Free: " + inventory.GetFreeCapacity().unitCapacity + "|" + inventory.GetFreeCapacity().massCapacity + "|" + inventory.GetFreeCapacity().volumeCapacity;
    }

    public void Deposit()
	{
        string goodName = dropdown.options[dropdown.value].text;
        uint amount = uint.Parse(input.text);

        if(inventory.DirectDeposit(new Stack(goodName, amount)))
            Debug.Log("Deposit " + amount + " " + goodName + " " + inventory.Deposit(new Stack(goodName, amount)));
        else
            Debug.Log("Insufficient Capacity");
        /*if(inventory.ReserveDeposit(new Stack(goodName, amount)))
            Debug.Log("Deposit " + amount + " " + goodName + " " + inventory.Deposit(new Stack(goodName, amount)));
        else
            Debug.Log("Insufficient Capacity");*/
	}

    public void Withdraw()
	{
        string goodName = dropdown.options[dropdown.value].text;
        uint amount = uint.Parse(input.text);

        if(inventory.DirectWithdraw(new Stack(goodName, amount)))
            Debug.Log("Withdraw " + amount + " " + goodName + " " + inventory.Withdraw(new Stack(goodName, amount)));
        else
            Debug.Log("Insufficient Stock");
        /*if(inventory.ReserveWithdraw(new Stack(goodName, amount)))
            Debug.Log("Withdraw " + amount + " " + goodName + " " + inventory.Withdraw(new Stack(goodName, amount)));
        else
            Debug.Log("Insufficient Stock");*/
	}
}
