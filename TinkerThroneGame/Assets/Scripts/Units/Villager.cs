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

        UpdateGoal(logisticJob.SourceInventory.GetLogisticPosition());
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * logisticJob.Stack.amount);
        logisticJob.SourceInventory.GetInventory().Withdraw(logisticJob.Stack);
        inventory.DirectDeposit(logisticJob.Stack);

        UpdateGoal(logisticJob.TargetInventory.GetLogisticPosition());
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * logisticJob.Stack.amount);
        inventory.DirectWithdraw(logisticJob.Stack);
        logisticJob.TargetInventory.GetInventory().Deposit(logisticJob.Stack);

        jobsManager.LogisticVillagerBusyToIdle(this);
    }

    public IEnumerator DoConstructionJob(ConstructionJob constructionJob)
    {
        UpdateGoal(constructionJob.ConstructionSite.GetLogisticPosition());
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * constructionJob.Stack.amount);
        constructionJob.ConstructionSite.GetInventory().Withdraw(constructionJob.Stack);
        inventory.DirectDeposit(constructionJob.Stack);

        UpdateGoal(constructionJob.Target.transform.position);
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * constructionJob.Stack.amount); // TODO add constructionTime
        inventory.DirectWithdraw(constructionJob.Stack);

        constructionJob.ConstructionSite.FinishConstructionJob(constructionJob, this);
    }
}