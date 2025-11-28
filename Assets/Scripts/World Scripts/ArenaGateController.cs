using Cainos.PixelArtTopDown_Village;
using UnityEngine;

public class ArenaGateController : MonoBehaviour
{
    public Door doorScript;
    public bool opened = false;

    private void Start()
    {
        ArenaWaveManager.OnWaveCompleted += OpenGate;
        doorScript.Close(); // stays closed during waves
    }

    void OpenGate()
    {
        opened = true;
        doorScript.Open();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened && other.CompareTag("Player"))
        {
            opened = false;
            doorScript.Close();

            FindObjectOfType<ArenaWaveManager>().StartWave();
        }
    }
}
