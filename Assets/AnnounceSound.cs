using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnounceSound : MonoBehaviour
{
    public bool isExecutionCall;
    SphereCollider sphereCollider;
    [SerializeField]EnemyStats announceEnemy;
    public float announceSoundDistance;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        announceEnemy = GetComponentInParent<EnemyStats>();
    }
    private void Update()
    {
        if (sphereCollider.radius < announceSoundDistance)
        {
            sphereCollider.radius += 8f * Time.deltaTime;
        }
        else 
        {
            sphereCollider.radius = 0.5f;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) 
        {
            EnemyManager enemyManager = other.GetComponent<EnemyManager>();
            if (enemyManager != null )
            {
                IdleState idleState = enemyManager.GetComponentInChildren<IdleState>();
                idleState.AnnouncedByOtherEnemy(announceEnemy, isExecutionCall);
            }

        }
    }
}
