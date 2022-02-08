using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotion : MonoBehaviour
{
    EnemyManager enemyManager;
    EnemyAnimatorManager enemyAnimatorManager;
    Rigidbody rigidbody;

    public Vector3 movementVelocity;
    float groundedGravity = -0.05f;
    public float gravity =-15;
    [SerializeField] LayerMask groundLayer;
    float rayCastHeightOffset = 0.5f;
    float radius = 0.15f;

    public CapsuleCollider characterCollider;
    public CapsuleCollider characterColliderBlocker;

    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Physics.IgnoreCollision(characterCollider, characterColliderBlocker, true);
    }

    private void FixedUpdate()
    {
        HandleGravity();
        GroundCheck();
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, movementVelocity.y, rigidbody.velocity.z);
    }

    public void HandleGravity()
    {
        float fallMultiplier = 2.0f; //下落加成, 加强下落的重力效果

        //基础重力参数变化
        if (enemyManager.isGround)
        {
            movementVelocity.y = groundedGravity; //当玩家isGround的时候玩家受到的重力统一为 groundedGravity = -0.05f
        }
        else //处于下落的状态会对基础重力进行加成
        {
            float previousYVelocity = movementVelocity.y;
            float newYVelocity = movementVelocity.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + previousYVelocity);

            movementVelocity.y += gravity * Time.deltaTime;
        }
    }

    public void GroundCheck()
    {
        //raycast和spherecast来检测是否在地上
        RaycastHit hit;
        Vector3 rayCastOrigin;
        rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y += rayCastHeightOffset;
        targetPosition = transform.position;

        //落地检测
        if (Physics.SphereCast(rayCastOrigin, radius, -Vector3.up, out hit, groundLayer))
        {
            //落地后的状态判定
            enemyManager.isGround = true;
            //collider触碰判定
            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
        }
        else
        {
            enemyManager.isGround = false;
        }

        //虚拟collider 针对斜坡
        if (enemyManager.isGround)
        {
            if (enemyManager.isInteracting || rigidbody.velocity.x != 0 || rigidbody.velocity.z != 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
}

