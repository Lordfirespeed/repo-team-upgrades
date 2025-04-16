using UnityEngine;

namespace TeamUpgrades;

public class TeamUpgradesManager : MonoBehaviour
{
    void Awake()
    {
        transform.parent = StatsManager.instance.transform;
        gameObject.name = "TeamUpgradesManager";
        gameObject.hideFlags &= ~HideFlags.HideAndDontSave;
        Plugin.Logger.LogInfo("I was alive (however briefly)");
    }
}
