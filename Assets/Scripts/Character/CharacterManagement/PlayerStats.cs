using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    InputManager inputManager;
    PlayerManager playerManager;
    PlayerInventory playerInventory;
    public HealthBar healthBar;
    public StaminaBar staminaBar;
    PlayerAttacker playerAttacker;
    AnimatorManager animatorManager;
    WeaponSlotManager weaponSlotManager;

    [Header("数值buff类参数")]
    public float attackBuffRatio = 0;
    public float defendBuffRatio = 0;
    public float movementBuffRatio = 0;

    [SerializeField] Transform regularPos;
    [SerializeField] Transform crouchPos;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
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
        if (currHealth <= 0 && !playerManager.isDead)
        {
            currHealth = 0;
            animatorManager.animator.SetTrigger("isDead");
            playerManager.isDead = true;
        }
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
    public void TakeDamage(float damage, Vector3 collisionDirection, bool isHeavy = false)
    {
        float damageAngle = Vector3.SignedAngle(collisionDirection, playerManager.transform.forward, Vector3.up);
        if (playerManager.isImmuAttack) damage = damage * 0.65f;
        currHealth = currHealth - damage;
        healthBar.SetCurrentHealth(currHealth);
        playerAttacker.comboCount = 0;
        weaponSlotManager.UnloadArrowOnSlot();
        playerManager.isHanging = false;
        playerManager.isHolding = false;
        playerManager.isDefending = false;
        inputManager.weaponAbility_Input = false;
        animatorManager.animator.ResetTrigger("isHoldingCancel");

        if (currHealth <= 0)
        {
            currHealth = 0;
            animatorManager.animator.SetTrigger("isDead");
            playerManager.isDead = true;
        }
        else
        {
            //Direction
            if ((damageAngle > 120 && damageAngle <= 180) || (damageAngle < -120 && damageAngle >= -180) )
            {
                if (!isHeavy)
                {
                    if (!playerManager.isImmuAttack && !playerManager.isGetingExecuted)
                    {
                        animatorManager.animator.speed = 1;
                        if (!playerManager.isImmuAttack)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_B");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
  
                        playerAttacker.comboCount = 0;
                        playerManager.isImmuAttack = false;
                        playerManager.cantBeInterrupted = false;
                        weaponSlotManager.weaponDamageCollider.DisableDamageCollider();
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
            else if ((damageAngle > -60 && damageAngle <= 0) || (damageAngle < 60 && damageAngle > 0))
            {
                if (!isHeavy)
                {
                    if (!playerManager.isImmuAttack && !playerManager.isGetingExecuted) 
                    {
                        animatorManager.animator.speed = 1;
                        if (!playerManager.isImmuAttack)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_F");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }

                        playerAttacker.comboCount = 0;
                        playerManager.isImmuAttack = false;
                        playerManager.cantBeInterrupted = false;
                        weaponSlotManager.weaponDamageCollider.DisableDamageCollider();
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
            else if (damageAngle >= 60 && damageAngle <= 120)
            {
                if (!isHeavy)
                {
                    if (!playerManager.isImmuAttack && !playerManager.isGetingExecuted)
                    {
                        animatorManager.animator.speed = 1;
                        if (!playerManager.isImmuAttack)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_R");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }

                        playerAttacker.comboCount = 0;
                        playerManager.isImmuAttack = false;
                        playerManager.cantBeInterrupted = false;
                        weaponSlotManager.weaponDamageCollider.DisableDamageCollider();
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
            } //右
            else if (damageAngle >= -120 && damageAngle <= -60)
            {
                if (!isHeavy)
                {
                    if (!playerManager.isImmuAttack && !playerManager.isGetingExecuted)
                    {
                        animatorManager.animator.speed = 1;
                        if (!playerManager.isImmuAttack)
                        {
                            animatorManager.animator.SetTrigger("beingAttacked_L");
                            animatorManager.animator.SetBool("isInteracting", true);
                            animatorManager.animator.SetBool("isUsingRootMotion", true);
                        }
                        playerAttacker.comboCount = 0;
                        playerManager.cantBeInterrupted = false;
                        weaponSlotManager.weaponDamageCollider.DisableDamageCollider();
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
            } //左
        }
        //攻击被打断时保证取消状态
        playerManager.cantBeInterrupted = false;
    }
    public void CostStamina(float cost) 
    {
        currStamina = currStamina - cost;
        staminaBar.SetCurrentStamina(currStamina);
    }
    public void StaminaController()
    {
        if (!playerManager.cantBeInterrupted && !playerManager.isSprinting && !playerManager.isAttacking && !playerManager.staminaRegenPause && currStamina < maxStamina && !playerManager.isHolding)//正常状态
        {
            currStamina += staminaRegen * Time.deltaTime;
        }
        else if (playerManager.isHolding && playerManager.isDefending && !playerManager.staminaRegenPause) //防御
        {
            currStamina += (staminaRegen * 0.25f )* Time.deltaTime;
        }
        else if (playerManager.isHolding && !playerManager.isDefending) //弓箭
        {
            currStamina -= playerInventory.curEquippedWeaponItem.holdingStaminaCost * Time.deltaTime;
            if (currStamina <= 0) animatorManager.animator.SetTrigger("isHoldingCancel");
        }

        staminaBar.SetCurrentStamina(currStamina);
    }
}
