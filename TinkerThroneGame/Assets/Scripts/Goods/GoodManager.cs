using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GoodManager : MonoBehaviour
{
	[Serializable]
	public struct GoodData
	{
		public Good[] goods;
	}

	private static GoodManager instance = null;

    [SerializeField] private TextAsset goodDataFile = null;
	private Dictionary<string, Good> goodDictionary = null;

	public static GoodManager GetInstance()
	{
		return instance;
	}

	private void Awake()
	{
		GoodData goodData = JsonUtility.FromJson<GoodData>(goodDataFile.text);
		goodDictionary = new Dictionary<string, Good>();
		foreach(Good good in goodData.goods)
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