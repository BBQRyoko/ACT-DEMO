using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    EnemyManager enemyManager;
    Animator animator;
    IdleState idleState;
    EnemyAnimatorManager animatorManager;
    EnemyWeaponSlotManager enemyWeaponSlotManager;

    //Boss 血条
    [SerializeField] HealthBar healthBar;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        animator = GetComponentInChildren<Animator>();
        animatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyWeaponSlotManager = GetComponentInChildren<EnemyWeaponSlotManager>();
    }
    private void Start()
    {
        currHealth = maxHealth;
        currStamina = maxStamina;
        if (healthBar) 
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    private void Update()
    {
        StaminaRegen();
    }
    public void TakeDamage(int damage, Vector3 collisionDir, CharacterStats characterStats = null)
    {
        float viewableAngle = Vector3.SignedAngle(collisionDir, enemyManager.transform.forward, Vector3.up);
        currHealth = currHealth - damage;
        if (healthBar) 
        {
            healthBar.SetCurrentHealth(currHealth);
        }

        if (currHealth <= 0)
        {
            currHealth = 0;
            if (!enemyManager.getingExecute) 
            {
                animatorManager.PlayTargetAnimation("Dead", true);
            }
            enemyManager.isDead = true;
        }
        else
        {
            if (!enemyManager.isImmuneAttacking && !enemyManager.getingExecute)
            {
                if (viewableAngle >= 91 && viewableAngle <= 180)
                {
                    animatorManager.PlayTargetAnimation("Hit_B", true, true);
                }
                else if (viewableAngle <= -91 && viewableAngle >= -180)
                {
                    animatorManager.PlayTargetAnimation("Hit_B", true, true);
                }
                else if (viewableAngle >= -90 && viewableAngle <= 0)
                {
                    animatorManager.PlayTargetAnimation("Hit_F", true, true);
                }
                else if (viewableAngle <= 90 && viewableAngle > 0)
                {
                    animatorManager.PlayTargetAnimation("Hit_F", true, true);
                }
                enemyManager.isDamaged = true;
                if (enemyManager.isWeak) 
                {
                    enemyManager.GetComponentInChildren<EnemyAnimatorManager>().animator.SetBool("isWeak", false);
                    enemyManager.weakTimer = 0;
                }
            }

            if (!enemyManager.isEquipped) 
            {
                enemyWeaponSlotManager.WeaponEquip();
            }
            enemyManager.getingExecute = false;
            enemyManager.curTarget = characterStats;
        }
    }

    public void StaminaRegen()
    {
        if (!enemyManager.isInteracting && currStamina < maxStamina)
        {
            currStamina = currStamina + staminaRegen * Time.deltaTime;
        }
    }
}
