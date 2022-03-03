using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
	public event StorageChangeHandler storageChanged;
	public delegate void StorageChangeHandler(Inventory inventory, EventArgs e);

	private Capacity maxCapacity = new Capacity(-1, -1.0f, -1.0f);
	private Dictionary<string, uint> reservedCapacities = null;
	private Dictionary<string, uint> reservedGoods = null;
	private Dictionary<string, uint> storedGoods = null;
	private Capacity reservedCapacity = new Capacity(0, 0.0f, 0.0f);
	private Capacity temporarilyOccupiedCapacity = new Capacity(0, 0.0f, 0.0f);
	private Capacity freeCapacity = new Capacity(-1, -1.0f, -1.0f);

	private void OnStorageChanged()
	{
		if (storageChanged != null)
		{
			storageChanged(this, EventArgs.Empty);
		}
	}

	public Inventory(Capacity maxCapacity)
	{
		this.maxCapacity = maxCapacity;

		reservedCapacities = new Dictionary<string, uint>();
		reservedGoods = new Dictionary<string, uint>();
		storedGoods = new Dictionary<string, uint>();
		foreach(String goodName in GoodManager.GetInstance().GetGoodDictionary().Keys)
		{
			reservedCapacities.Add(goodName, 0);
			reservedGoods.Add(goodName, 0);
			storedGoods.Add(goodName, 0);
		}

		freeCapacity = maxCapacity;
	}

	public bool ReserveDeposit(Stack goodStack)
	{
		Capacity requiredCapacity = new Capacity(goodStack);
		if(requiredCapacity <= freeCapacity)
		{
			reservedCapacities[goodStack.goodName] += goodStack.amount;
		
			reservedCapacity += requiredCapacity;
			freeCapacity -= requiredCapacity;

			return true;
		}

		return false;
	}

	public bool ReserveWithdraw(Stack goodStack)
	{
		if (storedGoods[goodStack.goodName] - reservedGoods[goodStack.goodName] >= goodStack.amount)
		{
			reservedGoods[goodStack.goodName] += goodStack.amount;

			temporarilyOccupiedCapacity += new Capacity(goodStack);

			return true;
		}

		return false;
	}

	public bool CancleReserveWithdraw(Stack goodStack)
	{
		if (reservedGoods[goodStack.goodName] >= goodStack.amount)
		{
			reservedGoods[goodStack.goodName] -= goodStack.amount;

			temporarilyOccupiedCapacity -= new Capacity(goodStack);

			return true;
		}

		return false;
	}



	// Stores a Stack of Goods in this Inventory.
	// Returns whether the storage operation was successful.
	// Storing will fail if not all Goods fit into the Inventory in which case no Goods at all will be stored.
	public bool Deposit(Stack goodStack)
	{
		if (reservedCapacities[goodStack.goodName] >= goodStack.amount)
		{
			reservedCapacities[goodStack.goodName] -= goodStack.amount;
			storedGoods[goodStack.goodName] += goodStack.amount;

			reservedCapacity -= new Capacity(goodStack);

			OnStorageChanged();
			return true;
		}
		else
		{
			Debug.LogError(this + " received a Deposit()-Request for " + goodStack + " without proper Reservation!");
		}

		return false;
	}

	// Retrieves a Stack of Goods from this Inventory.
	// Returns whether the Goods could be retrieved.
	// Retrieving will fail if an insufficient amount of Goods is stored in which case no Goods at all will be retrieved.
	public bool Withdraw(Stack goodStack)
	{
		if(reservedGoods[goodStack.goodName] >= goodStack.amount)
		{
			storedGoods[goodStack.goodName] -= goodStack.amount;
			reservedGoods[goodStack.goodName] -= goodStack.amount;

			Capacity requiredCapacity = new Capacity(goodStack);
			temporarilyOccupiedCapacity -= requiredCapacity;
			freeCapacity += requiredCapacity;

			OnStorageChanged();
			return true;
		}
		else
		{
			Debug.LogError(this + " received a Withdraw()-Request for " + goodStack + " without proper Reservation!");
		}

		return false;
	}

	public bool DirectDeposit(Stack goodStack)
	{
		Capacity requiredCapacity = new Capacity(goodStack);
		if(requiredCapacity <= freeCapacity)
		{
			storedGoods[goodStack.goodName] += goodStack.amount;

			freeCapacity -= new Capacity(goodStack);

			return true;
		}

		return false;
	}

	public bool DirectWithdraw(Stack goodStack)
	{
		if(storedGoods[goodStack.goodName] >= goodStack.amount)
		{
			storedGoods[goodStack.goodName] -= goodStack.amount;

			freeCapacity += new Capacity(goodStack);

			return true;
		}

		return false;
	}

	// Returns the stored amount of a Good in this Inventory
	public uint GetGoodAmount(string goodName)
	{
		return storedGoods[goodName];
	}

	// Returns a Dictionary with the names and amounts of all currently stored Goods in this Inventory
	public Dictionary<string, uint> GetStoredGoods()
	{
		return storedGoods;
	}

	public Dictionary<string, uint> GetReservedGoods()
	{
		return reservedGoods;
	}

	public Dictionary<string, uint> GetReservedCapacities()
	{
		return reservedCapacities;
	}

	public Capacity GetReservedCapacity()
	{
		return reservedCapacity;
	}

	public Capacity GetTemporarilyOccupiedCapacity()
	{
		return temporarilyOccupiedCapacity;
	}

	// Returns the free Capacity of this Inventory.
	public Capacity GetFreeCapacity()
	{
		return freeCapacity;
	}
}
