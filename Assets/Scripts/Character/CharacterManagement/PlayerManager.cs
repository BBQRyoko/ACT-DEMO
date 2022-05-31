using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : CharacterManager
{
    //PlayerManager统一管理所有当前所处的状态, 与locomotion, input, camera的update
    GameManager gameManager;
    Animator animator;
    InputManager inputManager;
    public CameraManager cameraManager;
    PlayerLocmotion playerLocmotion;
    PlayerStats playerStats;
    AnimatorManager animatorManager;
    WeaponSlotManager weaponSlotManager;
    [SerializeField] Sample_VFX sample_VFX;
    BaGuaManager baGuaManager;
    Rigidbody rig;

    [Header("运动状态")]
    public bool isInteracting;
    public bool isUsingRootMotion;

    public bool gameStart;
    public bool isFalling; //下落时
    public bool isGround; //在地面时
    public bool isCrouching; //下蹲时
    public bool isInGrass; //草丛里
    public bool isSprinting; 
    public bool isJumping; //跳跃上升阶段
    public bool isHanging;
    public Vector3 hangDirection;
    public bool isClimbing;
    public Vector3 climbDirection;
    public bool inInteractTrigger;
    public bool interactObject;
    public bool canReset;

    //通用
    public int keyNum;

    //战斗
    public bool isHitting;
    public bool attackRotate;
    public bool isAttacking;
    public bool isImmuAttack;
    public bool cantBeInterrupted;
    public bool isGettingDamage;
    public bool isDefending;
    float staminaRegenPauseTimer;
    public bool staminaRegenPause;
    public bool hitRecover;
    public bool damageAvoid;
    public Transform beTargetedPos;
    [SerializeField] ParryCollider parryCollider;
    [SerializeField] AudioSource generalAudio;
    [SerializeField] Sample_SFX sfxList;

    //武器切换相关
    public bool isWeaponSwitching;
    public float weaponSwitchCooldown;
    public Image cooldownTimer;
    float cooldownUnit;

    //太极系统
    public bool yinYangAbilityOn;
    [SerializeField] GameObject ultimateHint;
    public float transAttackTimer;
    public bool canTransAttack;

    //太刀弹反
    public bool isPerfect;

    //蓄力攻击相关
    public bool isHolding;
    public bool isAiming;
    public bool isArrowLoaded;

    //远程攻击
    public Transform shooting_Target;
    public Transform straightLineNullTarget;
    [SerializeField] FlyingObj arrow;
    [SerializeField] FlyingObj powerArrow;
    [SerializeField] FlyingObj fireBall;
    [SerializeField] FlyingObj tornado;
    [SerializeField] Transform shoot_Pos;
    [SerializeField] Transform tornado_ShootPos;
    [SerializeField] Transform tornado_TargetPos;

    //ATField
    [SerializeField] GameObject aT_Field_Prefab;
    [SerializeField] Transform aT_position;

    [Header("TutorialRelated")]
    [SerializeField] GameObject wakeUp;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        animator = GetComponentInChildren<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocmotion = GetComponent<PlayerLocmotion>();
        playerStats = GetComponent<PlayerStats>();
        baGuaManager = GetComponent<BaGuaManager>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        parryCollider = GetComponentInChildren<ParryCollider>();
        rig = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (gameStart)
        {
            wakeUp.SetActive(true);
            playerStats.currHealth = 10;
            weaponSlotManager.mainWeapon_Unequipped.gameObject.SetActive(true);
            weaponSlotManager.mainArmedWeapon.SetActive(false);
            if (inputManager.interact_Input)
            {
                animator.SetTrigger("gameStart");
                inputManager.interact_Input = false;
                gameStart = false;
                wakeUp.SetActive(false);
            }
        }
        else 
        {
            weaponSlotManager.mainWeapon_Unequipped.gameObject.SetActive(false);
            if (isHanging || isClimbing)
            {
                weaponSlotManager.mainArmedWeapon.SetActive(false);
            }
            else 
            {
                weaponSlotManager.mainArmedWeapon.SetActive(true);
            }
        }
        if (yinYangAbilityOn)
        {
            ultimateHint.SetActive(true);
        }
        else
        {
            ultimateHint.SetActive(false);
        }
        if (!isDead) 
        {
            inputManager.HandleAllInputs();
        }
        playerStats.StaminaController();
        GeneralTimerController();
        PowerArrowController();
        RollingDamagerController();
    }
    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (!gameStart) 
            {
                playerLocmotion.HandleAllMovement();
            }
        }
        else 
        {
            rig.isKinematic = true;
            gameObject.GetComponent<Collider>().enabled = false;
            playerLocmotion.characterColliderBlocker.enabled = false;
            cameraManager.currentLockOnTarget = null;
            cameraManager.isLockOn = false;
            inputManager.lockOn_Flag = false;
            gameManager.PlayerDead();
        }
        cameraManager.HandleAllCameraMovement();
    }
    private void LateUpdate()
    {
        isInteracting = animator.GetBool("isInteracting");
        isAttacking = animator.GetBool("isAttacking");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        isHolding = animator.GetBool("isHolding");
        isDefending = animator.GetBool("isDefending");
        isGettingDamage = animator.GetBool("isGettingDamage");
        cantBeInterrupted = animator.GetBool("cantBeInterrupted");
        damageAvoid = animator.GetBool("isDodging");
        canReset = animator.GetBool("canReset");
        animator.SetBool("isStunned", isStunned);
        animator.SetBool("isGround", isGround); 
        animator.SetBool("isFalling", isFalling);
        //inputManager.interact_Input = false;
        inputManager.weaponSwitch_Input = false;
        HandleDefending();
        HoldingAction();
        ClearAllStatus();
    }
    private void GeneralTimerController() 
    {
        if (weaponSwitchCooldown > 0) 
        {
            weaponSwitchCooldown -= Time.deltaTime;
            cooldownTimer.fillAmount = weaponSwitchCooldown * cooldownUnit;
        }

        if (staminaRegenPauseTimer > 0)
        {
            staminaRegenPauseTimer -= Time.deltaTime;
            staminaRegenPause = true;
        }
        else 
        {
            staminaRegenPauseTimer = 0;
            staminaRegenPause = false;
        }

        if (isStunned && !isToronadoCovered)
        {
            if (!isFalling)
            {
                stunTimer += Time.deltaTime;
                if (stunTimer >= 1)
                {
                    isStunned = false;
                    stunTimer = 0;
                }
            }
            else 
            {
                isStunned = false;
                stunTimer = 0;
            }
        }

        if (transAttackTimer > 0)
        {
            canTransAttack = true;
            transAttackTimer -= Time.deltaTime;
            if (transAttackTimer <= 0)
            {
                transAttackTimer = 0;
                canTransAttack = false;
            }
        }
    }
    void ClearAllStatus() 
    {
        if (canReset) 
        {
            isPerfect = false;
            isImmuAttack = false;
            damageAvoid = false;
            if (weaponSlotManager.weaponDamageCollider) weaponSlotManager.weaponDamageCollider.DisableDamageCollider();
            animator.SetBool("canReset", false);
        }
    }
    public void HandleRangeAttack(int index)
    {
        if (index == 0) //弓箭
        {
            PlayerAttacker playerAttacker = GetComponent<PlayerAttacker>();
            PlayerInventory playerInventory = GetComponent<PlayerInventory>();

            if (!playerAttacker.isUsingPowerArrow)
            {
                var obj = Instantiate(arrow, shoot_Pos, false);
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
                obj.StartFlyingObj(shooting_Target, false, beTargetedPos);
            }
            else 
            {
                var obj = Instantiate(powerArrow, shoot_Pos, false);
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
                obj.StartFlyingObj(shooting_Target, false, beTargetedPos);
                playerInventory.powerArrowNum -= 1;
            }

        }
        else if (index == 1) //火球
        {
            var obj = Instantiate(fireBall, shoot_Pos, false);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            obj.StartFlyingObj(shooting_Target, false, beTargetedPos);
        }
        else if (index == 2) //龙卷
        {
            var obj = Instantiate(tornado, tornado_ShootPos, false);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            obj.StartFlyingObj(tornado_TargetPos, true);
        }
    }
    public void HandleDefending() 
    {
        if (isDefending)
        {
            PlayerInventory playerInventory = GetComponent<PlayerInventory>();
            if (playerInventory.curEquippedWeaponItem.Id == 0) //大剑设置无法移动
            {
                animator.SetBool("isInteracting", true);
            }
            parryCollider.EnableParryCollider();
        }
        else 
        {
            parryCollider.DisableParryCollider();
        }
    }
    public void HandleParryingCheck(float incomingDamage) 
    {
        PlayerInventory playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory.curEquippedWeaponItem.Id == 0)
        {
            playerStats.currStamina -= playerStats.maxStamina * 0.2f;
        }
        else if (playerInventory.curEquippedWeaponItem.Id == 1) 
        {
            if (incomingDamage <= playerStats.maxHealth / 2) //小伤害
            {
                playerStats.currStamina -= playerStats.maxStamina * 0.45f;
            }
            else //大伤害
            {
                playerStats.currStamina -= playerStats.maxStamina * 0.9f;
            }
        }

        if (playerStats.currStamina > 0)
        {
            staminaRegenPauseTimer = 1.5f;
            animator.SetTrigger("isDefendSuccess");
        }
        else 
        {
            playerStats.currStamina = 0;
            animator.SetTrigger("isDefendFailed");
            animatorManager.animator.SetBool("isDefending", false);
        }
    }
    private void HoldingAction() //按键保持
    {
        //if (!isHolding)
        //{
        //    inputManager.weaponAbility_Input = false;
        //}
        //else 
        //{
        //    inputManager.weaponAbility_Input = true;
        //}
    }
    public void weaponEquiping(bool beDamaging = false) 
    {
        weaponSlotManager.EquipeWeapon();
    }
    public void WeaponSwitchTimerSetUp(float timer) 
    {
        weaponSwitchCooldown = timer;
        cooldownTimer.fillAmount = 1;
        cooldownUnit = 1 / timer;
    }
    public void PowerArrowController() //检测是否开启强击失
    {
        PlayerInventory playerInventory = GetComponent<PlayerInventory>();
        PlayerAttacker playerAttacker = GetComponent<PlayerAttacker>();

        if (playerInventory.powerArrowNum <= 0)
        {
            playerAttacker.isUsingPowerArrow = false;
        }
        if (playerInventory.powerArrowNum >= 25) 
        {
            playerInventory.powerArrowNum = 25;
        }
        if (inputManager.specialAction_Input) 
        {
            if (playerInventory.powerArrowNum > 0 && !playerAttacker.isUsingPowerArrow && playerInventory.curEquippedWeaponItem.Id == 2)
            {
                playerAttacker.isUsingPowerArrow = true;
                inputManager.specialAction_Input = false;
            }
            else if( playerAttacker.isUsingPowerArrow )
            {
                playerAttacker.isUsingPowerArrow = false;
                inputManager.specialAction_Input = false;
            }
        }
    }
    void RollingDamagerController() 
    {
        if (damageAvoid)
        {
            animatorManager.rollDamager.EnableDamageCollider();
        }
        else 
        {
            animatorManager.rollDamager.DisableDamageCollider();
        }
    }
    public void HangingController() 
    {
        if (!isHanging)
        {
            isHanging = true;
            animatorManager.PlayTargetAnimation("JumpToHang", true, true);
        }
        else 
        {
            isHanging = false;
            animatorManager.PlayTargetAnimation("HangToIdle", true, true);
        }
    }
    public void ClimbingController()
    {
        if (!isClimbing)
        {
            isClimbing = true;
            animatorManager.PlayTargetAnimation("ClimbStart", true, true);
        }
        else
        {
            //isClimbing = false;
            animatorManager.PlayTargetAnimation("ClimbToTop", true, true);
        }
    }
    public void YinYangAbilityActivate() 
    {
        if (yinYangAbilityOn && !isHolding && !isAiming) 
        {
            baGuaManager.curYin = 0;
            baGuaManager.curYang = 0;
            yinYangAbilityOn = false;
            animatorManager.PlayTargetAnimation("Ultimate", true, true);
            GameObject AT_Field_Temp = Instantiate(aT_Field_Prefab, aT_position.position, Quaternion.identity);
            generalAudio.clip = sfxList.Bagua_SFX_List[0];
            generalAudio.Play();
            sample_VFX.baGuaRelated_List[0].Stop();
            sample_VFX.baGuaRelated_List[1].Play();
            AT_Field_Temp.transform.SetParent(null);
            inputManager.weaponAbility_Input = false;
            inputManager.reAttack_Input = false;
        }
    }
    public void Rest() 
    {
        if (isDead)
        {
            isDead = false;
            rig.isKinematic = false;
            gameObject.GetComponent<Collider>().enabled = true;
            playerLocmotion.characterColliderBlocker.enabled = true;
            playerStats.currHealth = playerStats.maxHealth;
            playerStats.currStamina = 150f;
            baGuaManager.curEnergyCharge = 0f;
        }
        else 
        {
            animatorManager.generalAudio.volume = 0.3f;
            animatorManager.generalAudio.clip = animatorManager.sample_SFX.checkPoint_Heal[0];
            animatorManager.generalAudio.Play();
            playerStats.currHealth = playerStats.maxHealth;
            playerStats.currStamina = 150f;
            baGuaManager.curEnergyCharge = 300f;
        }
    }
}
