using UnityEngine;

public class ArenaWallController : MonoBehaviour
{
    public GameObject wallParent;

    void Start()
    {
        wallParent.SetActive(false);

        ArenaWaveManager.Instance.OnWaveStarted += CloseArena;
        ArenaWaveManager.Instance.OnWaveEnded += OpenArena;
    }

    void CloseArena()
    {
        Debug.Log("WALL ACTIVE");
        wallParent.SetActive(true);
    }

    void OpenArena()
    {
        Debug.Log("WALL REMOVED");
        wallParent.SetActive(false);
    }
}
