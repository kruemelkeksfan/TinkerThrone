using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestController : MonoBehaviour
{
    private static ForestController instance = null;

    [Tooltip("Good Name for raw Wood from Trees.")]
    [SerializeField] private string rawWoodGoodName = "Wood";
    [Tooltip("Area of the Forest in km^2.")]
    [SerializeField] private float forestArea = 10.0f;
    // Source: https://de.wikipedia.org/wiki/Borealer_Nadelwald
    // 100-300t biomass/hectar == 10000-30000t biomass/km^2 ~= 50000t wood/km^2
    [Tooltip("Amount of Wood in t per km^2.")]
    [SerializeField] private float forestDensity = 50000.0f;
    [Tooltip("Min Amount of Wood in this Forest as a Fraction of the Max Amount.")]
    [SerializeField] private float minWoodAmount = 0.2f;
    [Tooltip("Max Travel Distance is determined by the Size of the deforested Area multiplied by ventureAreaMultiplier.")]
    [SerializeField] private float ventureAreaMultiplier = 1.2f;
    [Tooltip("Time from planting a Tree to maximum Growth in Years.")]
    [SerializeField] private float woodGrowTime = 80.0f;
    [Tooltip("Time between Wood Growing Update Ticks in in-Game-Time Seconds.")]
    [SerializeField] private float woodGrowInterval = 600.0f;
    private float forestRadius = 0.0f;
    private float maxWoodAmount = 0.0f;
    private float woodAmount = 0.0f;
    private float woodGrowthAmount = 0.0f;
    private WaitForSeconds waitForWoodGrowInterval = null;

    public static ForestController GetInstance()
	{
        return instance;
	}

    private void Awake()
	{
        instance = this;
	}

	private void Start()
	{
        // Max travel distance in meters, assuming the forest is a circle centered around the village and neglecting the village area
        // A = pi * r^2
        forestRadius = Mathf.Sqrt(forestArea / Mathf.PI) * 1000.0f;

		forestDensity /= GoodManager.GetInstance().GetGood(rawWoodGoodName).mass;                                       // Convert forest density from t to units

        maxWoodAmount = forestArea * forestDensity;
        minWoodAmount *= maxWoodAmount;                                                                                 // Convert minWoodAmount from fraction to units
        woodAmount = maxWoodAmount;

        woodGrowthAmount = (float) ((woodGrowTime * 365.0 * 24.0 * 60.0 * 60.0) / woodGrowInterval) * maxWoodAmount;

        waitForWoodGrowInterval = new WaitForSeconds(woodGrowInterval);
        StartCoroutine(GrowWood());
	}

    // Fells trees in this forest. If called successfully, this method already deducts the woodAmount from this forest's woodReserves.
    // Accepts the movementSpeed of the lumberjacks vehicle and the woodAmount the lumberjack is about to extract from this forest.
    // If the forest contains enough wood, the method returns the travel time to the trees and back, else it returns a negative value.
	public float GetWood(float movementSpeed, float woodAmount)
	{
        if(this.woodAmount - woodAmount > minWoodAmount)
		{
            this.woodAmount -= woodAmount;

            float ventureArea = forestArea * (1.0f - (this.woodAmount / maxWoodAmount)) * ventureAreaMultiplier;
            return ((Random.Range(0.0f, Mathf.Sqrt(ventureArea / Mathf.PI) * 1000.0f)) / movementSpeed) * 2.0f;           // Travel time to a random point inside the forest and back
		}

        return -1.0f;
	}

    private IEnumerator GrowWood()
	{
        while(true)
		{
            yield return waitForWoodGrowInterval;

            woodAmount = Mathf.Clamp(woodAmount + woodGrowthAmount, 0.0f, maxWoodAmount);
		}
	}
}
