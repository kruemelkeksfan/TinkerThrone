using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class LogisticJob : IComparable<LogisticJob>
{
    public Inventory sourceInventory;
    public Inventory targetInventory;
    public Stack stack;
    public float priority;
    public Villager villager { get; set; }

    public LogisticJob(Inventory sourceInventory, Inventory tragetInventory, string goodName, uint amount, float priority)
    {
        this.sourceInventory = sourceInventory;
        this.targetInventory = tragetInventory;
        this.stack = new Stack(goodName, amount);
        this.priority = priority;
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

    public int CompareTo(LogisticJob other)
    {
        return priority.CompareTo(other.priority);
    }
}

