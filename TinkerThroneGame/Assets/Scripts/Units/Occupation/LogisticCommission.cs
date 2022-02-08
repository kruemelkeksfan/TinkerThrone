using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public struct LogisticCommission : IComparable<LogisticCommission>
{
    Inventory sourceInventory;
    string goodName;
    uint amount;
    public float priority;

    public LogisticCommission(Inventory sourceInventory, string goodName, uint amount, float priority)
    {
        this.sourceInventory = sourceInventory;
        this.goodName = goodName;
        this.amount = amount;
        this.priority = priority;
    }

    public int CompareTo(LogisticCommission other)
    {
        return priority.CompareTo(other.priority);
    }
}

