using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConstructionCostCounter : MonoBehaviour
{
    [SerializeField] TextAsset constructionCost;
    [SerializeField] List<Stack> materials;
    [SerializeField] ModuleInfoArray info;
    [SerializeField] bool set;
    bool isSet;
    // Start is called before the first frame update
    void Update()
    {
        if (set && !isSet)
        {
            isSet = true;
            Dictionary<string, ModuleInfo> moduleInfos = new Dictionary<string, ModuleInfo>();

            info = JsonUtility.FromJson<ModuleInfoArray>(constructionCost.text);

            foreach (ModuleInfo info in info.moduleInfos)
            {
                moduleInfos.Add(info.moduleName, info);
            }

            ModuleInfo moduleInfo;
            Dictionary<string, uint> stacks = new Dictionary<string, uint>();
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
