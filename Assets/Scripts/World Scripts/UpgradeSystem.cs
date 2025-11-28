using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    private Dictionary<string, int> unlockedLevels = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        // Initialize all trees with 0 unlocked
        unlockedLevels["Projectile"] = 0;
        unlockedLevels["Draw"] = 0;
        unlockedLevels["Speed"] = 0;
    }

    public bool CanUnlockNode(string group, int nodeIndex)
    {
        return unlockedLevels[group] >= nodeIndex;
    }

    public bool IsNodeUnlocked(string group, int nodeIndex)
    {
        return unlockedLevels[group] > nodeIndex;
    }

    public void UnlockNode(string group, int nodeIndex)
    {
        unlockedLevels[group] = Mathf.Max(unlockedLevels[group], nodeIndex + 1);
        Debug.Log($"Unlocked level {nodeIndex} in {group} tree. Now level {unlockedLevels[group]}");
    }
}
