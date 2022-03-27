using TMPro;
using UnityEngine;

public class GoodDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goodNameText;
    [SerializeField] private TextMeshProUGUI goodAmountText;

    public void DisplayInformation(StackDisplay stack)
    {
        goodNameText.text = stack.GoodName;
        if (stack.AmountChange > 0)
        {
            goodAmountText.text = stack.Amount + " / " + stack.TargetAmount + " (+" + stack.AmountChange + ")";
        }
        else
        {
            goodAmountText.text = stack.Amount + " / " + stack.TargetAmount + " (" + stack.AmountChange + ")";
        }
    }
}