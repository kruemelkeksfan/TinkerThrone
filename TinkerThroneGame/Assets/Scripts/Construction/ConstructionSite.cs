using System;
using System.Collections;
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
    private GameObject constructionModel;
    private GameObject finalModel;
    private GameObject[] constructionCorners;

    private ConstructionJob currentConstructionJob;
    private ModuleInfo currentModuleInfo = new("", "", 0, 0);
    private bool currentJobAssigned = true;
    private int moduleCounter = -1;
    private uint moduleStepCounter = 100;
    private int requestedVillagers = 0;
    private bool finishedAssigning = false;
    private bool isConstructing;

    public int GetAssignedVillagers()
    {
        return assignedIdleConstructionVillagers.Count + assignedConstructionVillagers.Count;
    }

    public bool IsFinishedAssigningJobs()
    {
        return finishedAssigning;
    }

    public bool IsConstructing()
    {
        return isConstructing;
    }

    public override Vector3 GetLogisticPosition()
    {
        return inventoryTransform.position;
    }

    private void InventoryChanged(Inventory inventory, EventArgs e)
    {
        foreach (Villager villager in assignedIdleConstructionVillagers)
        {
            if (!TryAssignJob(villager))
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

    private void OnDestroy()
    {
        foreach(GameObject corner in constructionCorners)
        {
            GameObject.Destroy(corner);
        }
    }

    public bool AssignVillager(Villager villager)
    {
        if (finishedAssigning)
        {
            return false;
        }
        if (!TryAssignJob(villager))
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

    private bool TryAssignJob(Villager villager)
    {
        if (isConstructing)
        {
            return TryAssignConstructionJob(villager);
        }
        else
        {
            return TryAssignDeconstructionJob(villager);
        }
    }

    private void InitConstructionSite(GameObject constructionModel, GameObject finalModel, Transform inventoryTransform, LogisticValue[] logisticValues, bool alreadyConstructing, bool isDeconstructing = false)
    {

        currentJobAssigned = true;
        finishedAssigning = false;
        StopAllCoroutines();
        
        if (!alreadyConstructing)
        {
            //spawn constructionCorners
            GameObject prefab = ConstructionPlacementManager.GetInstance().GetConstructioCornerPrefab();
            BoxCollider selectionColider = gameObject.GetComponent<BoxCollider>();
            constructionCorners = new GameObject[] {
                Instantiate(prefab, transform.position + new Vector3(selectionColider.size.x * 0.5f, 0, selectionColider.size.z * 0.5f), Quaternion.identity, transform),
                Instantiate(prefab, transform.position + new Vector3(selectionColider.size.x * 0.5f, 0, -selectionColider.size.z * 0.5f), Quaternion.identity, transform),
                Instantiate(prefab, transform.position + new Vector3(-selectionColider.size.x * 0.5f, 0, -selectionColider.size.z * 0.5f), Quaternion.identity, transform),
                Instantiate(prefab, transform.position + new Vector3(-selectionColider.size.x * 0.5f, 0, selectionColider.size.z * 0.5f), Quaternion.identity, transform)
            };
            //set part Array
            this.constructionModel = constructionModel;
            List<Transform> modelParts = new(constructionModel.GetComponentsInChildren<Transform>());
            modelParts.RemoveAt(0);
            parts = modelParts.ToArray();
            if (!alreadyConstructing && isDeconstructing)
            {
                moduleCounter = parts.Length;
            }
            //set finalModel
            this.finalModel = finalModel;
            //set Inventory World position
            this.inventoryTransform = inventoryTransform;
        }
        //set logisticValues
        specialLogisticValues = logisticValues;
        inventoryCapacity = WorldConsts.capacity;
        SetLogisticsValues();
        if (!alreadyConstructing)
        {
            //Add inventory listener
            inventory.storageChanged += new Inventory.StorageChangeHandler(InventoryChanged);
            //add to logisticSystem
            LogisticsManager.GetInstance().AddInventory(this);
            //get Singletons
            jobsManager = JobsManager.GetInstance();
            //Add Site to JobManager to recive construction villager
            jobsManager.AddConstructionSite(this);
        }
        else
        {
            LogisticsManager.GetInstance().UpdateLogisticsJobs();
            jobsManager.AssignConstructionVillagers();
        }
    }

    private void FinishAnyConstructionJob(Villager villager)
    {
        if (requestedVillagers > 0)
        {
            jobsManager.UnassignVillager(villager);
            assignedIdleConstructionVillagers.Remove(villager);
            requestedVillagers--;
            return;
        }

        if (!TryAssignJob(villager))
        {
            villager.Move(inventoryTransform.position + Vector3.forward * 3); //TODO set to boreder of Inventoryzone
            currentJobAssigned = false;
            assignedConstructionVillagers.Remove(villager);
            assignedIdleConstructionVillagers.Add(villager);
        }
    }


    #region construction

    public void StartConstruction(GameObject constructionModel, GameObject finalModel, Transform inventoryTransform, bool alreadyConstructing = false)
    {
        isConstructing = true;
        constructionLogisticValues = logisticValues;
        constructionCostManager = ConstructionCostManager.GetInstance();
        Stack[] stacks;
        if (!alreadyConstructing)
        {
            stacks = constructionCostManager.GetCostForModel(constructionModel.GetComponentsInChildren<Transform>());
        }
        else
        {
            List<Transform> partsToConstruct = new(parts);
            if(moduleCounter > 0)
            {
                partsToConstruct.RemoveRange(0, moduleCounter);
            }
            stacks = constructionCostManager.GetCostForModel(partsToConstruct.ToArray());
        }
        List<LogisticValue> logisticValues = new() { new LogisticValue(stacks[0].goodName, highPriority, lowPriority, stacks[0].amount) };
        for(int i = 1; i < stacks.Length; i++)
        {
            logisticValues.Add(new LogisticValue(stacks[i].goodName, lowPriority, lowPriority, stacks[i].amount));
        }

        InitConstructionSite(constructionModel, finalModel, inventoryTransform, logisticValues.ToArray(), alreadyConstructing);

        if (!alreadyConstructing)
        {
            foreach (Transform part in parts)
            {
                part.gameObject.SetActive(false);
            }

            moduleCounter = -1;
        }

        foreach (Villager villager in assignedIdleConstructionVillagers)
        {
            if (!TryAssignConstructionJob(villager))
            {
                break;
            }
        }
    }

    public void FinishConstructionJob(ConstructionJob constructionJob, Villager villager)
    {
        if (constructionJob.ModuleStep < constructionJob.ModuleInfo.buildingSteps)
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
            GameObject newModel = Instantiate(finalModel, constructionModel.transform.position, constructionModel.transform.rotation, constructionModel.transform.parent);
            GameObject.Destroy(constructionModel);
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

        FinishAnyConstructionJob(villager);
    }

    public bool TryAssignConstructionJob(Villager villager)
    {
        if (villager.HasJob) return false;
        if (currentJobAssigned)
        {
            currentJobAssigned = false;
            if (currentModuleInfo.buildingSteps <= moduleStepCounter)
            {
                moduleStepCounter = 1;
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

                currentConstructionJob = new ConstructionJob(this, parts[moduleCounter].gameObject, moduleStepCounter, currentModuleInfo);
            }
            else { moduleStepCounter++; }
        }

        if (inventory.ReserveWithdraw(currentConstructionJob.Stack))
        {
            if (!assignedConstructionVillagers.Contains(villager))
            {
                assignedConstructionVillagers.Add(villager);
            }
            //Debug.Log("assigned: " + villager + " Job: " + currentConstructionJob.Target.gameObject.name + " ModNr:" + moduleCounter + " ModpartNr:" + moduleStepCounter);
            villager.StartCoroutine(villager.DoConstructionJob(currentConstructionJob));
            currentJobAssigned = true;
            return true;
        }
        else
        {
            Debug.Log("missing " + currentConstructionJob.Stack.goodName + " " + currentConstructionJob.Stack.amount);
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

    #endregion

    #region deconstruction 

    public void StartDeconstruction(GameObject constructionModel, GameObject currentModel, GameObject finalModel, Transform inventoryTransform, bool alreadyConstructing = false)
    {
        isConstructing = false;

        if (!alreadyConstructing)
        {
            Building building = gameObject.GetComponent<Building>();
            constructionModel = Instantiate(constructionModel, currentModel.transform.position, currentModel.transform.rotation, currentModel.transform.parent);
            GameObject.Destroy(currentModel);
            building.SetCurrentModel(constructionModel);
        }
        constructionCostManager = ConstructionCostManager.GetInstance();
        Stack[] stacks = constructionCostManager.GetCostForModel(constructionModel.GetComponentsInChildren<Transform>());

        List<LogisticValue> logisticValues = new();
        foreach (Stack stack in stacks)
        {
            logisticValues.Add(new LogisticValue(stack.goodName, 10, 10, 0));
        }

        InitConstructionSite(constructionModel, finalModel, inventoryTransform, logisticValues.ToArray(), alreadyConstructing, true);

        foreach (Villager villager in assignedIdleConstructionVillagers)
        {
            if (!TryAssignDeconstructionJob(villager))
            {
                break;
            }
        }
    }

    public bool TryAssignDeconstructionJob(Villager villager)
    {
        if (finishedAssigning) return false;
        if (villager.HasJob) return false;

        if (currentJobAssigned)
        {
            currentJobAssigned = false;
            if (moduleStepCounter <= 1)
            {
                moduleCounter--;
                if (moduleCounter <= -1)
                {
                    finishedAssigning = true;
                    StartCoroutine(FinishDeconstruction());
                    return false;
                }
                if (constructionCostManager.TryGetModuleCost(parts[moduleCounter].name, out ModuleInfo newModuleInfo))
                {
                    currentModuleInfo = newModuleInfo;
                    moduleStepCounter = currentModuleInfo.buildingSteps;
                }
                else
                {
                    Debug.LogWarning("could not finde a corresponding module for " + parts[moduleCounter].name);
                    return false;
                }

                currentConstructionJob = new ConstructionJob(this, parts[moduleCounter].gameObject, moduleStepCounter, currentModuleInfo);
            }
            else { moduleStepCounter--; }
        }

        if (inventory.ReserveDeposit(currentConstructionJob.Stack))
        {
            if (!assignedConstructionVillagers.Contains(villager))
            {
                assignedConstructionVillagers.Add(villager);
                //Debug.Log("assigned: " + villager + " Job: " + currentConstructionJob.Target.gameObject.name + " ModNr:" + moduleCounter);
            }
            villager.StartCoroutine(villager.DoDeconstructionJob(currentConstructionJob));
            currentJobAssigned = true;
            return true;
        }
        return false;
    }

    public void DeconstructModule(ConstructionJob constructionJob)
    {
        if(constructionJob.ModuleStep == 1)
        {
            constructionJob.Target.SetActive(false);
            navMeshManager.UpdateNavMesh();
        }
        else
        {
            //TODO: partly disassemble
        }
    }

    public void FinishDeconstructionJob(Villager villager)
    {
        if (!finishedAssigning)
        {
            FinishAnyConstructionJob(villager);
        }
    }

    private IEnumerator FinishDeconstruction()
    {
        yield return new WaitUntil(() => assignedConstructionVillagers.Count == 0);
        for (int i = assignedIdleConstructionVillagers.Count - 1; i >= 0; i--)
        {
            jobsManager.UnassignVillager(assignedIdleConstructionVillagers[i], true);
            assignedIdleConstructionVillagers.RemoveAt(i);
        }
        yield return new WaitUntil(() => inventory.IsEmpty() == true);
        //Debug.Log("construction finished");
        LogisticsManager.GetInstance().RemoveInventory(this);
        jobsManager.RemoveConstructionSite(this);
        GameObject.Destroy(this.gameObject);
    }
    #endregion
}