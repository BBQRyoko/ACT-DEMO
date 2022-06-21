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

    float hitGauge;
    //Boss 血条
    [SerializeField] HealthBar healthBar;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        enemyManager = GetComponent<EnemyManager>();
        animator = GetComponentInChildren<Animator>();
        animatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyWeaponSlotManager = GetComponentInChildren<EnemyWeaponSlotManager>();
        idleState = GetComponentInChildren<IdleState>();
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
    public void TakeDamage(float damage, float staminaDamage, bool isHeavy, Vector3 collisionDir, CharacterStats characterStats = null)
    {
        if (!enemyManager.isDodging)
        {
            float damageAngle = Vector3.SignedAngle(collisionDir, enemyManager.transform.forward, Vector3.up);
            currHealth = currHealth - damage;
            currStamina = currStamina - staminaDamage;
            hitGauge += staminaDamage;
            enemyManager.isDamaged = true;

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
                if (!enemyManager.getingExecute) animator.SetTrigger("isDead");
                currHealth = 0;
                enemyWeaponSlotManager.CloseWeaponDamageCollider();
                enemyManager.isDead = true;
            }
            else
            {
                if (currStamina > 0 && (hitGauge >= enemyManager.hitRatio * maxStamina || isHeavy == true))
                {
                    if (!enemyManager.isImmuneAttacking && !enemyManager.getingExecute && !enemyManager.isDodging)
                    {
                        if (enemyManager.isFinalBoss)
                        {
                            if (enemyManager.phaseChanged)
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
                                if (enemyWeaponSlotManager.weaponDamageCollider)
                                {
                                    enemyWeaponSlotManager.weaponDamageCollider.DisableDamageCollider();
                                }
                                hitGauge = 0;
                            }
                            else 
                            {
                                hitGauge = 0;
                            }
                        }
                        else 
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
                            if (enemyWeaponSlotManager.weaponDamageCollider)
                            {
                                enemyWeaponSlotManager.weaponDamageCollider.DisableDamageCollider();
                            }
                            hitGauge = 0;
                        }
                    }
                }
                else if(currStamina <= 0 && !enemyManager.phaseChangeProtect)
                {
                    if (enemyManager.isStunned)
                    {
                        if (enemyWeaponSlotManager.weaponDamageCollider) enemyWeaponSlotManager.weaponDamageCollider.DisableDamageCollider();
                        GetComponentInChildren<EnemyAnimatorManager>().animator.SetBool("isStunned", false);
                        enemyManager.stunTimer = 0;
                        currStamina = maxStamina;
                        hitGauge = 0;
                    }
                    else 
                    {
                        if (enemyWeaponSlotManager.weaponDamageCollider) enemyWeaponSlotManager.weaponDamageCollider.DisableDamageCollider();
                        animatorManager.PlayTargetAnimation("Hit_Large", true, true);
                        currStamina = 0;
                        hitGauge = 0;
                    }
                }


                if (enemyManager.canAlertOthers && !enemyManager.calledAlready) 
                {
                    idleState.PlayerNoticeAnnounce(idleState.announceDistance, false);
                }
                if (!enemyManager.isAlerting && !enemyManager.curTarget)
                {
                    enemyManager.isAlerting = true;
                    enemyManager.alertTimer = 5f;
                }
                else 
                {
                    enemyManager.alertTimer = 5f;
                }
                if (!enemyManager.isEquipped) 
                {
                    enemyManager.isEquipped = true;
                    enemyWeaponSlotManager.LoadWeaponOnSlot(enemyWeaponSlotManager.weaponItem);
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
            if (enemyWeaponSlotManager.weaponDamageCollider) enemyWeaponSlotManager.weaponDamageCollider.DisableDamageCollider();
        }
    }
    public void StaminaRegen()
    {
        if (!enemyManager.isInteracting && currStamina < maxStamina && !enemyManager.isDamaged)
        {
            if (!enemyManager.isParrying && !enemyManager.isDamaged && !enemyManager.isBlocking)
            {
                currStamina = currStamina + staminaRegen * Time.deltaTime;
            }
            else 
            {
                currStamina = currStamina + staminaRegen/2 * Time.deltaTime;
            }
        }

        if (!enemyManager.isInteracting && hitGauge >0 ) 
        {
            hitGauge -= Time.deltaTime * enemyManager.hitGaugeRecoveryRate;
        }
    }
}
