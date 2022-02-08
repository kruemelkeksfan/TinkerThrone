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

    public float PriorityForHigherAmount(uint currentAmount)
    {
        float amountRatio = 1 - ((float)targetAmount / currentAmount);
        int priorityDif = logisticsPriorityBeeingFull - logisticsPriorityBeeingEmpty;
        return logisticsPriorityBeeingEmpty + priorityDif * amountRatio;
    }
    public float PriorityForLowerAmount(uint currentAmount)
    {
        float amountRatio = 1 - ((float)currentAmount / targetAmount);
        int priorityDif = logisticsPriorityBeeingFull - logisticsPriorityBeeingEmpty;
        return logisticsPriorityBeeingEmpty + priorityDif * amountRatio;
    }
}

