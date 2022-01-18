using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Recipe
{
	public Stack[] ingredients;
	public Stack[] products;
	public float productionTime;
}
