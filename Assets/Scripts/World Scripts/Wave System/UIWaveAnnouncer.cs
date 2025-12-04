using UnityEngine;
using TMPro;
using System.Collections;

public class UIWaveAnnouncer : MonoBehaviour
{
    public static UIWaveAnnouncer Instance;

    [SerializeField] TMP_Text announceText;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        ArenaWaveManager.Instance.OnWaveStarted += Announce;
    }

    void OnDisable()
    {
        if (ArenaWaveManager.Instance != null)
            ArenaWaveManager.Instance.OnWaveStarted -= Announce;
    }

    // MUST MATCH: Action → void Announce()
    void Announce()
    {
        StartCoroutine(ShowAnnouncement());
    }

    IEnumerator ShowAnnouncement()
    {
        announceText.text = "Wave " + ArenaWaveManager.Instance.currentWave + "!";
        announceText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        announceText.gameObject.SetActive(false);
    }
}
