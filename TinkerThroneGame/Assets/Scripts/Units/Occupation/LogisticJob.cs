﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class LogisticJob : IComparable<LogisticJob>
{
    public LogisticsUser sourceInventory;
    public LogisticsUser targetInventory;
    public Stack stack;
    public float priority;
    public Villager villager { get; set; }

    public LogisticJob(LogisticsUser sourceInventory, LogisticsUser tragetInventory, string goodName, uint amount, float priority)
    {
        this.sourceInventory = sourceInventory;
        this.targetInventory = tragetInventory;
        this.stack = new Stack(goodName, amount);
        this.priority = priority;
    }
    public LogisticJob(LogisticsUser sourceInventory, LogisticsUser tragetInventory, string goodName, uint amount, float priority, Villager villager)
    {
        this.sourceInventory = sourceInventory;
        this.targetInventory = tragetInventory;
        this.stack = new Stack(goodName, amount);
        this.priority = priority;
        this.villager = villager;
    }

    public LogisticJob(LogisticCommission inCommission, LogisticCommission outCommission)
    {
        this.sourceInventory = outCommission.sourceInventory;
        this.targetInventory = inCommission.sourceInventory;
        if (inCommission.amount > outCommission.amount)
        {
            this.stack = new Stack(outCommission.goodName, inCommission.amount);
        }
        else
        {
            this.stack = new Stack(outCommission.goodName, outCommission.amount);
        }
        this.priority = inCommission.priority - outCommission.priority;
    }

    public LogisticJob GetJobPart(Villager assignedVillager, out bool completedAssignment)
    {
        uint carryCapacity = assignedVillager.GetInventory().GetFreeCapacity().ToAmount(stack.goodName);
        if(carryCapacity < stack.amount)
        {
            completedAssignment = false;
            return new LogisticJob(sourceInventory, targetInventory, stack.goodName, carryCapacity, priority, assignedVillager);
        }
        else
        {
            villager = assignedVillager;
            completedAssignment = true;
            return this;
        }
    }

    public void ReserveStack()
    {
        sourceInventory.GetInventory().ReserveWithdraw(stack);
        targetInventory.GetInventory().ReserveDeposit(stack);
    }


    public int CompareTo(LogisticJob other)
    {
        return priority.CompareTo(other.priority);
    }
}
