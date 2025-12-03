using UnityEngine;
using TMPro;

public class UIWaveCounter : MonoBehaviour
{
    public TextMeshProUGUI text;

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    void UpdateDisplay(int wave)
    {
        text.text = "Wave: " + wave;
    }
}
