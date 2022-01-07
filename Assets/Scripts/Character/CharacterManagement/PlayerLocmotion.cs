using System.Collections;
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
    float rayCastHeightOffset = 0.5f;
    float radius = 0.2f;
    public float inAirTimer;

    [Header("移动参数")]
    [SerializeField] float movementSpeed = 7;
    [SerializeField] float inAirMovementSpeed = 4;
    [SerializeField] float sprintSpeed = 10;
    [SerializeField] float rotationSpeed = 15;
    public Vector3 movementVelocity;
    Vector3 moveDirection;
    //翻滚(冲刺)参数
    public Vector3 dashDir;
    public float distance;
    [SerializeField] int rollStaminaCost = 10;

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
        HandleJumping();
        HandleGravity();
        HandleFallingAndLanding();
        HandleChargingDash();
    }
    void SetupJumpVariables() //设置跳跃的参数, 重力通过1/2gt^2的方式用设置的跳跃高度与时间来控制
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }
    public void HandleGravity() 
    {
        //重力相关的状态变化
        if (!playerManager.isGround) //当玩家不在地面上时
        {
            playerManager.isInteracting = false;
            playerManager.isFalling = movementVelocity.y <= 0.0f || (!inputManager.jump_Input && jumpTakeEffectTimer >= 0.1f); //当y轴速度小于等于0时或者跳跃键松开时都进入下落
        }
        else 
        {
            playerManager.isFalling = false;
        }

        float fallMultiplier = 2.0f; //下落加成, 加强下落的重力效果

        //基础重力参数变化
        if (playerManager.isGround)
        {
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
        else if (!playerManager.isGround) //正常的跳跃状态时对玩家的重力作用, 对上升状态进行减速
        {
            float previousYVelocity = movementVelocity.y;
            float newYVelocity = movementVelocity.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + previousYVelocity);

            movementVelocity.y += gravity * Time.deltaTime;
        }
    }
    private void HandleMovement() 
    {
        if (playerManager.isInteracting || playerManager.isAttacking)
            return;

        //移动方向取决于相机的正面方向
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        float curSpeed = movementSpeed;
        if (playerManager.isSprinting)
        {
            curSpeed = sprintSpeed;
            moveDirection *= curSpeed;
            playerStats.CostStamina(15f * Time.deltaTime);
        }
        else 
        {
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
        if (playerManager.isInteracting)
        {
            movementVelocity.x = 0f;
            movementVelocity.z = 0f;
        }
        else 
        {
            movementVelocity.x = moveDirection.x;
            movementVelocity.z = moveDirection.z;
        }

        rig.velocity = movementVelocity;
    }
    private void HandleRotation() 
    {
        if (playerManager.isInteracting || playerManager.isAttacking)
            return;

        float rSpeed = rotationSpeed;
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
    public void HandleRotateTowardsTarger() 
    {
        if (cameraManager.currentLockOnTarget) 
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
    }
    private void HandleFallingAndLanding() //下落与落地相关
    {
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
                if (inAirTimer >= 0.7f) //根据下落时间来判断在空中的时间, 当下落时间超过0.7f时触发大落地, 会有个起身动作
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
    public void HandleJumping() 
    {
        //松开按键重置跳跃功能
        if (!inputManager.jump_Input)
        {
            jumpInputLocked = false;
        }
        //当玩家在地上, 按下跳跃键, 且不在跳跃状态时
        if (playerManager.isGround && !playerManager.isJumping && inputManager.jump_Input && !jumpInputLocked && !playerManager.isInteracting)
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
        if (playerManager.isAttacking) //攻击状态下
        {
            if (playerManager.cantBeInterrupted || !playerManager.isGround)
                return;

            //后>左右>前
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
            else if (inputManager.verticalInput >= 0) //朝前滚
            {
                animatorManager.animator.SetTrigger("isFrontRoll");
            }

            playerAttacker.comboCount = 0;
        }
        else //非攻击状态下翻滚, 只有在非互动且站在地上的状态下才能翻滚
        {
            if (playerManager.isInteracting || !playerManager.isGround)
                return;
            if (playerStats.currStamina >= rollStaminaCost)
            {
                animatorManager.PlayTargetAnimation("RegularRolling", true, true);
                playerStats.CostStamina(rollStaminaCost);
            }
        }
    }
    public void HandleChargingDash()  //蓄力攻击
    {
        if (playerManager.isAttackDashing) 
        {
            Vector3 dir = transform.forward;
            dir.Normalize();
            StartCoroutine(DashAttack(dir));
        }
    }
    IEnumerator DashAttack(Vector3 dir) //蓄力后的冲刺攻击
    {
        //cameraManager.cameraFollowSpeed = 0.1f;
        rig.AddForce(dir * 15f, ForceMode.Impulse);
        yield return new WaitForSecondsRealtime(0.1f);
        rig.velocity = Vector3.zero;
        playerManager.isAttackDashing = false;
    }
}
