using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamager : MonoBehaviour
{
    AnimatorManager animatorManager;
    [SerializeField] float curDamage;
    [SerializeField] float staminaDamage;
    [SerializeField] float radius;
    [SerializeField] float existDuration=3f;
    [SerializeField] bool isHeavy;
    bool exploed;
    [SerializeField] AudioClip fireTexpAudio;
    private void Start()
    {
        animatorManager = FindObjectOfType<AnimatorManager>();
        Destroy(gameObject,existDuration);
    }

    private void Update()
    {
        DamagerTrigger();
    }

    void DamagerTrigger() 
    {
        if (!exploed) 
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].tag == "DestructibleObject")
                    {
                        DestructibleObject destructibleObject = colliders[i].GetComponent<DestructibleObject>();

                        if (destructibleObject != null)
                        {
                            destructibleObject.ObjectDestroy();
                        }
                    }
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

                            enemyStats.TakeDamage(curDamage, staminaDamage, isHeavy, hitDirection);
                            exploed = true;
                        }
                        if (playerStats != null)
                        {
                            Vector3 hitDirection = transform.position - playerStats.transform.position;
                            hitDirection.y = 0;
                            hitDirection.Normalize();
                            exploed = true;

                            //playerStats.TakeDamage(curDamage, hitDirection);
                        }
                    }
                    animatorManager.generalAudio.clip = fireTexpAudio;
                    animatorManager.generalAudio.Play();
                }
            }
        }
    }
}
