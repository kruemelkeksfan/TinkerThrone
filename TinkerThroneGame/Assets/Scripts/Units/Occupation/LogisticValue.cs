using System;

[Serializable]
public struct LogisticValue
{
    public string goodName;
    public int logisticsPriorityBeeingEmpty;
    public int logisticsPriorityBeeingFull;
    public uint targetAmount;

    public LogisticValue(string goodName, int logisticsPriorityBeeingEmpty, int logisticsPriorityBeeingFull, uint targetAmount)
    {
        this.goodName = goodName;
        this.logisticsPriorityBeeingEmpty = logisticsPriorityBeeingEmpty;
        this.logisticsPriorityBeeingFull = logisticsPriorityBeeingFull;
        this.targetAmount = targetAmount;
    }

    public float PriorityForHigherAmount(uint currentAmount, Capacity maxCapacity)
    {
        uint maxAmount = maxCapacity.ToAmount(goodName);
        if (maxAmount == 0) maxAmount = 10000; //preventing infinit priority with infinit capacity
        if(maxAmount == targetAmount)
        {
            maxAmount++; //set 1 higher to prevent /0
        }
        float amountRatio = (float)(currentAmount - targetAmount) / (maxAmount - targetAmount);
        int priorityDif = logisticsPriorityBeeingFull - logisticsPriorityBeeingEmpty;
        return logisticsPriorityBeeingEmpty + priorityDif * amountRatio;
    }

    public float PriorityForLowerAmount(uint currentAmount)
    {
        float amountRatio = (float)currentAmount / targetAmount;
        int priorityDif = logisticsPriorityBeeingFull - logisticsPriorityBeeingEmpty;
        return logisticsPriorityBeeingEmpty + priorityDif * amountRatio;
    }
}