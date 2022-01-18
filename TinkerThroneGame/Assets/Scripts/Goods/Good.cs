using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Good
{
	public string goodName;
	[TextArea(3, 5)] public string decription;
	[Tooltip("Mass in Tons per Unit, 1 Unit is 1 Piece.")]
	public float mass;
	[Tooltip("The Volume in m^3 1 Unit of this Item takes up in Storage.")]
	public int volume;
	[Tooltip("Factor by which this Cargo contributes to Fires.")]
	public float flammability;
	[Tooltip("Base Price of this Good.")]
	public int price;
}
