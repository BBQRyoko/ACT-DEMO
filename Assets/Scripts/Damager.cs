using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public int damage = 10;
    [SerializeField] float hitFactor;

    private void OnTriggerEnter(Collider other)
    {
        if (enemyManager)
        {
            Vector3 hitDirection = enemyManager.enemyRig.transform.position - other.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (!playerStats.GetComponent<PlayerManager>().damageAvoid) 
                {
                    playerStats.TakeDamage(damage, hitDirection * hitFactor, true);
                    enemyManager.PlayHittedSound();
                }
            }
        }
        else
        {
            Vector3 hitDirection = new Vector3(0, 0, 0);

            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            ParryCollider parryCollider = other.GetComponent<ParryCollider>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(damage, hitDirection * hitFactor, true);
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
    }
}
