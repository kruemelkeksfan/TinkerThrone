using TMPro;
using UnityEngine;

public class JobUiController : MonoBehaviour
{
    private static JobUiController instance;

    [SerializeField] private TextMeshProUGUI idleAmountText;
    [SerializeField] private TextMeshProUGUI logisticAmountText;
    [SerializeField] private TextMeshProUGUI constructionAmountText;

    public static JobUiController GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public void UpdateUi(int idleCount, int logisticCount, int neededLogisticCount, int constructionCount, int neededConstructionCount)
    {
        idleAmountText.text = idleCount.ToString();
        logisticAmountText.text = logisticCount + " / " + neededLogisticCount;
        constructionAmountText.text = constructionCount + " / " + neededConstructionCount;
    }
}