using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


struct LogisticJob
{
    Inventory sourceInventory;
    Inventory tragetInventory;
    Good good;
    int amount;
    int priority;

    public LogisticJob(Inventory sourceInventory, Inventory tragetInventory, Good good, int amount, int priority)
    {
        this.sourceInventory = sourceInventory;
        this.tragetInventory = tragetInventory;
        this.good = good;
        this.amount = amount;
        this.priority = priority;
    }
}

