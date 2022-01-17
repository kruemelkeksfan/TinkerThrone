using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodManager : MonoBehaviour
{
	private static GoodManager instance = null;

    [SerializeField] private Good[] goods = { };
	private Dictionary<string, Good> goodDictionary = null;

	public static GoodManager GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
		goodDictionary = new Dictionary<string, Good>();
		foreach(Good good in goods)
		{
			goodDictionary.Add(good.goodName, good);
		}

		instance = this;
	}

	public Good GetGood(string goodName)
	{
		return goodDictionary[goodName];
	}

	public Dictionary<string, Good> GetGoodDictionary()
	{
		return goodDictionary;
	}
}