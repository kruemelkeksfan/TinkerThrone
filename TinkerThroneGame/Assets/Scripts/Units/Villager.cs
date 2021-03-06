using System.Collections;
using UnityEngine;

public class Villager : Unit
{
    public bool HasJob { get; private set; }

    private void Start()
    {
        InitializeInventory();
    }

    public IEnumerator DoLogisticJob(LogisticJob logisticJob)
    {
        HasJob = true;
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
        HasJob = false;
        jobsManager.LogisticVillagerBusyToIdle(this);
    }

    public IEnumerator DoConstructionJob(ConstructionJob constructionJob)
    {
        HasJob = true;
        UpdateGoal(constructionJob.ConstructionSite.GetLogisticPosition());
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * constructionJob.Stack.amount);
        constructionJob.ConstructionSite.GetInventory().Withdraw(constructionJob.Stack);
        constructionJob.ConstructionSite.ReduceTargetAmount(constructionJob.Stack);
        inventory.DirectDeposit(constructionJob.Stack);

        UpdateGoal(constructionJob.Target.transform.position);
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * constructionJob.Stack.amount); // TODO add constructionTime
        inventory.DirectWithdraw(constructionJob.Stack);

        HasJob = false;
        constructionJob.ConstructionSite.FinishConstructionJob(constructionJob, this);
    }

    public IEnumerator DoDeconstructionJob(ConstructionJob constructionJob)
    {
        HasJob = true;
        UpdateGoal(constructionJob.Target.transform.position);
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * constructionJob.Stack.amount); // TODO add constructionTime
        constructionJob.ConstructionSite.DeconstructModule(constructionJob);
        inventory.DirectDeposit(constructionJob.Stack);

        UpdateGoal(constructionJob.ConstructionSite.GetLogisticPosition());
        yield return new WaitUntil(() => HasGoal() == false);
        yield return new WaitForSeconds(0.5f * constructionJob.Stack.amount);
        constructionJob.ConstructionSite.GetInventory().Deposit(constructionJob.Stack);
        inventory.DirectWithdraw(constructionJob.Stack);

        HasJob = false;
        constructionJob.ConstructionSite.FinishDeconstructionJob(this);
    }

    public void Move(Vector3 position)
    {
        UpdateGoal(position);
    }
}