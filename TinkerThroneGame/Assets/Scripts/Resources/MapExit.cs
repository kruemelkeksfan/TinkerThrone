using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExit : MonoBehaviour
{
    [SerializeField] private ForestController forestController = null;

	private void Start()
	{
		forestController = ForestController.GetInstance();
	}

	// Fells trees in the forest. If called successfully, this method already deducts the woodAmount from the forest's woodReserves.
    // Accepts the movementSpeed of the lumberjacks vehicle and the woodAmount the lumberjack is about to extract from this forest.
    // If the forest contains enough wood, the method returns the travel time to the trees and back, else it returns a negative value.
	public float GetWood(float movementSpeed, float woodAmount)
	{
		return forestController.GetWood(movementSpeed, woodAmount);
	}
}
