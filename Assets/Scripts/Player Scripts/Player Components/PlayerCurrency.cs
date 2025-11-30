using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency Instance;

    [Header("Collected Lightning Bolts")]
    public int lightningBolts = 0;

    private void Awake()
    {
        Instance = this;
    }

    public bool Spend(int amount)
    {
        if (lightningBolts >= amount)
        {
            lightningBolts -= amount;
            return true;
        }
        return false;
    }

    public void AddBolts(int amount)
    {
        lightningBolts += amount;
    }
}
