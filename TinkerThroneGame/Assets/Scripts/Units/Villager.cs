using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Unit
{
    private void Start()
    {
        InitializeInventory();
    }

    public IEnumerator DoLogisticJob(LogisticJob logisticJob)
    {
        JobsManager jobsManager = JobsManager.GetInstance();
        logisticJob.ReserveStack();
        UpdateGoal(logisticJob.sourceInventory.transform.position);
        Debug.Log("updated goal");
        yield return new WaitUntil(() => HasGoal() == false);
        Debug.Log("arrived at goal");
        yield return new WaitForSeconds(0.5f * logisticJob.stack.amount);
        if (!(logisticJob.sourceInventory.GetInventory().Withdraw(logisticJob.stack) && inventory.DirectDeposit(logisticJob.stack)))
        {
            Debug.Log("pickup failed");
            jobsManager.LogisticVillagerBusyToIdle(this, logisticJob, false);
            this.StopCoroutine(DoLogisticJob(logisticJob));
        }
        UpdateGoal(logisticJob.targetInventory.transform.position);
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * logisticJob.stack.amount);
        if (!(inventory.DirectWithdraw(logisticJob.stack) && logisticJob.targetInventory.GetInventory().Deposit(logisticJob.stack)))
        {
            jobsManager.LogisticVillagerBusyToIdle(this, logisticJob, false);
        }
        jobsManager.LogisticVillagerBusyToIdle(this, logisticJob);
    }
}
