using System;

[Serializable]
public struct LogisticValue
{
    public string goodName;
    public int logisticsPriorityBeeingEmpty;
    public int logisticsPriorityBeeingFull;
    public int targetAmount;

    public LogisticValue(string goodName, int logisticsPriorityBeeingEmpty, int logisticsPriorityBeeingFull, int targetAmount)
    {
        this.goodName = goodName;
        this.logisticsPriorityBeeingEmpty = logisticsPriorityBeeingEmpty;
        this.logisticsPriorityBeeingFull = logisticsPriorityBeeingFull;
        this.targetAmount = targetAmount;
    }
}

