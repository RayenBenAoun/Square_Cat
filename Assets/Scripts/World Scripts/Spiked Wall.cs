using System.Collections.Generic;
using UnityEngine;

public class SpikedWall : MonoBehaviour
{
    public GameObject spikePrefab;
    private List<GameObject> spikes = new List<GameObject>();

    public EnemyColor wallColor = EnemyColor.None;

    private const float spikeScale = 0.05f;
    private const float spacing = 0.8f;
    private const int maxSpikes = 20;

    public void SpawnSpikesAlongEdge(List<Vector2> pts, EdgeCollider2D wallCol)
    {
        spikes.Clear();
        float dist = 0f;

        for (int i = 0; i < pts.Count - 1; i++)
        {
            if (spikes.Count >= maxSpikes)
                break;

            Vector2 p1 = pts[i];
            Vector2 p2 = pts[i + 1];

            float segDist = Vector2.Distance(p1, p2);
            dist += segDist;
            if (dist < spacing) continue;
            dist = 0f;

            Vector2 spawnPos = (p1 + p2) / 2f;

            GameObject spike = Instantiate(spikePrefab, spawnPos, Quaternion.identity, transform);
            spike.transform.localScale = Vector3.one * spikeScale;

            Rigidbody2D rb = spike.GetComponent<Rigidbody2D>();
            if (rb == null) rb = spike.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;

            Collider2D col = spike.GetComponent<Collider2D>();
            if (col == null) col = spike.AddComponent<BoxCollider2D>();

            Vector2 normal = new Vector2(-(p2.y - p1.y), p2.x - p1.x).normalized;
            spike.transform.up = normal;
            spike.transform.position += (Vector3)normal * 0.13f;

            SpikeDamage sd = spike.GetComponent<SpikeDamage>();
            sd.AttachToWall(wallCol, normal);
            sd.SetSpikeColor(wallColor);

            spikes.Add(spike);
        }
    }

    public void SetWallColor(EnemyColor color)
    {
        wallColor = color;

        foreach (var spike in spikes)
            spike.GetComponent<SpikeDamage>().SetSpikeColor(color);
    }

    public void ShootAllSpikes()
    {
        foreach (var spike in spikes)
        {
            SpikeDamage sd = spike.GetComponent<SpikeDamage>();
            sd.Launch();
        }
        spikes.Clear();
    }
}
