using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    CameraManager cameraManager;
    EnemyManager enemyManager;
    Animator animator;
    IdleState idleState;
    EnemyAnimatorManager animatorManager;
    EnemyWeaponSlotManager enemyWeaponSlotManager;

    //Boss 血条
    [SerializeField] HealthBar healthBar;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
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
    public void TakeDamage(float damage, float staminaDamage, Vector3 collisionDir, CharacterStats characterStats = null)
    {
        if (!enemyManager.isDodging) 
        {
            float damageAngle = Vector3.SignedAngle(collisionDir, enemyManager.transform.forward, Vector3.up);
            currHealth = currHealth - damage;
            currStamina = currStamina - staminaDamage;

            if (!enemyManager.healtBarSpawn)
            {
                cameraManager.GenerateUIBar(enemyManager);
                enemyManager.healtBarSpawn = true;
            }

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
                enemyWeaponSlotManager.CloseWeaponDamageCollider();
                enemyManager.isDead = true;
            }
            else
            {
                if (currStamina > 0)
                {
                    if (!enemyManager.isImmuneAttacking && !enemyManager.getingExecute && !enemyManager.isDodging)
                    {
                        if ((damageAngle > 120 && damageAngle <= 180) || (damageAngle < -120 && damageAngle >= -180))
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_B");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
                        else if ((damageAngle > -60 && damageAngle <= 0) || (damageAngle < 60 && damageAngle > 0))
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_F");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
                        else if (damageAngle >= 60 && damageAngle <= 120)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_L");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
                        else if (damageAngle >= -120 && damageAngle <= -60)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_R");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
                        if (enemyWeaponSlotManager.weaponDamageCollider) enemyWeaponSlotManager.weaponDamageCollider.DisableDamageCollider();
                        enemyManager.isDamaged = true;

                        //if (enemyManager.isStunned) //普通攻击会打醒敌人
                        //{
                        //    enemyManager.isStunned = false;
                        //    enemyManager.stunTimer = 0;
                        //}
                    }
                }
                else
                {
                    currStamina = 0;
                    animatorManager.PlayTargetAnimation("Hit_Large", true, true);
                }
                if (!enemyManager.isEquipped)
                {
                    enemyWeaponSlotManager.WeaponEquip();
                }
                enemyManager.curTarget = characterStats;
            }
        }
    }
    public void TakeStaminaDamage(float staminaDamage) 
    {
        currStamina = currStamina - staminaDamage;
        if (currStamina <= 0) 
        {
            animator.SetTrigger("isBreak");
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
