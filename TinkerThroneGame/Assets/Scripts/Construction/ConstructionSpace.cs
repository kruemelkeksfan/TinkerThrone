public class ConstructionSpace : BuildingSpace
{
    public bool IsBlocked { get; private set; }

    private void Update()
    {
        int colorEscelation = 0;
        IsBlocked = false;
        for (int i = colliders.Count - 1; i >= 0; i--)
        {
            if (colliders[i] == null)
            {
                colliders.Remove(colliders[i]);
                continue;
            }
            UpgradeSpace upgradeSpace = colliders[i].gameObject.GetComponent<UpgradeSpace>();
            if (colorEscelation < 2 && upgradeSpace)
            {
                colorEscelation = 2;
            }
            else if (colorEscelation < 3 && !upgradeSpace)
            {
                colorEscelation = 3;
                IsBlocked = true;
                break;
            }
        }
        meshRenderer.material.SetColor("_EmissionColor", WorldConsts.BUILDING_SPACE_COLORS[colorEscelation]);
    }
}