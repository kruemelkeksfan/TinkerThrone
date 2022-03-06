using System.Collections.Generic;
using UnityEngine;

public class ConstructionCostManager : MonoBehaviour
{
    [SerializeField] private TextAsset constructionCost;
    private static ConstructionCostManager instance;
    private readonly Dictionary<string, ModuleInfo> moduleInfos = new();

    public static ConstructionCostManager GetInstance()
    {
        return instance;
    }

    public bool TryGetModuleCost(string moduleName, out ModuleInfo moduleInfo)
    {
        moduleInfo = new();
        string[] splitName = moduleName.Split('.');
        if (splitName.Length < 2 || !moduleInfos.ContainsKey(splitName[1])) return false;
        moduleInfo = moduleInfos[splitName[1]];
        return true;
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

    public Stack[] GetCostForModel(Transform[] modules)
    {
        ModuleInfo moduleInfo;
        Dictionary<string, uint> stacks = new();
        foreach (Transform transform in modules)
        {
            string[] splitName = transform.name.Split('.');
            if (splitName.Length < 2) continue;
            moduleInfo = moduleInfos[splitName[1]];
            if (stacks.ContainsKey(moduleInfo.materialId))
            {
                stacks[moduleInfo.materialId] += moduleInfo.GetOverallAmount();
            }
            else
            {
                stacks.Add(moduleInfo.materialId, moduleInfo.GetOverallAmount());
            }
        }

        List<Stack> materials = new();
        foreach (string material in stacks.Keys)
        {
            materials.Add(new Stack(material, stacks[material]));
        }

        return materials.ToArray();
    }
}