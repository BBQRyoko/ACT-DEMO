using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : CharacterManager
{
    EnemyLocomotion enemyLocomotion;
    EnemyAnimatorManager enemyAnimatorManager;
    EnemyStats enemyStats;

    public Rigidbody enemyRig;
    public CapsuleCollider collider_Self;
    public CapsuleCollider collider_Combat;
    public NavMeshAgent navMeshAgent;
    public State curState;
    public CharacterStats curTarget;
    CombatCooldownManager combatCooldownManager;

    [SerializeField] ParryCollider parryCollider;

    //CombatRelated
    public bool isEquipped;
    public bool canDefend;
    public float defensiveRatio; 
    public bool isParrying;
    public bool isBlocking;
    public bool isDamaged;

    public bool isFirstStrike;
    public float firstStrikeTimer;
    public float defaultFirstStrikeTime;
    public bool isDodging;
    float dodgeTimer;

    //待机模式
    public enum IdleType {Stay, Patrol, Boss};
    public IdleType idleType;
    public List<Transform> patrolPos = new List<Transform>();
    public int curPatrolIndex = 0;

    public bool isPreformingAction;
    public bool isInteracting;
    public bool isImmuneAttacking;

    [Header("敌人主要参数")]
    public float rotationSpeed = 0.8f;
    public float moveSpeed = 1f;
    [SerializeField] bool gizmoOn;
    public float alertRadius = 15;
    public float detectionRadius = 10;
    public float maxAttackRange = 3f;
    public float pursueMaxDistance = 16;

    public float maxDetectionAngle = 70;
    public float minDetectionAngle = -70;

    public float curRecoveryTime = 0;

    [Header("飞行道具参数, 当前一个角色最多可以有两种不同类型/属性的飞行道具")]
    public FlyingObj arrow;
    public Transform shootPos;
    public FlyingObj arrow2;
    public Transform shootPos2;
    public Transform target;
    public Transform target2;

    private void Awake()
    {
        enemyLocomotion = GetComponent<EnemyLocomotion>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRig = GetComponent<Rigidbody>();
        combatCooldownManager = GetComponentInChildren<CombatCooldownManager>();
        navMeshAgent.enabled = false;
    }

    private void Start()
    {
        enemyRig.isKinematic = false;
        foreach (Transform child in gameObject.transform.parent) //巡逻模式
        {
            if (child.tag != "Enemy")
            {
                patrolPos.Add(child);
            }
        }
    }
    private void Update()
    {
        HandleRecoveryTimer();
        HandleParryCollider();
        isRotatingWithRootMotion = enemyAnimatorManager.animator.GetBool("isRotatingWithRootMotion");
        canRotate = enemyAnimatorManager.animator.GetBool("canRotate");

        if (isDead) 
        {
            enemyRig.isKinematic = true;
            collider_Self.enabled = false;
            collider_Combat.enabled = false;
            Destroy(gameObject, 10f);
        }
    }

    void GeneralTimerManager() 
    {
        if (isDodging) //躲避时间
        {
            dodgeTimer = 7.5f;
        }
        else
        {
            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                dodgeTimer = 0;
            }
        }

        if (!isFirstStrike) 
        { 
            if (firstStrikeTimer > 0)
            {
                firstStrikeTimer -= Time.deltaTime;
            }
            else 
            {
                firstStrikeTimer = 0;
            }
        }

        //处决状态Timer
        if (isWeak) 
        {
            
        }
    }
    private void FixedUpdate()
    {
        HandleStateMachine();
        GeneralTimerManager();
        DefendCheck();
        combatCooldownManager.CountDownAllTimer();
    }
    private void LateUpdate()
    {
        isInteracting = enemyAnimatorManager.animator.GetBool("isInteracting");
        isWeak = enemyAnimatorManager.animator.GetBool("isWeak");
    }
    private void HandleStateMachine() //单位状态机管理
    {
        if (curState != null && !isDead)
        {
            State nextState = curState.Tick(this, enemyStats, enemyAnimatorManager);

            if (nextState != null)
            {
                SwitchToNextState(nextState);
            }
        }
    }
    private void SwitchToNextState(State state) 
    {
        curState = state;
    }
 
       
    private void HandleRecoveryTimer() //攻击间隔
    {
        if (curRecoveryTime > 0) 
        {
            curRecoveryTime -= Time.deltaTime;
        }

        if (isPreformingAction) 
        {
            if (curRecoveryTime <= 0) 
            {
                isPreformingAction = false;
            }
        }
    }
    public void HandleRangeAttack() 
    {
        var obj = Instantiate(arrow, transform, false);
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        obj.StartFlyingObj(target);
    }
    public void HandleRangeAttack2()
    {
        var obj = Instantiate(arrow2, transform, false);
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        obj.StartFlyingObj(target2);
    }

    void DefendCheck() 
    {
        float horizontalMovementVaule = enemyAnimatorManager.animator.GetFloat("Horizontal");

        if (horizontalMovementVaule <= 2.5f && horizontalMovementVaule >= 1)
        {
            isParrying = true;
        }
        else 
        {
            isParrying = false;
        }
    }

    void HandleParryCollider() 
    {
        if (parryCollider) 
        {
            if (isParrying)
            {
                parryCollider.EnableParryCollider();
            }
            else
            {
                parryCollider.DisableParryCollider();
            }
        }
    }
    public void HandleParryingCheck(float staminaDamage) 
    {
        if (staminaDamage < enemyStats.currStamina)
        {
            enemyAnimatorManager.PlayTargetAnimation("Block_1", true, true);
            isBlocking = true;
        }
        else if(staminaDamage >= enemyStats.currStamina)
        {
            enemyAnimatorManager.PlayTargetAnimation("ParryBreak", true, true);
        }

        enemyStats.currStamina -= staminaDamage;
        if (enemyStats.currStamina <= 0) 
        {
            enemyStats.currStamina = 0;
        }
    }

    public void HandleExecuted(string skillName) 
    {
        enemyAnimatorManager.PlayTargetAnimation(skillName, true, true);
    }


    private void OnDrawGizmosSelected()
    {
        if (gizmoOn) 
        {
            //警戒范围
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, alertRadius);
            //察觉范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            //攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxAttackRange);
        }
    }

}
