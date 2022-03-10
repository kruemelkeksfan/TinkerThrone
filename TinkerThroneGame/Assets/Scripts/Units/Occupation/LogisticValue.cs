using System;
using UnityEngine;

[Serializable]
public struct LogisticValue
{
    public string goodName;
    public int logisticsPriorityAtTargetAmount;
    public int logisticsPriorityFillAlteration;
    public uint targetAmount;
    public bool targetAmountIsMax;

    public LogisticValue(string goodName, int logisticsPriorityAtTargetAmount, int logisticsPriorityFillAlteration, uint targetAmount, bool targetAmountIsMax)
    {
        this.goodName = goodName;
        this.logisticsPriorityAtTargetAmount = logisticsPriorityAtTargetAmount;
        this.logisticsPriorityFillAlteration = logisticsPriorityFillAlteration;
        this.targetAmount = targetAmount;
        this.targetAmountIsMax = targetAmountIsMax;
    }

    public float GetPriority(uint currentAmount, Capacity maxCapacity)
    {
        float rawPriority;
        if (currentAmount > targetAmount)
        {
            uint maxAmount = maxCapacity.ToAmount(goodName);
            if (maxAmount == 0) maxAmount = 10000; //preventing infinit priority with infinit capacity
            if (maxAmount == targetAmount)
            {
                maxAmount++; //set 1 higher to prevent /0
            }
            float amountRatio = (float)(currentAmount - targetAmount) / (maxAmount - targetAmount);
            rawPriority = logisticsPriorityAtTargetAmount + logisticsPriorityFillAlteration * amountRatio;
        }
        else
        {
            float amountRatio = 1 - (float)currentAmount / targetAmount;
            rawPriority = logisticsPriorityAtTargetAmount - logisticsPriorityFillAlteration * amountRatio;
        }
        return Mathf.RoundToInt(rawPriority * WorldConsts.PRIORITY_SETP_SIZE_RECIPROCAL) * WorldConsts.PRIORITY_SETP_SIZE;
    }
}