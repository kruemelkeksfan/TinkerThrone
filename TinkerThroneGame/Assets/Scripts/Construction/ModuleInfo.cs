using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public struct ModuleInfo
{
    public string moduleName;
    public int materialId;
    public int amountNeeded;
    public int buildingSteps;

    public ModuleInfo(string moduleName, int materialId, int amountNeeded, int buildingSteps)
    {
        this.moduleName = moduleName;
        this.materialId = materialId;
        this.amountNeeded = amountNeeded;
        this.buildingSteps = buildingSteps;
    }
}

[Serializable]
public struct ModuleInfoArray
{
    public ModuleInfo[] moduleInfos;

    public ModuleInfoArray(ModuleInfo[] moduleInfos)
    {
        this.moduleInfos = moduleInfos;
    }
}