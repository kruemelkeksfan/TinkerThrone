using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Stack
{
	public string goodName;
	public uint amount;

	public Stack(string goodName, uint amount)
	{
		this.goodName = goodName;
		this.amount = amount;
	}

	public static bool operator ==(Stack lhs, Stack rhs)
	{
		return (lhs.goodName == rhs.goodName && lhs.amount == rhs.amount);
	}

	public static bool operator !=(Stack lhs, Stack rhs)
	{
		return (lhs.goodName != rhs.goodName || lhs.amount != rhs.amount);
	}

	public static implicit operator string(Stack stack)
	{
		return "Stack: " + stack.goodName + " (" + stack.amount + ")";
	}

	public override bool Equals(object obj)
	{
		return obj is Stack stack &&
			   (goodName == stack.goodName && amount == stack.amount);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(goodName, amount);
	}
}
