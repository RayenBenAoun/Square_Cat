using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public bool stunned = false;
    private float stunTimer = 0f;

    public void Stun(float time)
    {
        stunned = true;
        stunTimer = time;
        Debug.Log($"Enemy stunned for {time} seconds");
    }

    private void Update()
    {
        if (stunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
                stunned = false;
        }
    }
}
