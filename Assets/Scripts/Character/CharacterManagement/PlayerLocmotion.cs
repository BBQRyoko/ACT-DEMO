﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocmotion : MonoBehaviour
{
    PlayerManager playerManager;
    InputManager inputManager;
    AnimatorManager animatorManager;
    CameraManager cameraManager;
    PlayerStats playerStats;
    PlayerAttacker playerAttacker;
    WeaponSlotManager weaponSlotManager;

    Transform cameraObject;
    public Rigidbody rig;

    [Header("重力与跳跃")]
    [SerializeField] float maxJumpHeight = 10f;
    [SerializeField] float maxJumpTime = 3f;
    float gravity;
    float groundedGravity = -0.05f;
    float initialJumpVelocity;
    float jumpTakeEffectTimer;
    bool jumpInputLocked;

    [Header("落地检测")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float rayCastHeightOffset = 0.5f;
    [SerializeField] float radius = 0.1f;
    public float inAirTimer;

    [Header("移动参数")]
    Vector3 playerDireee;
    [SerializeField] float movementSpeed = 10;
    [SerializeField] float inAirMovementSpeed = 4;
    [SerializeField] float crouchSpeed = 7f;
    float crouchTimer;
    [SerializeField] float sprintSpeed = 10;
    [SerializeField] float rotationSpeed = 15;
    public bool willRotateTowardsTarget;
    public Vector3 movementVelocity;
    Vector3 moveDirection;

    //翻滚(冲刺)参数
    [SerializeField] int rollStaminaCost = 25;

    //人物碰撞器, 用于防止玩家角色与敌人角色攻击时的穿模碰撞
    public CapsuleCollider characterCollider;
    public CapsuleCollider characterColliderBlocker;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerStats = GetComponent<PlayerStats>();
        playerAttacker = GetComponent<PlayerAttacker>();
        cameraManager = FindObjectOfType<CameraManager>();
        animatorManager = GetComponentInChildren<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        rig = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
        SetupJumpVariables();
    }
    private void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
    }
    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
        HandleGravity();
        HandleFallingAndLanding();
    }
    void SetupJumpVariables() //设置跳跃的参数, 重力通过1/2gt^2的方式用设置的跳跃高度与时间来控制
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }
    public void HandleGravity()
    {
        if (playerManager.isHanging || playerManager.isClimbing)
        {
            return;
        }

        //重力相关的状态变化
        if (!playerManager.isGround) //当玩家不在地面上时
        {
            if (!playerManager.isToronadoCovered) 
            {
                animatorManager.animator.SetBool("isInteracting", false);
                playerManager.isFalling = movementVelocity.y <= 0.0f || (!inputManager.jump_Input && jumpTakeEffectTimer >= 0.1f); //当y轴速度小于等于0时或者跳跃键松开时都进入下落
            }
        }
        else
        {
            playerManager.isFalling = false;
            maxJumpHeight = 10f;
            maxJumpTime = 1.5f;
            SetupJumpVariables();
        }

        float fallMultiplier = 2.0f; //下落加成, 加强下落的重力效果

        //基础重力参数变化
        if (playerManager.isGround)
        {
            groundedGravity = -0.05f;
            movementVelocity.y = groundedGravity; //当玩家isGround的时候玩家受到的重力统一为 groundedGravity = -0.05f
        }
        else if (playerManager.isFalling) //处于下落的状态会对基础重力进行加成
        {
            jumpTakeEffectTimer = 0f;

            float previousYVelocity = movementVelocity.y;
            float newYVelocity = movementVelocity.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + previousYVelocity);

            movementVelocity.y += gravity * Time.deltaTime;
        }
        else if (!playerManager.isGround && !playerManager.isToronadoCovered) //正常的跳跃状态时对玩家的重力作用, 对上升状态进行减速
        {
            float previousYVelocity = movementVelocity.y;
            float newYVelocity = movementVelocity.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + previousYVelocity);

            movementVelocity.y += gravity * Time.deltaTime;
        }
    }
    private void HandleMovement()
    {
        if (playerManager.isInteracting || playerManager.isAttacking || playerManager.isStunned || playerManager.isGetingExecuted)
            return;

        float curSpeed = (movementSpeed - 5 * (weaponSlotManager.weaponDamageCollider.weaponWeightRatio)) * (1 + playerStats.movementBuffRatio);

        //移动方向取决于相机的正面方向
        if (!playerManager.isHanging && !playerManager.isClimbing)
        {
            moveDirection = cameraObject.forward * inputManager.verticalInput;
            moveDirection += cameraObject.right * inputManager.horizontalInput;
            moveDirection.Normalize();
        }
        
        if(playerManager.isHanging)
        {
            Vector3 forwardDir = gameObject.transform.forward;
            Vector3 cameraDir = gameObject.transform.position - cameraObject.transform.position;
            forwardDir.Normalize();
            cameraDir.Normalize();

            moveDirection = playerManager.hangDirection * inputManager.horizontalInput;
            moveDirection.Normalize();
        }

        if (playerManager.isClimbing)
        {
            moveDirection = playerManager.climbDirection * inputManager.verticalInput;
            moveDirection.Normalize();
        }

        if (playerManager.isSprinting)
        {
            if (playerManager.isHanging || playerManager.isClimbing) return;
            if (playerManager.isCrouching)
            {
                curSpeed = crouchSpeed * 1.25f * (1 + playerStats.movementBuffRatio); ;
                moveDirection *= curSpeed;
                playerStats.CostStamina(15f * Time.deltaTime);
            }
            else
            {
                curSpeed = sprintSpeed * (1 + playerStats.movementBuffRatio);
                rotationSpeed = 6;
                moveDirection *= curSpeed;
                playerStats.CostStamina(15f * (1f + weaponSlotManager.weaponDamageCollider.weaponWeightRatio) * Time.deltaTime);
            }
            if (playerStats.currStamina <= 0) 
            {
                playerManager.isSprinting = false;
                inputManager.sprint_Input = false;
            }
        }
        else if (playerManager.isCrouching || playerManager.isHolding || playerManager.isHanging || playerManager.isClimbing)
        {
            curSpeed = crouchSpeed * (1 + playerStats.movementBuffRatio); ;
            moveDirection *= curSpeed;
        }
        else
        {
            rotationSpeed = 8;
            if (playerManager.isFalling)
            {
                curSpeed = inAirMovementSpeed;
                moveDirection *= curSpeed;
            }
            else
            {
                moveDirection *= curSpeed;
            }
        }
        //Assign移动的x,z轴的速度
        if (playerManager.isInteracting || playerManager.isToronadoCovered )
        {
            movementVelocity.x = 0f;
            movementVelocity.z = 0f;
        }
        else
        {
            movementVelocity.x = moveDirection.x;
            if (playerManager.isHanging || playerManager.isClimbing) 
            {
                movementVelocity.y = moveDirection.y;
            }
            movementVelocity.z = moveDirection.z;
        }
        rig.velocity = movementVelocity;
    }
    private void HandleRotation() //还可以优化
    {
        if (playerManager.isInteracting || playerManager.isStunned || playerManager.isHanging || playerManager.isClimbing)
            return;

        if (playerManager.isAiming)
        {
            Quaternion targetRotation = Quaternion.Euler(0, cameraManager.cameraTransform.eulerAngles.y, 0);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = playerRotation;
        }
        else 
        {
            if (inputManager.lockOn_Flag)
            {
                if (inputManager.sprint_Input || inputManager.roll_Input)
                {
                    Vector3 targerDirection = Vector3.zero;
                    targerDirection = cameraManager.cameraTransform.forward * inputManager.verticalInput;
                    targerDirection += cameraManager.cameraTransform.right * inputManager.horizontalInput;
                    targerDirection.Normalize();
                    targerDirection.y = 0;

                    if (targerDirection == Vector3.zero)
                    {
                        targerDirection = transform.forward;
                    }

                    Quaternion tr = Quaternion.LookRotation(targerDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                    transform.rotation = targetRotation;
                }
                else
                {
                    if (willRotateTowardsTarget)
                    {
                        HandleRotateTowardsTarger();
                    }

                    Vector3 rotationDirection = moveDirection;
                    rotationDirection = cameraManager.currentLockOnTarget.position - transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                    transform.rotation = targetRotation;
                }
            }
            else
            {
                float rSpeed = rotationSpeed;

                if (willRotateTowardsTarget)
                {
                    HandleRotateTowardsTarger();
                }

                if (!playerManager.attackRotate)
                {
                    rSpeed = rotationSpeed;
                    if (playerManager.isInteracting)
                        return;
                }
                else
                {
                    rSpeed = rotationSpeed / 8;
                }

                if (playerManager.isFalling)
                {
                    rSpeed = rotationSpeed / 10;
                }
                else
                {
                    rSpeed = rotationSpeed;
                }

                Vector3 targetDirection = Vector3.zero;

                targetDirection = cameraObject.forward * inputManager.verticalInput;
                targetDirection += cameraObject.right * inputManager.horizontalInput;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                    targetDirection = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion playerRotataion = Quaternion.Slerp(transform.rotation, targetRotation, rSpeed * Time.deltaTime);

                transform.rotation = playerRotataion;
            }
        }
    }
    public void HandleRotateTowardsTarger() 
    {
        if (cameraManager.currentLockOnTarget && !playerManager.damageAvoid)
        {
            Vector3 direction = cameraManager.currentLockOnTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15);
        }
        else if (cameraManager.curExuectionTarget) 
        {
            Vector3 direction = cameraManager.curExuectionTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15);
        }
    }
    private void HandleFallingAndLanding() //下落与落地相关
    {
        if (playerManager.isHanging || playerManager.isClimbing) return;
        //raycast和spherecast来检测是否在地上
        RaycastHit hit;
        Vector3 rayCastOrigin;
        rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y += rayCastHeightOffset;
        targetPosition = transform.position;

        if (playerManager.isFalling) //下落状态时的状态更新
        {
            playerManager.isJumping = false;
            jumpInputLocked = true;
            inAirTimer += Time.deltaTime;

            //animatorManager.PlayTargetAnimation("Falling", false);
            animatorManager.animator.SetBool("isUsingRootMotion", false);
        }

        //落地检测
        if (!playerManager.isJumping) //在跳跃状态下不会判定, 防止重复触发isGround状态
        {
            if (Physics.SphereCast(rayCastOrigin, radius, -Vector3.up, out hit, groundLayer))
            {
                if (inAirTimer >= 1.5f) //根据下落时间来判断在空中的时间, 当下落时间超过0.7f时触发大落地, 会有个起身动作
                {
                    animatorManager.animator.SetTrigger("isBigFall"); 
                    animatorManager.animator.SetBool("isInteracting", true);
                    rig.velocity = new Vector3(0, rig.velocity.y, 0);
                }

                //落地后的状态判定
                playerManager.isGround = true;
                playerManager.isJumping = false;
                playerManager.isFalling = false;
                inAirTimer = 0.0f;

                //collider触碰判定
                Vector3 rayCastHitPoint = hit.point;
                targetPosition.y = rayCastHitPoint.y;
            }
            else
            {
                playerManager.isGround = false;
            }
        }

        //脚底的虚拟collider(暂时不用动), 用于处理楼梯斜坡等问题
        if (playerManager.isGround && !playerManager.isJumping)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
    public void HandleJumping(float maxHeight, float maxTime) 
    {
        maxJumpHeight = maxHeight;
        maxJumpTime = maxTime;
        SetupJumpVariables();

        //松开按键重置跳跃功能
        if (!inputManager.jump_Input)
        {
            jumpInputLocked = false;
        }
        //当玩家在地上, 按下跳跃键, 且不在跳跃状态时
        if (playerManager.isGround && !playerManager.isJumping && !jumpInputLocked)
        {
            animatorManager.animator.SetBool("isJumping", true);

            if (inputManager.moveAmount != 0) //根据状态判定时移动跳跃还是原地跳跃
            {
                animatorManager.PlayTargetAnimation("JumpMove", false);
            }
            else
            {
                animatorManager.PlayTargetAnimation("Jump", false);
            }

            playerManager.isGround = false;
            playerManager.isJumping = true;
            movementVelocity.y = initialJumpVelocity; //给予跳跃的初速度
        }

        if (playerManager.isJumping) 
        {
            jumpTakeEffectTimer += Time.deltaTime;
        }
    }//角色跳跃相关
    public void HandleRoll() //角色翻滚(冲刺)
    {
        Vector3 forwardDir = gameObject.transform.forward;
        Vector3 cameraDir = gameObject.transform.position - cameraObject.transform.position;
        forwardDir.Normalize();
        cameraDir.Normalize();
        forwardDir.y = 0;
        cameraDir.y = 0;

        if (playerStats.currStamina >= 0f)
        {
            if (playerManager.isAttacking) //攻击状态下
            {
                if (playerManager.cantBeInterrupted || !playerManager.isGround || playerManager.isHanging || playerManager.isClimbing || playerManager.isAiming)
                    return;

                //还可以优化：现在只有在玩家背对相机的情况下时才正常，其它方向翻滚方向会不精准
                if (!inputManager.lockOn_Flag)
                {
                    if (((Mathf.Abs(cameraDir.x) > Mathf.Abs(cameraDir.z)))) //相机在X轴上时
                    {
                        if (inputManager.verticalInput < 0) //朝后滚
                        {
                            animatorManager.animator.SetTrigger("isBackRoll");
                        }
                        else if (inputManager.horizontalInput > 0) //朝右滚
                        {
                            animatorManager.animator.SetTrigger("isRightRoll");
                        }
                        else if (inputManager.horizontalInput < 0) //朝左滚
                        {
                            animatorManager.animator.SetTrigger("isLeftRoll");
                        }
                        else if (inputManager.verticalInput > 0) //朝前滚
                        {
                            animatorManager.animator.SetTrigger("isFrontRoll");
                        }
                        else if (inputManager.verticalInput == 0)
                        {
                            animatorManager.animator.SetTrigger("isBackStep");
                        }
                    }
                    else //相机朝Z轴方向时
                    {
                        if (inputManager.verticalInput < 0) //朝后滚
                        {
                            animatorManager.animator.SetTrigger("isBackRoll");
                        }
                        else if (inputManager.horizontalInput > 0) //朝右滚
                        {
                            animatorManager.animator.SetTrigger("isRightRoll");
                        }
                        else if (inputManager.horizontalInput < 0) //朝左滚
                        {
                            animatorManager.animator.SetTrigger("isLeftRoll");
                        }
                        else if (inputManager.verticalInput > 0) //朝前滚
                        {
                            animatorManager.animator.SetTrigger("isFrontRoll");
                        }
                        else if (inputManager.verticalInput == 0)
                        {
                            animatorManager.animator.SetTrigger("isBackStep");
                        }
                    }
                }
                else 
                {
                    Transform targetTransform = cameraManager.currentLockOnTarget;
                    Vector3 targetDir = targetTransform.position - transform.position;
                    targetDir.y = 0;
                    targetDir.Normalize();

                    if (((Mathf.Abs(targetDir.x) > Mathf.Abs(targetDir.z))))
                    {
                        if (Mathf.Abs(transform.forward.x) > Mathf.Abs(transform.forward.z))
                        {
                            if (inputManager.verticalInput * transform.forward.x * targetDir.x < 0) //朝后滚
                            {
                                animatorManager.animator.SetTrigger("isBackRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.x > 0) //朝右滚
                            {
                                animatorManager.animator.SetTrigger("isRightRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.x < 0) //朝左滚
                            {
                                animatorManager.animator.SetTrigger("isLeftRoll");
                            }
                            else if (inputManager.verticalInput * transform.forward.x * targetDir.x > 0) //朝前滚
                            {
                                animatorManager.animator.SetTrigger("isFrontRoll");
                            }
                            else if (inputManager.verticalInput == 0)
                            {
                                animatorManager.animator.SetTrigger("isBackStep");
                            }
                        }
                        else
                        {
                            if (inputManager.verticalInput * transform.forward.z * targetDir.x < 0) //朝后滚
                            {
                                animatorManager.animator.SetTrigger("isLeftRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.x > 0) //朝右滚
                            {
                                animatorManager.animator.SetTrigger("isBackRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.x < 0) //朝左滚
                            {
                                animatorManager.animator.SetTrigger("isFrontRoll");
                            }
                            else if (inputManager.verticalInput * transform.forward.z * targetDir.x > 0) //朝前滚
                            {
                                animatorManager.animator.SetTrigger("isRightRoll");
                            }
                            else if (inputManager.verticalInput * transform.forward.z == 0)
                            {
                                animatorManager.animator.SetTrigger("isBackStep");
                            }
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(transform.forward.x) > Mathf.Abs(transform.forward.z))
                        {
                            if (inputManager.verticalInput * transform.forward.x * targetDir.z < 0) //朝后滚
                            {
                                animatorManager.animator.SetTrigger("isRightRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.z > 0) //朝右滚
                            {
                                animatorManager.animator.SetTrigger("isFrontRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.z < 0) //朝左滚
                            {
                                animatorManager.animator.SetTrigger("isBackRoll");
                            }
                            else if (inputManager.verticalInput * transform.forward.x * targetDir.z > 0) //朝前滚
                            {
                                animatorManager.animator.SetTrigger("isLeftRoll");
                            }
                            else if (inputManager.verticalInput == 0)
                            {
                                animatorManager.animator.SetTrigger("isBackStep");
                            }
                        }
                        else
                        {
                            if (inputManager.verticalInput * transform.forward.z * targetDir.z < 0) //朝后滚
                            {
                                animatorManager.animator.SetTrigger("isBackRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.z > 0) //朝右滚
                            {
                                animatorManager.animator.SetTrigger("isRightRoll");
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.z < 0) //朝左滚
                            {
                                animatorManager.animator.SetTrigger("isLeftRoll");
                            }
                            else if (inputManager.verticalInput * transform.forward.z * targetDir.z > 0) //朝前滚
                            {
                                animatorManager.animator.SetTrigger("isFrontRoll");
                            }
                            else if (inputManager.verticalInput * transform.forward.z == 0)
                            {
                                animatorManager.animator.SetTrigger("isBackStep");
                            }
                        }
                    }
                }
                weaponSlotManager.UnloadArrowOnSlot();
                animatorManager.animator.SetBool("cantBeInterrupted",true);
                playerStats.CostStamina(rollStaminaCost);
            }
            else //非攻击状态下翻滚, 只有在非互动且站在地上的状态下才能翻滚
            {
                if (playerManager.isInteracting || !playerManager.isGround || playerManager.isHanging || playerManager.isClimbing)
                    return;

                if (!inputManager.lockOn_Flag) 
                {
                    animatorManager.PlayTargetAnimation("FrontRoll", true, true);
                }
                else
                {
                    Transform targetTransform = cameraManager.currentLockOnTarget;
                    Vector3 targetDir = targetTransform.position - transform.position;
                    targetDir.y = 0;
                    targetDir.Normalize();

                    if (((Mathf.Abs(targetDir.x) > Mathf.Abs(targetDir.z))))
                    {
                        if (Mathf.Abs(transform.forward.x) > Mathf.Abs(transform.forward.z))
                        {
                            if (inputManager.verticalInput * transform.forward.x * targetDir.x < 0) //朝后滚
                            {
                                animatorManager.PlayTargetAnimation("BackRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.x > 0) //朝右滚
                            {
                                animatorManager.PlayTargetAnimation("RightRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.x < 0) //朝左滚
                            {
                                animatorManager.PlayTargetAnimation("LeftRoll", true, true);
                            }
                            else if (inputManager.verticalInput * transform.forward.x * targetDir.x > 0) //朝前滚
                            {
                                animatorManager.PlayTargetAnimation("FrontRoll", true, true);
                            }
                            else if (inputManager.verticalInput == 0)
                            {
                                animatorManager.PlayTargetAnimation("StepBack", true, true);
                            }
                        }
                        else
                        {
                            if (inputManager.verticalInput * transform.forward.z * targetDir.x < 0) //朝后滚
                            {
                                animatorManager.PlayTargetAnimation("LeftRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.x > 0) //朝右滚
                            {
                                animatorManager.PlayTargetAnimation("BackRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.x< 0) //朝左滚
                            {
                                animatorManager.PlayTargetAnimation("FrontRoll", true, true);
                            }
                            else if (inputManager.verticalInput * transform.forward.z * targetDir.x > 0) //朝前滚
                            {
                                animatorManager.PlayTargetAnimation("RightRoll", true, true);
                            }
                            else if (inputManager.verticalInput * transform.forward.z == 0)
                            {
                                animatorManager.PlayTargetAnimation("StepBack", true, true);
                            }
                        }
                    }
                    else 
                    {
                        if (Mathf.Abs(transform.forward.x) > Mathf.Abs(transform.forward.z))
                        {
                            if (inputManager.verticalInput * transform.forward.x * targetDir.z < 0) //朝后滚
                            {
                                animatorManager.PlayTargetAnimation("RightRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.z > 0) //朝右滚
                            {
                                animatorManager.PlayTargetAnimation("FrontRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.x * targetDir.z < 0) //朝左滚
                            {
                                animatorManager.PlayTargetAnimation("BackRoll", true, true);
                            }
                            else if (inputManager.verticalInput * transform.forward.x * targetDir.z > 0) //朝前滚
                            {
                                animatorManager.PlayTargetAnimation("LeftRoll", true, true);
                            }
                            else if (inputManager.verticalInput == 0)
                            {
                                animatorManager.PlayTargetAnimation("StepBack", true, true);
                            }
                        }
                        else
                        {
                            if (inputManager.verticalInput * transform.forward.z * targetDir.z < 0) //朝后滚
                            {
                                animatorManager.PlayTargetAnimation("BackRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.z > 0) //朝右滚
                            {
                                animatorManager.PlayTargetAnimation("RightRoll", true, true);
                            }
                            else if (inputManager.horizontalInput * transform.forward.z * targetDir.z < 0) //朝左滚
                            {
                                animatorManager.PlayTargetAnimation("LeftRoll", true, true);
                            }
                            else if (inputManager.verticalInput * transform.forward.z * targetDir.z > 0) //朝前滚
                            {
                                animatorManager.PlayTargetAnimation("FrontRoll", true, true);
                            }
                            else if (inputManager.verticalInput * transform.forward.z == 0)
                            {
                                animatorManager.PlayTargetAnimation("StepBack", true, true);
                            }
                        }
                    }
                }
                weaponSlotManager.UnloadArrowOnSlot();
                animatorManager.animator.SetBool("cantBeInterrupted", true);
                playerStats.CostStamina(rollStaminaCost);
            }
        }
    }
    public void HandleCrouch() 
    {
        if (playerManager.isInteracting || !playerManager.isGround || playerManager.isHanging || playerManager.isClimbing)
            return;

        if (playerManager.isCrouching)
        {
            playerManager.isCrouching = false;
            inputManager.crouch_Input = false;
        }
        else
        {
            crouchTimer += Time.deltaTime;
            if (crouchTimer >= 0.5f) 
            {
                playerManager.isCrouching = true;
                inputManager.crouch_Input = false;
                crouchTimer = 0;
            }
        }
    }
}
