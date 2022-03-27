﻿using System;

public class LogisticJob : IComparable<LogisticJob>
{
    public LogisticsUser SourceInventory { get; private set; }
    public LogisticsUser TargetInventory { get; private set; }
    public float Priority { get; private set; }
    private Stack stack;

    public Stack Stack { get { return stack; } }

    public LogisticJob(LogisticsUser sourceInventory, LogisticsUser tragetInventory, string goodName, uint amount, float priority)
    {
        this.SourceInventory = sourceInventory;
        this.TargetInventory = tragetInventory;
        this.stack = new Stack(goodName, amount);
        this.Priority = priority;
    }

    public LogisticJob(LogisticCommission inCommission, LogisticCommission outCommission, uint amount)
    {
        this.SourceInventory = outCommission.sourceInventory;
        this.TargetInventory = inCommission.sourceInventory;
        this.stack = new Stack(outCommission.goodName, amount);
        this.Priority = outCommission.Priority - inCommission.Priority;
    }

    public bool ReserveStack()
    {
        if (SourceInventory.GetInventory().ReserveWithdraw(Stack))
        {
            if (TargetInventory.GetInventory().ReserveDeposit(Stack))
            {
                return true;
            }
            else
            {
                SourceInventory.GetInventory().CancleReserveWithdraw(Stack);
            }
        }
        return false;
    }

    public void RevertReserveStack()
    {
        SourceInventory.GetInventory().CancleReserveWithdraw(Stack);
        TargetInventory.GetInventory().CancleReserveDeposit(Stack);
    }

    public int CompareTo(LogisticJob other)
    {
        return Priority.CompareTo(other.Priority);
    }
}