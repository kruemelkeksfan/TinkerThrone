using System;

public struct LogisticCommission : IComparable<LogisticCommission>
{
    public readonly LogisticsUser sourceInventory;
    public readonly string goodName;
    public readonly float priority;
    public uint Amount { get; private set; }

    public void ReduceAmount(uint amount)
    {
        this.Amount -= amount;
    }

    public LogisticCommission(LogisticsUser sourceInventory, string goodName, uint amount, float priority)
    {
        this.sourceInventory = sourceInventory;
        this.goodName = goodName;
        this.Amount = amount;
        this.priority = priority;
    }

    public int CompareTo(LogisticCommission other)
    {
        return priority.CompareTo(other.priority);
    }
}