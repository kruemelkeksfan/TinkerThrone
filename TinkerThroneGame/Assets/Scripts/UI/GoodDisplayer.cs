using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoodDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goodNameText;
    [SerializeField] TextMeshProUGUI goodAmountText;

    public void DisplayInformation(StackDisplay stack)
    {
        goodNameText.text = stack.goodName;
        if(stack.amountChange > 0)
        {
            goodAmountText.text = stack.amount + " (+" + stack.amountChange + ")";
        }
        else
        {
            goodAmountText.text = stack.amount + " (" + stack.amountChange + ")";
        }
    }
}
