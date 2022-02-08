using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


struct LogisticJob
{
    Inventory sourceInventory;
    Inventory tragetInventory;
    string goodName;
    uint amount;
    float priority;

    public LogisticJob(Inventory sourceInventory, Inventory tragetInventory, string goodName, uint amount, float priority)
    {
        this.sourceInventory = sourceInventory;
        this.tragetInventory = tragetInventory;
        this.goodName = goodName;
        this.amount = amount;
        this.priority = priority;
    }
}

