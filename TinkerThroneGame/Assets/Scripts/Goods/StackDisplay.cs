public struct StackDisplay
{
    public string GoodName { get; private set; }
    public uint Amount { get; private set; }
    public int AmountChange { get; private set; }

    public StackDisplay(string goodName, uint amount, int amountChange)
    {
        this.GoodName = goodName;
        this.Amount = amount;
        this.AmountChange = amountChange;
    }
}