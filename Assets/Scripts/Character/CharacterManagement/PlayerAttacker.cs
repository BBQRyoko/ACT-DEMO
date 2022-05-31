using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    CameraManager cameraManager;
    InputManager inputManager;
    PlayerManager playerManager;
    PlayerStats playerStats;
    PlayerLocmotion playerLocmotion;
    PlayerInventory playerInventory;
    AnimatorManager animatorManager;
    WeaponSlotManager weaponSlotManager;
    PlayerUIManager playerUIManager;
    [SerializeField] Sample_VFX sample_VFX;
    [SerializeField] Sample_SFX sample_SFX;
    [SerializeField] AudioSource generalAudio;

    public Sample_VFX sample_VFX_R;
    public Sample_VFX sample_VFX_S;

    //处决
    public Collider[] colliders;
    public EnemyManager executionTarget;
    [SerializeField] Vector3 executionOffset;

    //普通攻击
    public int comboCount;
    public float attackTimer;
    public float internalDuration = 3.75f;

    //弓箭相关
    public bool isUsingPowerArrow;
    [SerializeField] float powerArrowRatio = 1.5f;

    //太极切换触发
    public float chargeValue; //switchValue

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        inputManager = GetComponent<InputManager>();
        playerManager = GetComponent<PlayerManager>();
        playerLocmotion = GetComponent<PlayerLocmotion>();
        playerStats = GetComponent<PlayerStats>();
        playerInventory = GetComponent<PlayerInventory>();
        playerUIManager = GetComponent<PlayerUIManager>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }
    private void Update()
    {
        AttackComboTimer();
        HandleAttackCharge();
        HoldingController();
        ExecutionHandler();
    }
    public void HandleRegularAttack(WeaponItem weapon) //左键普攻
    {
        //使用指定武器信息中的普通攻击
        if (!playerManager.cantBeInterrupted && playerManager.isGround && !playerManager.isGettingDamage && !playerManager.isHanging && !playerManager.isClimbing)
        {
            playerLocmotion.HandleRotateTowardsTarger();

            //可处决
            if (executionTarget) //无消耗
            {
                playerManager.isCrouching = false;
                animatorManager.animator.SetBool("cantBeInterrupted", true);
                animatorManager.animator.SetBool("isAttacking", true);
                attackTimer = internalDuration;
                if (!executionTarget.isStunned) //背刺
                {
                    playerManager.transform.position = executionTarget.execute_Back.position;
                    playerLocmotion.HandleRotateTowardsTarger();
                    animatorManager.PlayTargetAnimation(weapon.executionSkill[0].skillName, true, true); //背刺
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.executionSkill[0].damagePoint * (1 + playerStats.attackBuffRatio);
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.executionSkill[0].tenacityDamagePoint;
                    animatorManager.pauseDuration = weapon.executionSkill[0].pauseDuration;
                    executionTarget.getingExecute = true;
                    executionTarget.HandleExecuted(weapon.executionSkill[1].skillName);
                }
                else //常规处决
                {
                    playerManager.transform.position = executionTarget.execute_Front.position;
                    playerLocmotion.HandleRotateTowardsTarger();
                    animatorManager.PlayTargetAnimation(weapon.executionSkill[2].skillName, true, true); //处决
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.executionSkill[2].damagePoint * (1 + playerStats.attackBuffRatio); 
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.executionSkill[2].tenacityDamagePoint;
                    animatorManager.pauseDuration = weapon.executionSkill[2].pauseDuration;
                    executionTarget.getingExecute = true;
                    executionTarget.HandleExecuted(weapon.executionSkill[3].skillName);
                }
                animatorManager.generalAudio.volume = 0.1f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.ExecutionSFX;
                animatorManager.generalAudio.Play();
                executionTarget = null;
            }
            //普通攻击
            else
            {
                playerManager.isCrouching = false;
                if (playerManager.isSprinting)
                {
                    comboCount = 0;
                    comboCount++;
                    if (playerManager.GetComponent<PlayerStats>().currStamina >= weapon.springAttack[0].staminaCost - 15f && !playerManager.cantBeInterrupted)
                    {
                        playerManager.cantBeInterrupted = true;
                        animatorManager.animator.SetBool("isAttacking", true);
                        attackTimer = internalDuration;
                        //播放指定的攻击动画
                        animatorManager.PlayTargetAnimation(weapon.springAttack[0].skillName, true, true);
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.springAttack[0].damagePoint * (1 + playerStats.attackBuffRatio);
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.springAttack[0].tenacityDamagePoint;
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.springAttack[0].energyRestore;
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().chargeAmount = weapon.springAttack[0].energyRestore;
                        animatorManager.pauseDuration = weapon.springAttack[0].pauseDuration;
                        playerManager.GetComponent<PlayerStats>().currStamina -= weapon.springAttack[0].staminaCost;
                        playerManager.isImmuAttack = weapon.springAttack[0].isImmuAttack;
                    }
                }
                else
                {
                    comboCount++;
                    if (comboCount > weapon.regularSkills.Length)
                    {
                        comboCount = 1;
                    }
                    //检测是否有足够的体力释放
                    if (playerManager.GetComponent<PlayerStats>().currStamina >= weapon.regularSkills[comboCount - 1].staminaCost - 10f && !playerManager.cantBeInterrupted && !playerManager.isHolding)
                    {
                        playerManager.cantBeInterrupted = true;
                        animatorManager.animator.SetBool("isAttacking", true);
                        attackTimer = internalDuration;
                        //播放指定的攻击动画
                        animatorManager.PlayTargetAnimation(weapon.regularSkills[comboCount - 1].skillName, true, true);
                        if (playerInventory.curEquippedWeaponItem.Id == 2) //使用弓箭时的状态
                        {
                            ProjectileDamager projectileDamager = weaponSlotManager.curArrowObj.GetComponentInChildren<ProjectileDamager>();

                            if (!isUsingPowerArrow)
                            {
                                projectileDamager.curDamage = weapon.regularSkills[comboCount - 1].damagePoint * (1 + playerStats.attackBuffRatio);
                                projectileDamager.staminaDamage = weapon.regularSkills[comboCount - 1].tenacityDamagePoint;
                                projectileDamager.energyRestoreAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                                projectileDamager.chargeAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                                weaponSlotManager.curArrowObj.m_MaxSpeed = weapon.regularSkills[comboCount - 1].maxSpeed;
                            }
                            else 
                            {
                                projectileDamager.curDamage = weapon.regularSkills[comboCount - 1].damagePoint * (1 + playerStats.attackBuffRatio) * powerArrowRatio;
                                projectileDamager.staminaDamage = weapon.regularSkills[comboCount - 1].tenacityDamagePoint;
                                projectileDamager.energyRestoreAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                                projectileDamager.chargeAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                                weaponSlotManager.curArrowObj.m_MaxSpeed = weapon.regularSkills[comboCount - 1].maxSpeed;
                            }
                        }
                        else
                        {
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.regularSkills[comboCount - 1].damagePoint * (1 + playerStats.attackBuffRatio);
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.regularSkills[comboCount - 1].tenacityDamagePoint;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().chargeAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                        }
                        animatorManager.pauseDuration = weapon.regularSkills[comboCount - 1].pauseDuration;
                        playerManager.GetComponent<PlayerStats>().currStamina -= weapon.regularSkills[comboCount - 1].staminaCost;
                        if (weapon.regularSkills[comboCount - 1].isImmuAttack)
                        {
                            playerManager.isImmuAttack = true;
                        }
                        else
                        {
                            playerManager.isImmuAttack = false;
                        }
                    }
                    else
                    {
                        comboCount--;
                    }
                }
            }
        }
    }
    public void HandleSpecialAttack(WeaponItem weapon) //右键特殊攻击
    {
        playerLocmotion.HandleRotateTowardsTarger();
        if (!playerManager.cantBeInterrupted && playerManager.isGround && !playerManager.isGettingDamage)
        {
            playerManager.cantBeInterrupted = true;
            animatorManager.animator.SetBool("isAttacking", true);
            attackTimer = internalDuration;
            if (comboCount == 0)
            {
                //右键的第一下就是普通的第一下
                animatorManager.PlayTargetAnimation(weapon.regularSkills[comboCount].skillName, true, true);
                comboCount++;
            }
            else
            {
                ////其余都播放特殊攻击的动作
                animatorManager.PlayTargetAnimation(weapon.specialSkills[comboCount - 1].skillName, true, true);
                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.regularSkills[comboCount - 1].damagePoint * (1 + playerStats.attackBuffRatio);
                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().chargeAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                playerManager.GetComponent<PlayerStats>().currStamina -= weapon.regularSkills[comboCount - 1].staminaCost;
                //sample_VFX_S.curVFX_List[comboCount - 1].Play();
                comboCount = 0;
            }
        }
        //rig.velocity = new Vector3(0, rig.velocity.y, 0);
    } //重攻击
    public void HandleWeaponAbility(WeaponItem weapon)
    {
        if (playerManager.isInteracting || !playerManager.isGround || playerManager.isHanging || playerManager.isClimbing) return;
        if (playerInventory.curEquippedWeaponItem.Id == 0) //大剑
        {
            HandleHoldingAbility();
            animatorManager.animator.SetBool("isDefending", playerManager.isHolding);
        }
        else if (playerInventory.curEquippedWeaponItem.Id == 1) //太刀
        {
            HandleHoldingAbility();
            animatorManager.animator.SetBool("isDefending", playerManager.isHolding);
        }
        else if (playerInventory.curEquippedWeaponItem.Id == 2) //弓
        {
            //开镜功能
            if (inputManager.weaponAbility_Input)
            {
                playerManager.isAiming = true;
                playerUIManager.aimingCorsshair.SetActive(true);
            }
            else 
            {
                playerManager.isAiming = false;
                playerUIManager.aimingCorsshair.SetActive(false);
                cameraManager.ResetAimingCameraRotation();
            }

        }
    }
    public void HandleWeaponAbilityCancel(WeaponItem weapon)
    {
        if (playerInventory.curEquippedWeaponItem.Id == 0) //大剑
        {
            if (playerManager.isHolding)
            {
                animatorManager.animator.SetTrigger("isHoldingCancel");
            }
        }
        else if (playerInventory.curEquippedWeaponItem.Id == 1) //太刀
        {
            if (playerManager.isHolding)
            {
                animatorManager.animator.SetTrigger("isHoldingCancel");
            }
        }
        else if (playerInventory.curEquippedWeaponItem.Id == 2) //弓
        {
            //开镜功能
            if (inputManager.weaponAbility_Input)
            {
                playerManager.isAiming = true;
                playerUIManager.aimingCorsshair.SetActive(true);
            }
            else
            {
                playerManager.isAiming = false;
                playerUIManager.aimingCorsshair.SetActive(false);
                cameraManager.ResetAimingCameraRotation();
            }

        }
    }
    public void HandleHoldingAbility() 
    {
        if (!playerManager.cantBeInterrupted && playerManager.isGround && !playerManager.isHolding && !playerManager.isHanging && !playerManager.isClimbing)
        {
            animatorManager.PlayTargetAnimation("HoldingAbility", true, true);
        }
    }
    public void HandleTransformAttack(WeaponItem weapon) 
    {
        //使用指定武器信息中的普通攻击
        if (!playerManager.cantBeInterrupted && playerManager.isGround)
        {
            playerLocmotion.HandleRotateTowardsTarger();
            comboCount = 0;
            animatorManager.animator.SetBool("isAttacking", true);
            playerManager.cantBeInterrupted = true;
            playerManager.isWeaponSwitching = false;
            attackTimer = internalDuration;
            //播放指定的攻击动画
            animatorManager.PlayTargetAnimation(weapon.transSkills[0].skillName, true, true);
            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.transSkills[0].damagePoint * (1 + playerStats.attackBuffRatio);
            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.transSkills[0].tenacityDamagePoint;
            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.transSkills[0].energyRestore;
            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().chargeAmount = weapon.transSkills[0].energyRestore;
            animatorManager.pauseDuration = weapon.transSkills[0].pauseDuration;
            playerManager.GetComponent<PlayerStats>().currStamina -= weapon.transSkills[0].staminaCost;
        }
    }
    public void HandleDefend(WeaponItem weapon) //武器防御
    {
        if (!playerManager.cantBeInterrupted && playerManager.isGround && !playerManager.isDefending)
        {
            animatorManager.PlayTargetAnimation("Defend", true, true);
        }
    }
    void AttackComboTimer() 
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            attackTimer = 0;
            comboCount = 0;
        }
    }
    void HoldingController() 
    {
        //if (playerInventory.curEquippedWeaponItem.Id == 2) //弓
        //{
        //    if (playerManager.isHolding)
        //    {

        //    }
        //    else
        //    {

        //    }
        //}
        //else 
        //{

        //}
    }
    void HandleAttackCharge() 
    {
        if (chargeValue >= 5) 
        {
            generalAudio.clip = sample_SFX.Bagua_SFX_List[4];
            generalAudio.Play();
            playerManager.transAttackTimer = 1.75f;
            playerManager.canTransAttack = true;
            chargeValue = 0;
            sample_VFX.baGuaRelated_List[0].Play();
        }
    }
    void ExecutionHandler() 
    {
        if (executionTarget && !executionTarget.isDead && executionTarget.canBeExecuted)
        {
            playerManager.cameraManager.curExuectionTarget = executionTarget.transform;
        }
        else 
        {
            executionTarget = null;
            playerManager.cameraManager.curExuectionTarget = null;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + executionOffset, 4f);
    }
}
