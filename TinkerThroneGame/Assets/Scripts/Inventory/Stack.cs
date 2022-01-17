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
}
