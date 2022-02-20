using System.Collections;
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

        UpdateGoal(logisticJob.SourceInventory.transform.position);
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * logisticJob.Stack.amount);
        logisticJob.SourceInventory.GetInventory().Withdraw(logisticJob.Stack);
        inventory.DirectDeposit(logisticJob.Stack);

        UpdateGoal(logisticJob.TargetInventory.transform.position);
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * logisticJob.Stack.amount);
        inventory.DirectWithdraw(logisticJob.Stack);
        logisticJob.TargetInventory.GetInventory().Deposit(logisticJob.Stack);

        jobsManager.LogisticVillagerBusyToIdle(this);
    }
}