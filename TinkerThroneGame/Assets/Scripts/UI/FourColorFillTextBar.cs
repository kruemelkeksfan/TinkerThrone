using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FourColorFillTextBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image FillImage1;
    [SerializeField] private Image FillImage2;
    [SerializeField] private Image FillImage3;

    public void UpdateBar(float fillAmount1, float fillAmount2, float fillAmount3, string text)
    {
        this.text.text = text;
        FillImage1.fillAmount = fillAmount1;
        FillImage2.fillAmount = fillAmount2;
        FillImage3.fillAmount = fillAmount3;
    }
}