using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConstructionCostCounter : MonoBehaviour
{
    [SerializeField] private TextAsset constructionCost;
    [SerializeField] private List<Stack> materials;
    [SerializeField] private ModuleInfoArray info;
    [SerializeField] private bool set;
    private bool isSet = false;

    void Update()
    {
        if (set && !isSet)
        {
            isSet = true;
            Dictionary<string, ModuleInfo> moduleInfos = new();

            info = JsonUtility.FromJson<ModuleInfoArray>(constructionCost.text);

            foreach (ModuleInfo info in info.moduleInfos)
            {
                moduleInfos.Add(info.moduleName, info);
            }

            ModuleInfo moduleInfo;
            Dictionary<string, uint> stacks = new();
            foreach (Transform transform in this.gameObject.GetComponentsInChildren<Transform>())
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

            materials = new List<Stack>();
            foreach (string material in stacks.Keys)
            {
                materials.Add(new Stack(material, stacks[material]));
            }
        }
    }
}