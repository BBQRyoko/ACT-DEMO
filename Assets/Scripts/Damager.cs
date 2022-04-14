using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public bool isHeavy;
    public bool stunEffect;
    [SerializeField] bool isFlyingObject;
    [SerializeField] bool isPlayerDamage;
    public EnemyManager enemyManager;
    public int curDamage = 10;
    public int staminaDamage;
    [SerializeField] float hitFactor;
    PlayerManager playerManager;
    [SerializeField] bool cantBlock;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 && !cantBlock) 
        {
            Destroy(gameObject);
        }
        if (!isPlayerDamage)
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            ParryCollider parryCollider = other.GetComponent<ParryCollider>();
            if (playerStats != null)
            {
                Vector3 hitDirection = transform.position - playerStats.transform.position;
                hitDirection.y = 0;
                hitDirection.Normalize();

                playerStats.TakeDamage(curDamage, hitDirection * hitFactor, isHeavy);

                if (stunEffect) 
                {
                    playerStats.GetComponentInChildren<Animator>().SetTrigger("isStun");
                }

                if (isFlyingObject)
                {
                    if (transform.parent)
                    {
                        Destroy(transform.parent.gameObject);
                    }
                    else 
                    {
                        Destroy(transform.gameObject);
                    }
                }
            }
            else if (parryCollider != null)
            {
                PlayerManager playerManager = parryCollider.GetComponentInParent<PlayerManager>();
                AudioSource attackAudioSource = playerManager.GetComponentInChildren<AudioSource>();
                Sample_SFX sample_SFX_Source = playerManager.GetComponentInChildren<Sample_SFX>();
                attackAudioSource.volume = 0.2f;
                int i = sample_SFX_Source.blockedSFX_List.Length;
                int random = Random.Range(0, i - 1);
                attackAudioSource.clip = sample_SFX_Source.blockedSFX_List[random];
                attackAudioSource.Play();
                playerManager.HandleParryingCheck(curDamage);
                Destroy(this.gameObject);
            }
        }
        else 
        {
            Vector3 hitDirection = new Vector3(0, 0, 0);
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            AnimatorManager animatorManager = playerStats.GetComponentInChildren<AnimatorManager>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(curDamage,staminaDamage, hitDirection * hitFactor);
                enemyStats.GetComponent<EnemyManager>().curTarget = playerStats;
                animatorManager.generalAudio.volume = 0.1f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[3];
                animatorManager.generalAudio.Play();
                if (isFlyingObject)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
            else
            {
                if (other.CompareTag("FireBlocker")) 
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }
}