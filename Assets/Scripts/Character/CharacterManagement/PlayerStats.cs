using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    InputManager inputManager;
    PlayerManager playerManager;
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    PlayerAttacker playerAttacker;

    AnimatorManager animatorManager;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
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

    private void Update()
    {
        healthBar.SetCurrentHealth(currHealth);
    }

    public void TakeDamage(int damage, Vector3 collisionDirection, bool isBoss, int forceType = 0) 
    {
        float viewableAngle = Vector3.SignedAngle(collisionDirection, playerManager.transform.forward, Vector3.up);
        currHealth = currHealth - damage;
        healthBar.SetCurrentHealth(currHealth);
        if (inputManager.spAttack_Input) 
        {
            inputManager.spAttack_Input = false;
            playerManager.isCharging = false;
        }

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
                if (forceType == 0)
                {
                    animatorManager.PlayTargetAnimation("Hit_F", true, true);
                }
                else if (forceType == 2) 
                {
                    animatorManager.PlayTargetAnimation("Hit_Large", true, true);
                }
            }
            else if (viewableAngle <= 90 && viewableAngle > 0)
            {
                if (forceType == 0)
                {
                    animatorManager.PlayTargetAnimation("Hit_F", true, true);
                }
                else if (forceType == 2)
                {
                    animatorManager.PlayTargetAnimation("Hit_Large", true, true);
                }
            }
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
        if (!playerManager.isInteracting && !playerManager.isSprinting && currStamina < maxStamina)
        {
            currStamina = currStamina + staminaRegen * Time.deltaTime;
        }
        staminaBar.SetCurrentStamina(currStamina);
    }
}
