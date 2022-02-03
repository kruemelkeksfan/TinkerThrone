using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LogisticCommision
{
    Inventory sourceInventory;
    Good good;
    int amount;
    int priority;

    public LogisticCommision(Inventory sourceInventory, Good good, int amount, int priority)
    {
        this.sourceInventory = sourceInventory;
        this.good = good;
        this.amount = amount;
        this.priority = priority;
    }
}

