using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    InputManager inputManager;
    PlayerManager playerManager;
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public 
    PlayerAttacker playerAttacker;
    AnimatorManager animatorManager;
    WeaponSlotManager weaponSlotManager;

    [SerializeField] Transform regularPos;
    [SerializeField] Transform crouchPos;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
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
        HandleEyePos();
    }
    void HandleEyePos() 
    {
        if (!playerManager.isCrouching)
        {
            eyePos = regularPos;
        }
        else 
        {
            eyePos = crouchPos;
        }
    }
    public void TakeDamage(int damage, Vector3 collisionDirection, bool isHeavy = false)
    {
        float viewableAngle = Vector3.SignedAngle(collisionDirection, playerManager.transform.forward, Vector3.up);
        currHealth = currHealth - damage;
        healthBar.SetCurrentHealth(currHealth);
        playerAttacker.comboCount = 0;

        if (currHealth <= 0)
        {
            currHealth = 0;
            animatorManager.PlayTargetAnimation("Dead", true, true);
            playerManager.isDead = true;
        }
        else
        {
            //Direction
            if ((viewableAngle >= 91 && viewableAngle <= 180) || (viewableAngle <= -91 && viewableAngle >= -180))
            {
                if (!isHeavy)
                {
                    if (!playerManager.isImmuAttack)
                    {
                        animatorManager.animator.speed = 1;
                        if (playerManager.isGettingDamage)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_B");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
                        else 
                        {
                            animatorManager.PlayTargetAnimation("Hit_B", true, true);
                        }
                        playerAttacker.comboCount = 0;
                        playerManager.isImmuAttack = false;
                        playerManager.cantBeInterrupted = false;
                    }
                }
                else 
                {
                    Vector3 direction = collisionDirection;
                    direction.y = 0;
                    direction.Normalize();

                    if (direction == Vector3.zero)
                    {
                        direction = transform.forward;
                    }

                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f / Time.deltaTime);

                    animatorManager.PlayTargetAnimation("Hit_Large", true, true);
                }
            }
            else if ((viewableAngle >= -90 && viewableAngle <= 0) || (viewableAngle <= 90 && viewableAngle > 0))
            {
                if (!isHeavy)
                {
                    if (!playerManager.isImmuAttack) 
                    {
                        animatorManager.animator.speed = 1;
                        if (playerManager.isGettingDamage)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_F");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
                        else
                        {
                            animatorManager.PlayTargetAnimation("Hit_F", true, true);
                        }
                        playerAttacker.comboCount = 0;
                        playerManager.isImmuAttack = false;
                        playerManager.cantBeInterrupted = false;
                    }
                }
                else
                {
                    Vector3 direction = collisionDirection;
                    direction.y = 0;
                    direction.Normalize();

                    if (direction == Vector3.zero)
                    {
                        direction = transform.forward;
                    }

                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f / Time.deltaTime);

                    animatorManager.PlayTargetAnimation("Hit_Large", true, true);
                }
            }
        }
        
        //攻击被打断时保证取消状态
        playerManager.cantBeInterrupted = false;
    }
    public void CostStamina(float cost) 
    {
        currStamina = currStamina - cost;
        staminaBar.SetCurrentStamina(currStamina);
    }
    public void StaminaRegen()
    {
        if (!playerManager.cantBeInterrupted && !playerManager.isSprinting && !playerManager.staminaRegenPause && currStamina < maxStamina)
        {
            if (playerManager.isDefending)
            {
                currStamina = currStamina + staminaRegen / 3 * Time.deltaTime;
            }
            else 
            {
                currStamina = currStamina + staminaRegen * Time.deltaTime;
            }
        }

        staminaBar.SetCurrentStamina(currStamina);
    }
}
