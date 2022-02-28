using UnityEngine;

public struct ConstructionJob
{
    public ConstructionSite ConstructionSite { get; private set; }
    public GameObject Target { get; private set; }
    public int ModuleNumber { get; private set; }
    public ModuleInfo ModuleInfo { get; private set; }
    private Stack stack;

    public Stack Stack { get { return stack; } }

    public ConstructionJob(ConstructionSite constructionSite, GameObject target, int moduleNumber, ModuleInfo moduleInfo)
    {
        ConstructionSite = constructionSite;
        Target = target;
        ModuleInfo = moduleInfo;
        ModuleNumber = moduleNumber;
        this.stack = new Stack(moduleInfo.materialId, moduleInfo.amountNeededPerStep);
    }
}