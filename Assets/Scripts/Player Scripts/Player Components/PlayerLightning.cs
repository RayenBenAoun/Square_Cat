using UnityEngine;
using TMPro;

public class PlayerLightning : MonoBehaviour
{
    public int bolts = 9999;  // LOTS for testing

    [Header("UI")]
    public TextMeshProUGUI boltText;

    void Start()
    {
        UpdateUI();
    }

    public void AddBolts(int amount)
    {
        bolts += amount;
        UpdateUI();
    }

    public bool SpendBolts(int amount)
    {
        if (bolts < amount)
            return false;

        bolts -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (boltText != null)
            boltText.text = $"⚡ {bolts}";
    }
}
