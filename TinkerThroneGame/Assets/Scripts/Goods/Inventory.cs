using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	[Serializable]
	public struct Capacity
	{
		[Tooltip("How many Units of Goods an Inventory holds or can hold, negative Values for infinite.")]
		public int unitCapacity;
		[Tooltip("How much Mass an Inventory holds or can hold, negative Values for infinite.")]
		public float massCapacity;
		[Tooltip("How much Volume an Inventory holds or can hold, negative Values for infinite.")]
		public float volumeCapacity;

		public Capacity(int unitCapacity, float massCapacity, float volumeCapacity)
		{
			this.unitCapacity = unitCapacity;
			this.massCapacity = massCapacity;
			this.volumeCapacity = volumeCapacity;
		}

<<<<<<< HEAD
		public Capacity(string goodName, uint amount)
		{
			Good goodType = GoodManager.GetInstance().GetGood(goodName);

			this.unitCapacity = (int)amount;
			this.massCapacity = amount * goodType.mass;
			this.volumeCapacity = amount * goodType.volume;
=======
		public Capacity(Stack stack)
		{
			Good goodType = GoodManager.GetInstance().GetGood(stack.goodName);

			this.unitCapacity = (int)stack.amount;
			this.massCapacity = stack.amount * goodType.mass;
			this.volumeCapacity = stack.amount * goodType.volume;
>>>>>>> 0915882d2afe2c8bd1f377feb57835ec82fc4baa
		}

		public static Capacity operator +(Capacity lhs, Capacity rhs)
		{
			return new Capacity((lhs.unitCapacity >= 0 && rhs.unitCapacity >= 0) ? lhs.unitCapacity + rhs.unitCapacity : -1,
				(lhs.massCapacity >= 0.0f && rhs.massCapacity >= 0.0f) ? lhs.massCapacity + rhs.massCapacity : -1.0f,
				(lhs.volumeCapacity >= 0.0f && rhs.volumeCapacity >= 0.0f) ? lhs.volumeCapacity + rhs.volumeCapacity : -1.0f);
		}

		public static Capacity operator -(Capacity lhs, Capacity rhs)
		{
			return new Capacity((lhs.unitCapacity >= 0 && rhs.unitCapacity >= 0) ? lhs.unitCapacity - rhs.unitCapacity : -1,
				(lhs.massCapacity >= 0.0f && rhs.massCapacity >= 0.0f) ? lhs.massCapacity - rhs.massCapacity : -1.0f,
				(lhs.volumeCapacity >= 0.0f && rhs.volumeCapacity >= 0.0f) ? lhs.volumeCapacity - rhs.volumeCapacity : -1.0f);
		}

		public uint ToAmount(string goodName)
		{
			Good goodType = GoodManager.GetInstance().GetGood(goodName);

			return (uint) Mathf.Min(unitCapacity, massCapacity / goodType.mass, volumeCapacity / goodType.volume);
		}

		public uint ToAmount(Good goodType)
		{
			return (uint) Mathf.Min(unitCapacity, massCapacity / goodType.mass, volumeCapacity / goodType.volume);
		}
	}

	[SerializeField] private Capacity maxCapacity = new Capacity(-1, -1.0f, -1.0f);
	private GoodManager goodManager = null;
	private Dictionary<string, uint> storedGoods = null;
	private Capacity freeCapacity = new Capacity(-1, -1.0f, -1.0f);

	private void Awake()
	{
		storedGoods = new Dictionary<string, uint>();
		freeCapacity = maxCapacity;
	}

	private void Start()
	{
		goodManager = GoodManager.GetInstance();
	}

	// Stores a Stack of Goods in this Inventory.
	// Returns whether the storage operation was successful.
	// Storing will fail if not all Goods fit into the Inventory in which case no Goods at all will be stored.
	public bool Deposit(Stack goodStack)
	{
		Capacity requiredCapacity = new Capacity(goodStack);
		if(CheckCapacity(requiredCapacity))
		{
			if(!storedGoods.TryAdd(goodStack.goodName, goodStack.amount))
			{
				storedGoods[goodStack.goodName] += goodStack.amount;
			}

			freeCapacity -= requiredCapacity;

			return true;
		}

		return false;
	}

	// Retrieves a Stack of Goods from this Inventory.
	// Returns whether the Goods could be retrieved.
	// Retrieving will fail if an insufficient amount of Goods is stored in which case no Goods at all will be retrieved.
	public bool Withdraw(Stack goodStack)
	{
		if(storedGoods.ContainsKey(goodStack.goodName) && storedGoods[goodStack.goodName] >= goodStack.amount)
		{
			storedGoods[goodStack.goodName] -= goodStack.amount;
			freeCapacity += new Capacity(goodStack);

			return true;
		}

		return false;
	}

	// Returns if the required Capacity would be free in this Inventory.
	public bool CheckCapacity(Capacity requiredCapacity)
	{
		return (freeCapacity.unitCapacity < 0 || freeCapacity.unitCapacity >= requiredCapacity.unitCapacity)
			&& (freeCapacity.massCapacity < 0.0f || freeCapacity.massCapacity >= requiredCapacity.massCapacity)
			&& (freeCapacity.volumeCapacity < 0.0f || freeCapacity.volumeCapacity >= requiredCapacity.volumeCapacity);
	}

	// Returns the stored amount of a Good in this Inventory
	public uint GetGoodAmount(string goodName)
	{
		if(storedGoods.ContainsKey(goodName))
		{
			return storedGoods[goodName];
		}

		return 0;
	}

	// Returns a Dictionary with the names and amounts of all currently stored Goods in this Inventory
	public Dictionary<string, uint> GetStoredGoods()
	{
		return storedGoods;
	}

	// Returns the free Capacity of this Inventory.
	public Capacity GetFreeCapacity()
	{
		return freeCapacity;
	}
}
