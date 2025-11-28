using UnityEngine;

public class StunWall : MonoBehaviour
{
    public float stunTime = 1.5f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        var enemy = other.collider.GetComponentInParent<EnemyAI>();
        if (enemy != null)
        {
            Debug.Log("Enemy stunned!");
            enemy.Stun(stunTime);
        }
    }
}
