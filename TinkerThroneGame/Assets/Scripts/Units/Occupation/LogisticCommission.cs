using System;

public struct LogisticCommission : IComparable<LogisticCommission>
{
    public readonly LogisticsUser sourceInventory;
    public readonly string goodName;
    public float Priority { get; private set; }
    public uint StoreableAmount { get; private set; }
    public uint TakeableAmount { get; private set; }

    private readonly Capacity maxCapacity;
    private readonly LogisticValue logisticValue;

    public LogisticCommission(LogisticsUser sourceInventory, uint storeableAmount, uint takeableAmount, LogisticValue logisticValue, Capacity maxCapacity)
    {
        this.sourceInventory = sourceInventory;
        this.goodName = logisticValue.goodName;
        this.StoreableAmount = storeableAmount;
        this.TakeableAmount = takeableAmount;
        this.logisticValue = logisticValue;
        this.maxCapacity = maxCapacity;
        Priority = logisticValue.GetPriority(takeableAmount, maxCapacity);
    }

    public void TakeAmount(uint amount)
    {
        TakeableAmount -= amount;
        Priority = logisticValue.GetPriority(TakeableAmount, maxCapacity);
    }

    public void StoreAmount(uint amount)
    {
        StoreableAmount -= amount;
        TakeableAmount += amount;
        Priority = logisticValue.GetPriority(TakeableAmount, maxCapacity);
    }

    public int CompareTo(LogisticCommission other)
    {
        return Priority.CompareTo(other.Priority);
    }
}