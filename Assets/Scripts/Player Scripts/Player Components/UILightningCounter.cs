using UnityEngine;
using TMPro;

public class UILightningCounter : MonoBehaviour
{
    public TMP_Text counterText;

    public void UpdateCounter(int amount)
    {
        counterText.text = "⚡ " + amount.ToString();
    }
}
