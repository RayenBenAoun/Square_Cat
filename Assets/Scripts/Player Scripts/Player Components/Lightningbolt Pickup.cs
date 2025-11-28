using UnityEngine;

public class LightningBoltPickup : MonoBehaviour
{
    public int value = 1;  // how many bolts this pickup gives

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerLightning player = other.GetComponent<PlayerLightning>();

        if (player != null)
        {
            player.AddBolts(value);
            Destroy(gameObject);
        }
    }

}
