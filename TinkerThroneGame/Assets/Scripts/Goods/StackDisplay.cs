using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public struct StackDisplay
{
    public string goodName { get; private set; }
    public uint amount { get; private set; }
    public int amountChange { get; private set; }

    public StackDisplay(string goodName, uint amount, int amountChange)
    {
        this.goodName = goodName;
        this.amount = amount;
        this.amountChange = amountChange;
    }

}

