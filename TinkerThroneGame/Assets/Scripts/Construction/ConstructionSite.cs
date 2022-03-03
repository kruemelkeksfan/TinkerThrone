using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSite : LogisticsUser
{
    private readonly List<Villager> assignedIdleConstructionVillagers = new();
    private readonly List<Villager> assignedConstructionVillagers = new();
    private readonly int lowPriority = 3;
    private readonly int highPriority = 2;

    private ConstructionCostManager constructionCostManager;
    private JobsManager jobsManager;
    private NavMeshManager navMeshManager;
    private Transform[] parts;
    private Transform inventoryTransform;
    private GameObject model;
    private GameObject finalModel;

    private ConstructionJob currentConstructionJob;
    private ModuleInfo currentModuleInfo = new("", "", 0, 0);
    private bool currentJobAssigned = true;
    private int moduleCounter = -1;
    private int modulePartCounter = 0;
    private int requestedVillagers = 0;
    private bool finishedAssigning = false;

    public int GetAssignedVillagers()
    {
        return assignedIdleConstructionVillagers.Count + assignedConstructionVillagers.Count;
    }

    public bool IsFinishedAssigningJobs()
    {
        return finishedAssigning;
    }

    public override Vector3 GetLogisticPosition()
    {
        return inventoryTransform.position;
    }

    private void InventoryChanged(Inventory inventory, EventArgs e)
    {
        foreach (Villager villager in assignedIdleConstructionVillagers)
        {
            if (!TryAssignConstructionJob(villager))
            {
                break;
            }
        }
    }

    private void Start()
    {
        jobsManager = JobsManager.GetInstance();
        constructionCostManager = ConstructionCostManager.GetInstance();
        navMeshManager = NavMeshManager.GetInstance();
    }

    public bool AssignVillager(Villager villager)
    {
        if (finishedAssigning)
        {
            return false;
        }
        if (!TryAssignConstructionJob(villager))
        {
            villager.Move(inventoryTransform.position + Vector3.forward * 3); //TODO set to boreder of Inventoryzone
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
        foreach (Transform part in modelParts)
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
        inventory.storageChanged += new Inventory.StorageChangeHandler(InventoryChanged);
        LogisticsManager.GetInstance().AddInventory(this);
        jobsManager = JobsManager.GetInstance();
        constructionCostManager = ConstructionCostManager.GetInstance();
        //Add Site to JobManager to recive construction villager
        jobsManager.AddConstructionSite(this);
    }

    public void FinishConstructionJob(ConstructionJob constructionJob, Villager villager)
    {
        if (constructionJob.ModuleInfo.GetBuildingProgress() < 1)
        {
            //TODO
        }
        else
        {
            //Already done
        }

        //move to else
        constructionJob.Target.SetActive(true);
        //Debug.Log("Job finished: " + constructionJob.Target.name);

        if (finishedAssigning && constructionJob.Target == parts[^1].gameObject && assignedConstructionVillagers.Count == 1)
        {
            //Debug.Log("construction finished");
            LogisticsManager.GetInstance().RemoveInventory(this);
            Building building = gameObject.GetComponent<Building>();
            GameObject newModel = Instantiate(finalModel, model.transform.position, model.transform.rotation, model.transform.parent);
            GameObject.Destroy(model);
            building.SetCurrentModel(newModel);
            assignedConstructionVillagers.Remove(villager);
            jobsManager.UnassignVillager(villager, true);
            for (int i = assignedIdleConstructionVillagers.Count - 1; i >= 0; i--)
            {
                jobsManager.UnassignVillager(assignedIdleConstructionVillagers[i], true);
                assignedIdleConstructionVillagers.RemoveAt(i);
            }
            jobsManager.RemoveConstructionSite(this);
            building.ActivateBuilding();
            navMeshManager.UpdateNavMesh();
            GameObject.Destroy(this);
            return;
        }

        navMeshManager.UpdateNavMesh();

        if (requestedVillagers > 0)
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
            currentJobAssigned = false;
            if (currentModuleInfo.buildingSteps >= modulePartCounter)
            {
                modulePartCounter = 1;
                moduleCounter++;
                if (moduleCounter >= parts.Length)
                {
                    finishedAssigning = true;
                    return false;
                }
                if (constructionCostManager.TryGetModuleCost(parts[moduleCounter].name, out ModuleInfo newModuleInfo))
                {
                    if (currentModuleInfo.materialId != newModuleInfo.materialId)
                    {
                        if (logisticValues.ContainsKey(currentModuleInfo.materialId))
                        {
                            LogisticValue changingValue = logisticValues[currentModuleInfo.materialId];
                            changingValue.logisticsPriorityBeeingEmpty = lowPriority;
                            logisticValues[currentModuleInfo.materialId] = changingValue;
                        }
                        if (logisticValues.ContainsKey(newModuleInfo.materialId))
                        {
                            LogisticValue changingValue = logisticValues[newModuleInfo.materialId];
                            changingValue.logisticsPriorityBeeingEmpty = highPriority;
                            logisticValues[newModuleInfo.materialId] = changingValue;
                        }
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
            if (!assignedConstructionVillagers.Contains(villager))
            {
                assignedConstructionVillagers.Add(villager);
                //Debug.Log("assigned: " + villager + " Job: " + currentConstructionJob.Target.gameObject.name + " ModNr:" + moduleCounter);
            }
            villager.StartCoroutine(villager.DoConstructionJob(currentConstructionJob));
            currentJobAssigned = true;
            return true;
        }
        return false;
    }

    public void ReduceTargetAmount(Stack stack)
    {
        if (logisticValues.ContainsKey(stack.goodName))
        {
            LogisticValue changingValue = logisticValues[stack.goodName];
            changingValue.targetAmount -= stack.amount;
            logisticValues[stack.goodName] = changingValue;
        }
    }
}