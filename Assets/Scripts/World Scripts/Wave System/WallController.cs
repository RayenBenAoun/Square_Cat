using UnityEngine;

public class ArenaWallController : MonoBehaviour
{
    public GameObject wallParent;

    void Start()
    {
        wallParent.SetActive(false);

        ArenaWaveManager.Instance.OnWaveStarted += () =>
        {
            Debug.Log("WALL ACTIVE");
            wallParent.SetActive(true);
        };

        ArenaWaveManager.Instance.OnWaveEnded += () =>
        {
            Debug.Log("WALL REMOVED");
            wallParent.SetActive(false);
        };
    }
}
