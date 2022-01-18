using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
	[Serializable]
	public struct BuildingData
	{
		public Building[] buildings;
	}

	public GameObject testBuilding = null;

	private void Start()
	{
		BuildingData buildingData = new BuildingData();
		buildingData.buildings = new Building[1];
		buildingData.buildings[0] = testBuilding.GetComponent<Building>();
		using(StreamWriter writer = new StreamWriter(Application.persistentDataPath + Path.DirectorySeparatorChar + "Buildings.json", false))
		{
			writer.WriteLine(JsonUtility.ToJson(buildingData, true));
		}

		GameObject.Instantiate<GameObject>(testBuilding);
	}
}
