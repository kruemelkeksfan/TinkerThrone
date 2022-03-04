using System;

[Serializable]
public struct ModuleInfo
{
    public string moduleName;
    public string materialId;
    public uint amountNeededPerStep;
    public uint buildingSteps;

    public ModuleInfo(string moduleName, string materialId, uint amountNeededPerStep, uint buildingSteps)
    {
        this.moduleName = moduleName;
        this.materialId = materialId;
        this.amountNeededPerStep = amountNeededPerStep;
        this.buildingSteps = buildingSteps;
    }

    public uint GetOverallAmount()
    {
        return amountNeededPerStep * buildingSteps;
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