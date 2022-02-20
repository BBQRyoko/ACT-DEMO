using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public bool isHeavy;
    [SerializeField] bool isFlyingObject;
    [SerializeField] bool isPlayerDamage;
    public EnemyManager enemyManager;
    public int curDamage = 10;
    [SerializeField] float hitFactor;

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayerDamage)
        {
            Vector3 hitDirection = new Vector3(0, 0, 0);
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            ParryCollider parryCollider = other.GetComponent<ParryCollider>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(curDamage, hitDirection * hitFactor, isHeavy);
                if (isFlyingObject)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
            else if (parryCollider != null)
            {
                if (parryCollider.isPerfect)
                {
                    Debug.Log("完美");
                }
                else
                {
                    Debug.Log("普通");
                }
            }
        }
        else 
        {
            Vector3 hitDirection = new Vector3(0, 0, 0);
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            if (enemyStats != null) 
            {
                enemyStats.TakeDamage(curDamage, hitDirection * hitFactor);
                if (isFlyingObject)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
    }
}