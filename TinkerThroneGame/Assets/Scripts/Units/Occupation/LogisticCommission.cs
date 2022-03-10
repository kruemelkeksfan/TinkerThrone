using System;

public struct LogisticCommission : IComparable<LogisticCommission>
{
    public readonly LogisticsUser sourceInventory;
    public readonly string goodName;
    public readonly float priority;
    public uint StoreableAmount { get; private set; }
    public uint TakeableAmount { get; private set; }

    public void ReduceStoreableAmount(uint amount)
    {
        this.StoreableAmount -= amount;
    }
    public void ReduceTakeableAmount(uint amount)
    {
        this.TakeableAmount -= amount;
    }

    public LogisticCommission(LogisticsUser sourceInventory, string goodName, uint storeableAmount, uint takeableAmount, float priority)
    {
        this.sourceInventory = sourceInventory;
        this.goodName = goodName;
        this.StoreableAmount = storeableAmount;
        this.TakeableAmount = takeableAmount;
        this.priority = priority;
    }

    public int CompareTo(LogisticCommission other)
    {
        return priority.CompareTo(other.priority);
    }
}