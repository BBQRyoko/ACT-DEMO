using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExecutionAreaTrigger : MonoBehaviour
{
    EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = GetComponentInParent<EnemyManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            enemyManager.canExecute = true;
            playerManager.stunTimer = -10f;
            playerManager.isStunned = true;
        }
    }
}
