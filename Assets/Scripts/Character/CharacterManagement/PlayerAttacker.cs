using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    [SerializeField] AudioSource noticeAudio;


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

    //大剑相关
    public int gsChargeLevel;
    public float gsChargeSlot;
    [SerializeField] float gsEnhanceRatio = 1.75f;
    [SerializeField] GameObject gsEnhanceVFX;

    //弓箭相关
    public bool isUsingPowerArrow;
    [SerializeField] float powerArrowRatio = 2f;

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
        GreatSwordChargeController();
    }
    public void HandleRegularAttack(WeaponItem weapon) //左键普攻
    {
        //使用指定武器信息中的普通攻击
        if (!playerManager.cantBeInterrupted && playerManager.isGround && !playerManager.isGettingDamage && !playerManager.isHanging && !playerManager.isClimbing && !playerManager.isDefending && !playerManager.isStunned)
        {
            playerLocmotion.HandleRotateTowardsTarger();
            playerManager.GetComponent<BaGuaManager>().isSwitchAttack = false;
            //可处决
            if (executionTarget && playerInventory.curEquippedWeaponItem.Id != 2) //无消耗，弓箭无法触发
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
                    if (playerInventory.curEquippedWeaponItem.Id == 0 && gsChargeLevel >= 1) //当大剑有储能时
                    {
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.executionSkill[0].damagePoint * (1 + playerStats.attackBuffRatio) * gsEnhanceRatio;
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.executionSkill[0].energyRestore;
                    }
                    else 
                    {
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.executionSkill[0].damagePoint * (1 + playerStats.attackBuffRatio);
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.executionSkill[0].energyRestore;
                    }
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = 0;
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().hitChargeAmount = 0;
                    animatorManager.pauseDuration = weapon.executionSkill[0].pauseDuration;
                    executionTarget.curTarget = playerStats;
                    executionTarget.getingExecute = true;
                    executionTarget.HandleExecuted(weapon.executionSkill[1].skillName);
                    playerManager.isImmuAttack = true;
                }
                else //常规处决
                {
                    playerManager.transform.position = executionTarget.execute_Front.position;
                    playerLocmotion.HandleRotateTowardsTarger();
                    animatorManager.PlayTargetAnimation(weapon.executionSkill[2].skillName, true, true); //处决
                    if (playerInventory.curEquippedWeaponItem.Id == 0 && gsChargeLevel >= 1) //当大剑有储能时
                    {
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.executionSkill[2].damagePoint * (1 + playerStats.attackBuffRatio) * gsEnhanceRatio;
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.executionSkill[2].energyRestore;
                    }
                    else 
                    {
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.executionSkill[2].damagePoint * (1 + playerStats.attackBuffRatio);
                        weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.executionSkill[2].energyRestore;
                    }
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = 0;
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().hitChargeAmount = 0;
                    animatorManager.pauseDuration = weapon.executionSkill[2].pauseDuration;
                    executionTarget.getingExecute = true;
                    executionTarget.HandleExecuted(weapon.executionSkill[3].skillName);
                }
                animatorManager.generalAudio.volume = 0.1f;
                animatorManager.generalAudio.clip = animatorManager.sample_SFX.ExecutionSFX;
                animatorManager.generalAudio.Play();
                executionTarget = null;
                playerManager.isImmuAttack = true;
            }
            //普通攻击
            else
            {
                playerManager.isCrouching = false;
                if (playerManager.isSprinting && playerInventory.curEquippedWeaponItem.Id != 2)
                {
                    comboCount = 0;
                    comboCount++;
                    if (playerManager.GetComponent<PlayerStats>().currStamina >= weapon.springAttack[0].staminaCost - 10f && !playerManager.isAttacking)
                    {
                        playerManager.cantBeInterrupted = true;
                        animatorManager.animator.SetBool("isAttacking", true);
                        attackTimer = internalDuration;
                        //播放指定的攻击动画
                        animatorManager.PlayTargetAnimation(weapon.springAttack[0].skillName, true, true);
                        if (playerInventory.curEquippedWeaponItem.Id == 0 && gsChargeLevel >= 1) //当大剑有储能时
                        {
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.springAttack[0].damagePoint * (1 + playerStats.attackBuffRatio) * gsEnhanceRatio;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.springAttack[0].tenacityDamagePoint * (1 + playerStats.attackBuffRatio) * gsEnhanceRatio;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.springAttack[0].energyRestore;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().hitChargeAmount = weapon.springAttack[0].hitPoint;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().isEnhanced = true;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().isHeavyAttack = true;
                        }
                        else
                        {
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.springAttack[0].damagePoint * (1 + playerStats.attackBuffRatio);
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.springAttack[0].tenacityDamagePoint * (1 + playerStats.attackBuffRatio);
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.springAttack[0].energyRestore;
                            weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().hitChargeAmount = weapon.springAttack[0].hitPoint;
                        }
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
                            ProjectileDamager projectileDamager = weaponSlotManager.arrowObjs[0].GetComponentInChildren<ProjectileDamager>();
                            projectileDamager.curDamage = weapon.regularSkills[comboCount - 1].damagePoint * (1 + playerStats.attackBuffRatio);
                            projectileDamager.staminaDamage = weapon.regularSkills[comboCount - 1].tenacityDamagePoint * (1 + playerStats.attackBuffRatio);
                            projectileDamager.energyRestoreAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                            projectileDamager.hitChargeAmount = weapon.regularSkills[comboCount - 1].hitPoint;
                            weaponSlotManager.arrowObjs[0].m_MaxSpeed = weapon.regularSkills[comboCount - 1].maxSpeed;
                        }
                        else
                        {
                            if (playerInventory.curEquippedWeaponItem.Id == 0 && gsChargeLevel >= 1) //当大剑有储能时
                            {
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.regularSkills[comboCount - 1].damagePoint * (1 + playerStats.attackBuffRatio) * gsEnhanceRatio;
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.regularSkills[comboCount - 1].tenacityDamagePoint * (1 + playerStats.attackBuffRatio) * gsEnhanceRatio;
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().hitChargeAmount = weapon.regularSkills[comboCount - 1].hitPoint;
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().isEnhanced = true;
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().isHeavyAttack = true;
                            }
                            else 
                            {
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.regularSkills[comboCount - 1].damagePoint * (1 + playerStats.attackBuffRatio);
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.regularSkills[comboCount - 1].tenacityDamagePoint * (1 + playerStats.attackBuffRatio);
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.regularSkills[comboCount - 1].energyRestore;
                                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().hitChargeAmount = weapon.regularSkills[comboCount - 1].hitPoint;
                            }
                        }
                        animatorManager.pauseDuration = weapon.regularSkills[comboCount - 1].pauseDuration;
                        playerManager.GetComponent<PlayerStats>().currStamina -= weapon.regularSkills[comboCount - 1].staminaCost;
                        playerManager.isImmuAttack = weapon.regularSkills[comboCount - 1].isImmuAttack;
                    }
                    else
                    {
                        comboCount--;
                    }
                }
            }
        }
    }
    public void HandleWeaponAbility(WeaponItem weapon)
    {
        if (playerManager.isInteracting || !playerManager.isGround || playerManager.isHanging || playerManager.isClimbing || playerManager.isGettingDamage) return;
        if (playerInventory.curEquippedWeaponItem.Id == 0) //大剑
        {
            playerLocmotion.HandleRotateTowardsTarger();
            HandleHoldingAbility();
            animatorManager.animator.SetBool("isDefending", playerManager.isHolding);
        }
        else if (playerInventory.curEquippedWeaponItem.Id == 1) //太刀
        {
            playerLocmotion.HandleRotateTowardsTarger();
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
            animatorManager.animator.SetBool("cantBeInterrupted", true);
            playerManager.isWeaponSwitching = false;
            attackTimer = internalDuration*2;
            //播放指定的攻击动画
            playerManager.isImmuAttack = true;
            animatorManager.PlayTargetAnimation(weapon.transSkills[0].skillName, true, true);
            playerManager.GetComponent<BaGuaManager>().isSwitchAttack = true;
            if (weapon.Id == 2) //使用弓箭时的状态
            {
                ProjectileDamager projectileDamager = weaponSlotManager.arrowObjs[1].GetComponentInChildren<ProjectileDamager>(); //必为强击
                projectileDamager.curDamage = weapon.transSkills[0].damagePoint * (1 + playerStats.attackBuffRatio);
                projectileDamager.staminaDamage = weapon.transSkills[0].tenacityDamagePoint * (1 + playerStats.attackBuffRatio);
                projectileDamager.energyRestoreAmount = weapon.transSkills[0].energyRestore;
                projectileDamager.hitChargeAmount = weapon.transSkills[0].hitPoint;
                weaponSlotManager.arrowObjs[1].m_MaxSpeed = weapon.transSkills[0].maxSpeed;
            }
            else 
            {
                if (weapon.Id == 0) 
                {
                    gsChargeSlot += 50;
                    weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().isEnhanced = true;
                }
                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().curDamage = weapon.transSkills[0].damagePoint * (1 + playerStats.attackBuffRatio);
                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().staminaDamage = weapon.transSkills[0].tenacityDamagePoint * (1 + playerStats.attackBuffRatio);
                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().energyRestoreAmount = weapon.transSkills[0].energyRestore;
                weaponSlotManager.mainArmedWeapon.GetComponentInChildren<DamageCollider>().hitChargeAmount = weapon.transSkills[0].hitPoint;
                playerManager.GetComponent<PlayerStats>().currStamina -= weapon.transSkills[0].staminaCost;
            }
            animatorManager.pauseDuration = weapon.transSkills[0].pauseDuration;
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
    }
    void HandleAttackCharge() 
    {
        if (chargeValue >= 5) 
        {
            chargeValue = 0;
            BaGuaManager baGuaManager = GetComponent<BaGuaManager>();
            baGuaManager.curEnergyCharge += 25;
            if (playerInventory.unequippedWeaponItems[1] != null) 
            {
                noticeAudio.clip = sample_SFX.Bagua_SFX_List[4];
                noticeAudio.Play();
                playerManager.transAttackTimer = 1.75f;
                playerManager.canTransAttack = true;
                sample_VFX.baGuaRelated_List[1].Play();
            }
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
    public void PowerArrowSetUp() 
    {
        ProjectileDamager PowerfulProjectileDamager = weaponSlotManager.arrowObjs[1].GetComponentInChildren<ProjectileDamager>();
        ProjectileDamager projectileDamager = weaponSlotManager.arrowObjs[0].GetComponentInChildren<ProjectileDamager>();
        PowerfulProjectileDamager.curDamage = projectileDamager.curDamage * powerArrowRatio;
        PowerfulProjectileDamager.staminaDamage = projectileDamager.staminaDamage * powerArrowRatio;
        PowerfulProjectileDamager.energyRestoreAmount = 10f;
        PowerfulProjectileDamager.hitChargeAmount = 3f;
        PowerfulProjectileDamager.isEnhanced = true;
        PowerfulProjectileDamager.isHeavy = true;
    }
    public void GreatSwordChargeController() 
    {

        playerUIManager.gsChargeBarSlider.fillAmount = gsChargeSlot/100;

        if (playerInventory.curEquippedWeaponItem.Id == 0)
        {
            if (gsChargeSlot > 0)
            {
                playerUIManager.gsChargeBar.SetActive(true);
            }
            else
            {
                if (playerManager.isDefending)
                {
                    playerUIManager.gsChargeBar.SetActive(true);
                }
                else
                {
                    playerUIManager.gsChargeBar.SetActive(false);
                }
            }

            if (gsChargeSlot >= 0 && gsChargeSlot < 50)
            {
                gsChargeLevel = 0;
                gsEnhanceVFX.SetActive(false);
            }

            if (gsChargeSlot >= 50 && gsChargeSlot < 100)
            {
                gsChargeLevel = 1;
                gsEnhanceVFX.SetActive(true);
            }

            if (gsChargeSlot >= 100)
            {
                gsChargeSlot = 100;
                gsChargeLevel = 2;
                gsEnhanceVFX.SetActive(true);
            }

            if (!playerManager.isDefending && gsChargeSlot > 0)
            {
                if (gsChargeLevel == 0)
                {
                    gsChargeSlot -= 10 * Time.deltaTime;
                    if (gsChargeSlot <= 0) gsChargeSlot = 0;
                }
                else if (gsChargeLevel == 1)
                {
                    gsChargeSlot -= 10 * Time.deltaTime;
                    if (gsChargeSlot <= 50)
                    {
                        gsChargeSlot = 50;
                    }
                }
                else if (gsChargeLevel == 2)
                {
                    gsChargeSlot = 100;
                }
            }
        }
        else
        {
            gsChargeSlot = 0;
            playerUIManager.gsChargeBar.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + executionOffset, 4f);
    }
}
