//using UnityEngine;

//public class EnemyMovement : MonoBehaviour
//{
//    public float moveSpeed = 2f;
//    private Transform player;
//    private EnemyAI ai;
//    private EnemyHealth health;

//    void Start()
//    {
//        player = GameObject.FindGameObjectWithTag("Player").transform;
//        ai = GetComponent<EnemyAI>();
//        health = GetComponent<EnemyHealth>();
//    }
//    void Update()
//    {
//        if (health.IsCurrentlyDowned()) return; 
//        if (ai != null && ai.stunned) return;

//        if (player != null)
//        {
//            Vector3 dir = (player.position - transform.position).normalized;
//            transform.position += dir * moveSpeed * Time.deltaTime;
//        }
//    }

//}
