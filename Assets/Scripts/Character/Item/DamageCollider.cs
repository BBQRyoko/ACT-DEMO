using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public EnemyManager enemyManager;
    public PlayerManager playerManager;
    Collider damageCollider;

    int curDamage = 10;

    public int duration;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    private void Start()
    {
        enemyManager = GetComponentInParent<EnemyManager>();
    }
    public void EnableDamageCollider() 
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider() 
    {
        damageCollider.enabled = false;
        playerManager.isHitting = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            Vector3 hitDirection = transform.position - playerManager.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            PlayerStats playerStats = collision.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (!playerStats.GetComponent<PlayerManager>().damageAvoid && !playerStats.GetComponent<PlayerManager>().isPerfect)
                {
                    playerStats.TakeDamage(curDamage, hitDirection, true);
                }
                else if (playerStats.GetComponent<PlayerManager>().isPerfect) 
                {
                    enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimation("GetHit_Up", true, true);
                    playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("WeaponAbility_01(Success)", true, true);
                    playerManager.PerfectBlock();
                }
            }
        }
        else if (collision.tag == "Enemy")
        {
            Vector3 hitDirection = transform.position - collision.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            if (enemyStats != null && enemyStats.currHealth != 0 && !enemyStats.GetComponent<EnemyManager>().isDodging)
            {
                enemyStats.TakeDamage(curDamage, hitDirection, playerManager.GetComponent<PlayerStats>());
                HitPause(duration);
                playerManager.isHitting = true;
            }
        }
        else if (collision.tag == "DestructibleObject")
        {
            DestructibleObject destructibleObject = collision.GetComponent<DestructibleObject>();

            if (destructibleObject != null)
            {
                destructibleObject.ObjectDestroy();
            }
        }
        else if (collision.tag == "Parry") 
        {
            ParryCollider parryCollider = collision.GetComponent<ParryCollider>();

            if (parryCollider != null) 
            {
                if (parryCollider.isPerfect)
                {
                    enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimation("GetHit_Up", true, true);
                    playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("WeaponAbility_01(Success)", true, true);
                    playerManager.PerfectBlock();
                }
                else
                {
                    playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("WeaponAbility_01(Broken)", true, true);
                }
                DisableDamageCollider();
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
                playerManager.isHitting = false;
            }
        }
    }

    public void HitPause(int dur) 
    {
        StartCoroutine(Hitted(dur));
    }

    IEnumerator Hitted(int dur) 
    {
        float pauseTime = dur / 60f;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1f;
    }
}
