using UnityEngine;
using TMPro;

public class PlayerLightning : MonoBehaviour
{
    [Header("Lightning Bolt Stats")]
    public int bolts = 0;

    [Header("UI")]
    public TextMeshProUGUI boltText;  // MUST be assigned in Inspector

    void Start()
    {
        if (boltText == null)
        {
            // Try to auto-find it so you don't get null errors
            boltText = FindAnyObjectByType<TextMeshProUGUI>();
        }

        UpdateUI();
    }

    // Called when picking up a bolt
    public void AddBolts(int amount)
    {
        bolts += amount;

        Debug.Log($"⚡ PlayerLightning FOUND — Adding bolts! Total = {bolts}");

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (boltText != null)
        {
            boltText.text = $"⚡ {bolts}";
        }
        else
        {
            Debug.LogWarning("⚠ boltText is NOT assigned on PlayerLightning!");
        }
    }
}
