using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerManager playerManager;
    PlayerStats playerStats;
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    PlayerLocmotion playerLocmotion;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;

    Vector2 movementInput;
    Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;

    public bool sprint_Input; //跑步键
    public bool roll_Input; //翻滚/冲刺键
    public bool crouch_Input;
    public bool jump_Input; //跳跃
    public bool interact_Input; //互动键

    //战斗
    public bool weaponSwitch_Input;

    //攻击
    public bool reAttack_Input;
    public bool spAttack_Input;
    public bool cbAttack_Input;

    //Ability
    public bool weaponAbility_Input;

    //大招
    public bool ultimateAbility_Input;

    //八卦盘
    public bool baGua_Input;

    //锁定
    CameraManager cameraManager;
    public bool lockOn_Input;
    public bool lockOn_Flag;
    public bool page_Up_Input;
    public bool page_Down_Input;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerStats = GetComponent<PlayerStats>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        playerLocmotion = GetComponent<PlayerLocmotion>();
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        cameraManager = FindObjectOfType<CameraManager>();
    }
    private void OnEnable()
    {
        if (playerControls == null) 
        {
            playerControls = new PlayerControls();

            //设置input的输入
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            //判定是否有跳跃输入
            playerControls.PlayerActions.Jump.started += i => jump_Input = true;
            playerControls.PlayerActions.Jump.canceled += i => jump_Input = false;
            playerControls.PlayerActions.Sprint.performed += i => sprint_Input = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprint_Input = false;
            playerControls.PlayerActions.Roll.performed += i => roll_Input = true;
            playerControls.PlayerActions.Crouch.performed += i => crouch_Input = true;
            playerControls.PlayerActions.Crouch.canceled += i => crouch_Input = false;

            playerControls.PlayerActions.Interact.performed += i => interact_Input = true;


            //攻击输入
            playerControls.PlayerActions.RegularAttack.performed += i => reAttack_Input = true;
            playerControls.PlayerActions.SpecialAttack.performed += i => spAttack_Input = true;
            playerControls.PlayerActions.SpecialAttack.canceled += i => spAttack_Input = false;
            playerControls.PlayerActions.CombieAttack.performed += i => cbAttack_Input = true;
            playerControls.PlayerActions.CombieAttack.canceled += i => cbAttack_Input = false;

            //Ability输入
            playerControls.PlayerActions.WeaponAbility.performed += i => weaponAbility_Input = true;
            playerControls.PlayerActions.WeaponAbility.canceled += i => weaponAbility_Input = false;

            //锁定模式
            playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
            playerControls.PlayerMovement.LockOnTargetLeft.performed += i => page_Up_Input = true;
            playerControls.PlayerMovement.LockOnTargetRight.performed += i => page_Down_Input = true;

            //武器切换
            playerControls.PlayerActions.WeaponSwitch.performed += i => weaponSwitch_Input = true;

            //八卦系统
            playerControls.PlayerActions.BaGuaSystem.performed += i => baGua_Input = true;
            playerControls.PlayerActions.BaGuaSystem.canceled += i => baGua_Input = false;

            //UI操作
            playerControls.UIActions.Backpack.performed += i => UIManager.OpenOrCloseUIForm("BackpackForm", playerInventory.items);
        }
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
    public void HandleAllInputs() 
    {
        HandleMovement();
        if (!playerManager.gameStart) 
        {
            HandleSprintInput();
            HandleRollInput();
            HandleCrouchInput();
            HandleUltimateInput();
            HandleAttackInput();
            HandleLockOnInput();
            HandleWeaponSwitch();
            HandleBaguaInput();
        }
        HandleInteractInput();
    }
    private void HandleMovement() 
    {
        movementInput.Normalize();
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInput.Normalize();
        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        if (lockOn_Flag && !sprint_Input)
        {
            animatorManager.UpdateAnimatorVaules(horizontalInput, verticalInput, playerManager.isSprinting);
        }
        else 
        {
            animatorManager.UpdateAnimatorVaules(0, moveAmount, playerManager.isSprinting);
        }
    }
    private void HandleSprintInput()
    {
        if (sprint_Input && moveAmount != 0 && playerStats.currStamina > 0)
        {
            playerManager.isSprinting = true;
        }
        else 
        {
            playerManager.isSprinting = false;
        }
    }
    private void HandleRollInput() 
    {
        if (roll_Input) 
        {
            roll_Input = false;
            playerLocmotion.HandleRoll();
        }
    }
    private void HandleCrouchInput()
    {
        if (crouch_Input)
        {
            crouch_Input = false;
            playerLocmotion.HandleCrouch();
        }
    }
    private void HandleAttackInput() 
    {
        if (reAttack_Input) 
        {
            playerAttacker.HandleRegularAttack(playerInventory.unequippedWeaponItems[playerInventory.currentWeaponIndex]);
        }

        if (weaponAbility_Input)
        {
            playerAttacker.HandleWeaponAbility(playerInventory.unequippedWeaponItems[0]);
            //playerAttacker.HandleDefend(playerInventory.unequippedWeaponItems[0]);
        }
        else 
        {
            if (playerManager.isHolding) 
            {
                animatorManager.animator.SetTrigger("isHoldingCancel");
            }
        }
    }
    private void HandleUltimateInput() 
    {
        if (cbAttack_Input) 
        {
            playerManager.YinYangAbilityActivate();
            weaponAbility_Input = false;
            reAttack_Input = false;
        }
    }
    private void HandleInteractInput() 
    {
        if (interact_Input) 
        {
            if (playerManager.inInteractTrigger && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Interact", true, true);
                playerManager.interactObject = true;
            }
        }
    }
    private void HandleLockOnInput() //手动锁定敌人
    {
        if (lockOn_Input && !lockOn_Flag)
        {
            lockOn_Input = false;
            cameraManager.HandleLockOn();
            if (cameraManager.nearestLockOnTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.nearestLockOnTarget;
                lockOn_Flag = true;
            }
        }
        else if (lockOn_Input && lockOn_Flag)
        {
            lockOn_Input = false;
            lockOn_Flag = false;
            //取消锁定
            cameraManager.ClearLockOnTargets();
        }

        if (page_Up_Input && lockOn_Flag)
        {
            page_Up_Input = false;
            cameraManager.HandleLockOn();
            if (cameraManager.leftLockTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.leftLockTarget;
            }
        }

        if (lockOn_Flag && page_Down_Input) 
        {
            page_Down_Input = false;
            cameraManager.HandleLockOn();
            if (cameraManager.rightLockTarget != null)
            {
                cameraManager.currentLockOnTarget = cameraManager.rightLockTarget;
            }
        }  
    }
    private void HandleWeaponSwitch() 
    {
        if (weaponSwitch_Input && !playerManager.cantBeInterrupted) 
        {
            playerManager.GetComponentInChildren<WeaponSlotManager>().WeaponSwitch();
        }
    }
    private void HandleBaguaInput() 
    {
        if (baGua_Input) 
        {
            cameraManager.cameraLock = true;
        }
    }
}

