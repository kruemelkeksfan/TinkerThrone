public struct StackDisplay
{
    public string GoodName { get; private set; }
    public uint Amount { get; private set; }
    public int AmountChange { get; private set; }

    public uint TargetAmount { get; private set; }

    public StackDisplay(string goodName, uint amount, int amountChange, uint targetAmount)
    {
        this.GoodName = goodName;
        this.Amount = amount;
        this.AmountChange = amountChange;
        this.TargetAmount = targetAmount;
    }
}