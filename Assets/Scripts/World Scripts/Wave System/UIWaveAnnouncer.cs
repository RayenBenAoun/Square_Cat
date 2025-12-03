using UnityEngine;
using TMPro;
using System.Collections;

public class UIWaveText : MonoBehaviour
{
    public static UIWaveText Instance;

    public TextMeshProUGUI waveText;

    void Awake()
    {
        Instance = this;
        waveText.gameObject.SetActive(false);
    }

    public void ShowWaveNumber(int waveNum)
    {
        StartCoroutine(ShowWaveRoutine(waveNum));
    }

    IEnumerator ShowWaveRoutine(int waveNum)
    {
        waveText.text = $"WAVE {waveNum}";
        waveText.alpha = 0;
        waveText.gameObject.SetActive(true);

        // fade in
        for (float t = 0; t < 1; t += Time.deltaTime * 2)
        {
            waveText.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        // fade out
        for (float t = 1; t > 0; t -= Time.deltaTime * 2)
        {
            waveText.alpha = t;
            yield return null;
        }

        waveText.gameObject.SetActive(false);
    }
}
