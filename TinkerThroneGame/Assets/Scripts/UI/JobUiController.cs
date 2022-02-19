using TMPro;
using UnityEngine;

public class JobUiController : MonoBehaviour
{
    static JobUiController instance;

    [SerializeField] TextMeshProUGUI idleAmountText;
    [SerializeField] TextMeshProUGUI logisticAmountText;
 
    public static JobUiController GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public void UpdateUi(int idleCount, int neededIdleCount, int logisticCount, int neededLogisticCount)
    {
        idleAmountText.text = idleCount + " / " + neededIdleCount;
        logisticAmountText.text = logisticCount + " / " + neededLogisticCount;
    }
}