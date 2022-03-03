using System;
using UnityEngine;

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

	public Capacity(Stack stack)
	{
		Good goodType = GoodManager.GetInstance().GetGood(stack.goodName);

		this.unitCapacity = (int)stack.amount;
		this.massCapacity = stack.amount * goodType.mass;
		this.volumeCapacity = stack.amount * goodType.volume;
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

	public static bool operator <=(Capacity lhs, Capacity rhs)
	{
		return (rhs.unitCapacity < 0 || lhs.unitCapacity <= rhs.unitCapacity)
		&& (rhs.massCapacity < 0.0f || lhs.massCapacity <= rhs.massCapacity)
		&& (rhs.volumeCapacity < 0.0f || lhs.volumeCapacity <= rhs.volumeCapacity);
	}

	public static bool operator >=(Capacity lhs, Capacity rhs)
	{
		return (lhs.unitCapacity < 0 || lhs.unitCapacity >= rhs.unitCapacity)
		&& (lhs.massCapacity < 0.0f || lhs.massCapacity >= rhs.massCapacity)
		&& (lhs.volumeCapacity < 0.0f || lhs.volumeCapacity >= rhs.volumeCapacity);
	}

	public static bool operator ==(Capacity lhs, Capacity rhs)
	{
		return lhs.unitCapacity == rhs.unitCapacity
			&& lhs.massCapacity == rhs.massCapacity
			&& lhs.volumeCapacity == rhs.volumeCapacity;
	}

	public static bool operator !=(Capacity lhs, Capacity rhs)
	{
		return lhs.unitCapacity != rhs.unitCapacity
			|| lhs.massCapacity != rhs.massCapacity
			|| lhs.volumeCapacity != rhs.volumeCapacity;
	}

	public uint ToAmount(string goodName)
	{
		Good goodType = GoodManager.GetInstance().GetGood(goodName);

		return (uint)Mathf.Min(unitCapacity, massCapacity / goodType.mass, volumeCapacity / goodType.volume);
	}

	public uint ToAmount(Good goodType)
	{
		return (uint)Mathf.Min(unitCapacity, massCapacity / goodType.mass, volumeCapacity / goodType.volume);
	}
}