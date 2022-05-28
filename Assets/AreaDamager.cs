using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamager : MonoBehaviour
{
    [SerializeField] float curDamage;
    [SerializeField] float staminaDamage;
    [SerializeField] float radius;
    [SerializeField] float existDuration=3f;

    private void Start()
    {
        Destroy(gameObject,existDuration);
    }

    private void Update()
    {
        DamagerTrigger();
    }

    void DamagerTrigger() 
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        if (colliders.Length > 0) 
        {
            for (int i = 0; i < colliders.Length; i++) 
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null)
                {
                    PlayerStats playerStats = character.GetComponent<PlayerStats>();
                    EnemyStats enemyStats = character.GetComponent<EnemyStats>();
                    if (enemyStats != null) 
                    {
                        Vector3 hitDirection = transform.position - enemyStats.transform.position;
                        hitDirection.y = 0;
                        hitDirection.Normalize();

                        enemyStats.TakeDamage(curDamage, staminaDamage, hitDirection);
                    }
                    if (playerStats != null) 
                    {
                        Vector3 hitDirection = transform.position - playerStats.transform.position;
                        hitDirection.y = 0;
                        hitDirection.Normalize();

                        //playerStats.TakeDamage(curDamage, hitDirection);
                    }
                }
            }
        }
    }
}
