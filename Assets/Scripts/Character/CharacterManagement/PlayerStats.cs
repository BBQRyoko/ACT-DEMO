using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    PlayerManager playerManager;
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    PlayerAttacker playerAttacker;

    AnimatorManager animatorManager;

    public float currStamina;
    [SerializeField] float maxStamina = 100;
    [SerializeField] float staminaRegen = 5;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        playerAttacker = GetComponent<PlayerAttacker>();
    }
    private void Start()
    {
        currHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }
    public void TakeDamage(int damage, Vector3 collisionDirection, bool isBoss) 
    {
        float viewableAngle = Vector3.SignedAngle(collisionDirection, playerManager.transform.forward, Vector3.up);
        currHealth = currHealth - damage;
        healthBar.SetCurrentHealth(currHealth);

        if (currHealth <= 0)
        {
            currHealth = 0;
            animatorManager.PlayTargetAnimation("Dead", true, true);
            playerManager.isDead = true;
        }
        else 
        {
            //Direction
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
            
            //临时添加, 受到伤害直接打断攻击状态
            if (isBoss) 
            {
                playerManager.GetComponent<Rigidbody>().AddForce(-collisionDirection, ForceMode.Impulse); //到时候要根据情况调整击退的力度
            }
            playerManager.isAttacking = false;
            animatorManager.animator.SetBool("isCharging", false);
            playerAttacker.chargingLevel = 0;
            playerAttacker.chargingTimer = 0;
        }
        
        //攻击被打断时保证取消状态
        playerManager.cantBeInterrupted = false;
        playerManager.weaponEquiping(true);
    }
    public void CostStamina(float cost) 
    {
        currStamina = currStamina - cost;
        staminaBar.SetCurrentStamina(currStamina);
    }

    public void StaminaRegen() 
    {
        if (!playerManager.isInteracting && !playerManager.isSprinting &&currStamina < maxStamina) 
        {
            currStamina = currStamina + staminaRegen * Time.deltaTime;
        }
        staminaBar.SetCurrentStamina(currStamina);
    }
}
