using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI boltText;

    void Update()
    {
        boltText.text = PlayerCurrency.Instance.lightningBolts.ToString();
    }
}
