using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency Instance;

    [Header("Player Currency")]
    [SerializeField] public int lightningBolts = 0; // editable in inspector

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
