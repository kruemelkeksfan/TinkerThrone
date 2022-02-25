using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionCostManager : MonoBehaviour
{
    [SerializeField] TextAsset constructionCost;
    static ConstructionCostManager instance;
    Dictionary<string, ModuleInfo> moduleInfos = new Dictionary<string, ModuleInfo>();
    public static ConstructionCostManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;

        ModuleInfoArray infos = JsonUtility.FromJson<ModuleInfoArray>(constructionCost.text);

        foreach (ModuleInfo info in infos.moduleInfos)
        {
            moduleInfos.Add(info.moduleName, info);
        }
    }

    public bool TryGetModuleCost(string moduleName, out ModuleInfo moduleInfo)
    {
        moduleInfo = new();
        string[] splitName = moduleName.Split('.');
        if (splitName.Length < 2 || !moduleInfos.ContainsKey(splitName[1])) return false;
        moduleInfo = moduleInfos[splitName[1]];
        return true;
    }

}
