using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : LogisticsUser
{
    Transform[] parts;
    Transform inventoryTransform;
    GameObject model;
    GameObject finalModel;
    ConstructionCostManager constructionCostManager;
    List<Villager> assignedIdleConstructionVillagers = new();
    List<Villager> assignedConstructionVillagers = new();
    Dictionary<string, LogisticValue> logisticValuesDictionary = new();
    JobsManager jobsManager;

    ModuleInfo currentModuleInfo = new("", "", 0, 0);

    ConstructionJob currentConstructionJob;
    bool currentJobAssigned = true;
    int lowPriority = 3;
    int highPriority = 2;
    int moduleCounter = -1;
    int modulePartCounter = 0;
    int requestedVillagers = 0;
    bool finishedAssigning = false;

    public int GetAssignedVillagers()
    {
        return assignedIdleConstructionVillagers.Count + assignedConstructionVillagers.Count;
    }

    public override Vector3 GetLogisticPosition()
    {
        return inventoryTransform.position;
    }

    private void Start()
    {
        jobsManager = JobsManager.GetInstance();
        constructionCostManager = ConstructionCostManager.GetInstance();
    }

    public bool AssignVillager(Villager villager)
    {
        if (finishedAssigning)
        {
            return false;
        }
        if (!TryAssignConstructionJob(villager))
        {
            currentJobAssigned = false;
            assignedIdleConstructionVillagers.Add(villager);
        }
        return true;
    }

    public bool RequestVillagers(int count)
    {
        if (assignedIdleConstructionVillagers.Count < count)
        {
            requestedVillagers = count - assignedIdleConstructionVillagers.Count;
        }
        if (assignedIdleConstructionVillagers.Count > 0)
        {
            for (int i = assignedIdleConstructionVillagers.Count - 1; i >= 0; i--)
            {
                jobsManager.UnassignVillager(assignedIdleConstructionVillagers[i], true);
                assignedIdleConstructionVillagers.RemoveAt(i);
            }
            return true;
        }
        else
        {
            requestedVillagers = count;
        }
        return false;
    }

    public void StartConstruction(GameObject model, GameObject finalModel, Transform inventoryTransform, LogisticValue[] logisticValues)
    {
        //deactivate Parts and set Array
        this.model = model;
        List<Transform> modelParts = new(model.GetComponentsInChildren<Transform>());
        modelParts.RemoveAt(0);
        foreach(Transform part in modelParts)
        {
            part.gameObject.SetActive(false);
        }
        parts = modelParts.ToArray();
        //set finalModel
        this.finalModel = finalModel;
        //set Inventory World position
        this.inventoryTransform = inventoryTransform;
        //set logisticValues and fill Dictionary
        specialLogisticValues = logisticValues;
        inventoryCapacity = WorldConsts.capacity;
        SetLogisticsValues();
        foreach(LogisticValue logisticValue in logisticValues)
        {
            logisticValuesDictionary.Add(logisticValue.goodName, logisticValue);
        }
        LogisticsManager.GetInstance().AddInventory(this);
        jobsManager = JobsManager.GetInstance();
        constructionCostManager = ConstructionCostManager.GetInstance();
        //Add Site to JobManager to recive construction villager
        jobsManager.AddConstructionSite(this);
    }

    public void FinishConstructionJob(ConstructionJob constructionJob, Villager villager)
    {
        if(constructionJob.ModuleInfo.GetBuildingProgress() < 1)
        {
            //TODO
        }
        else
        {
            //Already done
        }

        //move to else
        constructionJob.Target.SetActive(true);


        if (finishedAssigning && constructionJob.Target == parts[^1] &&  assignedConstructionVillagers.Count == 1)
        {
            LogisticsManager.GetInstance().RemoveInventory(this);
            Building building = gameObject.GetComponent<Building>();
            GameObject newModel = Instantiate(finalModel, model.transform.position, model.transform.rotation, model.transform.parent);
            GameObject.Destroy(model);
            building.currentModel = newModel;
            assignedConstructionVillagers.Remove(villager);
            jobsManager.UnassignVillager(villager, true);
            for(int i = assignedIdleConstructionVillagers.Count - 1; i >= 0; i--)
            {
                jobsManager.UnassignVillager(assignedIdleConstructionVillagers[i], true);
                assignedIdleConstructionVillagers.RemoveAt(i);
            }
            jobsManager.RemoveConstructionSite(this);
            building.ActivateBuilding();
            GameObject.Destroy(this);
            return;
        }

        if(requestedVillagers > 0)
        {
            jobsManager.UnassignVillager(villager);
            assignedIdleConstructionVillagers.Remove(villager);
            requestedVillagers--;
            return;
        }

        if (!TryAssignConstructionJob(villager))
        {
            currentJobAssigned = false;
            assignedConstructionVillagers.Remove(villager);
            assignedIdleConstructionVillagers.Add(villager);
        }
    }

    public bool TryAssignConstructionJob(Villager villager)
    {
        if (currentJobAssigned)
        {
            if (currentModuleInfo.buildingSteps >= modulePartCounter)
            {
                modulePartCounter = 1;
                moduleCounter++;
                if(moduleCounter >= parts.Length) 
                {
                    finishedAssigning = true;
                    return false; 
                }
                if (constructionCostManager.TryGetModuleCost(parts[moduleCounter].name, out ModuleInfo newModuleInfo))
                {
                    if(currentModuleInfo.materialId != newModuleInfo.materialId)
                    {
                        if (logisticValuesDictionary.ContainsKey(currentModuleInfo.materialId))
                        {
                            LogisticValue changingValue = logisticValuesDictionary[currentModuleInfo.materialId];
                            changingValue.logisticsPriorityBeeingEmpty = lowPriority;
                            logisticValuesDictionary[currentModuleInfo.materialId] = changingValue;
                        }
                        if (logisticValuesDictionary.ContainsKey(newModuleInfo.materialId))
                        {
                            LogisticValue changingValue = logisticValuesDictionary[newModuleInfo.materialId];
                            changingValue.logisticsPriorityBeeingEmpty = highPriority;
                            logisticValuesDictionary[newModuleInfo.materialId] = changingValue;
                        }
                        SetLogisticsValues();
                    }
                    currentModuleInfo = newModuleInfo;
                }
                else
                {
                    Debug.LogWarning("could not finde a corresponding module for " + parts[moduleCounter].name);
                    return false;
                }

                currentConstructionJob = new ConstructionJob(this, parts[moduleCounter].gameObject, moduleCounter, currentModuleInfo);
            }
            else { modulePartCounter++; }
        }

        if (inventory.ReserveWithdraw(currentConstructionJob.Stack))
        {
            LogisticValue changingValue = logisticValuesDictionary[currentModuleInfo.materialId];
            changingValue.targetAmount -= currentConstructionJob.Stack.amount;
            logisticValuesDictionary[currentModuleInfo.materialId] = changingValue;

            if (!assignedConstructionVillagers.Contains(villager))
            {
                assignedConstructionVillagers.Add(villager);
            }
            villager.StartCoroutine(villager.DoConstructionJob(currentConstructionJob));
            return true;
        }
        return false;
    }
}