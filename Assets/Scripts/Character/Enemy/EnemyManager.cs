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

    public enum enemyType {melee, range};
    public enemyType curEnemyType;

    //木桩
    public bool isDummy;

    //Boss
    public bool isBoss;

    //CombatRelated
    public bool isEquipped;

    public bool isFirstAttack;
    public bool isDodging;
    public float dodgeTimer;

    //待机模式
    public enum IdleType {Stay, Patrol};
    public IdleType idleType;
    public List<Transform> patrolPos = new List<Transform>();
    public int curPatrolIndex = 0;

    public bool isPreformingAction;
    public bool isInteracting;
    public bool isImmuneAttacking;
    public float rotationSpeed = 0.8f;
    public float maxAttackRange = 3f;
    public float moveSpeed=1f;

    [Header("AI Setting")]
    public float detectionRadius = 10;
    public float pursueMaxDistance = 12;

    public float maxDetectionAngle = 70;
    public float minDetectionAngle = -70;

    public float curRecoveryTime = 0;
    public bool canDoCombo;
    public int comboCount;
    public int maxComboCount;

    //Range Objects
    public FlyingObj arrow;
    public Transform shootPos;
    public Transform target;
    public float rangeRecoveryTime;

    //BossOnly(距离方位检测)
    public int curTargetAngle; //0 - Front, 1 - Back, 2 - Flank
    public int curTargetDistance; //0 - Short, 1 - Mid, 2 - Long

    public float distanceFromTarget;
    public float viewableAngle;

    public bool shouted;
    public float shoutTimer;
    public float shoutRadius;
    public LayerMask playerLayer; //在EnemyAnimator中使用

    [SerializeField] float mediumRange = 6f;

    private void Awake()
    {
        enemyLocomotion = GetComponent<EnemyLocomotion>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRig = GetComponent<Rigidbody>();
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

        if (curEnemyType == enemyType.range) //临时做给远程单位的
        {
            maxAttackRange = 15f;
            detectionRadius = 18f;
            pursueMaxDistance = 25f;
        }

        comboCount = maxComboCount;
    }
    private void Update()
    {
        HandleRecoveryTimer();
        HandleAbilityTimer();

        isRotatingWithRootMotion = enemyAnimatorManager.animator.GetBool("isRotatingWithRootMotion");
        canRotate = enemyAnimatorManager.animator.GetBool("canRotate");

        if (isBoss) 
        {
            HandleTargetPositionCheck();
            if (curTarget) 
            {
                enemyStats.healthBar.gameObject.SetActive(true);
            }
        }

        if (isDead) 
        {
            enemyRig.isKinematic = true;
            collider_Self.enabled = false;
            collider_Combat.enabled = false;
            Destroy(gameObject, 10f);
        }

        //之后专门做一个Timer的function
        if (isDodging)
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
    }
    private void FixedUpdate()
    {
        HandleStateMachine();      
    }
    private void LateUpdate()
    {
        isInteracting = enemyAnimatorManager.animator.GetBool("isInteracting");
        if (comboCount == 0) 
        {
            //enemyAnimatorManager.animator.SetBool("canCombo", false);
        }
    }
    private void HandleStateMachine() //单位状态机管理
    {
        if (curState != null && !isDead && !isDummy)
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

    private void HandleTargetPositionCheck()  //Boss专用目标位置检测
    {
        if (curTarget) 
        {
            Vector3 targetDirection = curTarget.transform.position - transform.position;
            distanceFromTarget = Vector3.Distance(curTarget.transform.position, transform.position);
            float viewableAngle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

            //Angle Check
            if (viewableAngle >= 0 && viewableAngle < 45)
            {
                curTargetAngle = 0;
            }
            else if (viewableAngle < 0 && viewableAngle > -45)
            {
                curTargetAngle = 0;
            }
            else if (viewableAngle >= 45 && viewableAngle < 110)
            {
                curTargetAngle = 2;
            }
            else if (viewableAngle <= -45 && viewableAngle > -110)
            {
                curTargetAngle = 2;
            }
            else if (viewableAngle >= 110 && viewableAngle <= 180)
            {
                curTargetAngle = 1;
            }
            else if (viewableAngle <= -110 && viewableAngle >= -180)
            {
                curTargetAngle = 1;
            }

            //Distance Check
            if (distanceFromTarget > 0 && distanceFromTarget <= maxAttackRange)
            {
                curTargetDistance = 0;
            }
            else if (distanceFromTarget > maxAttackRange && distanceFromTarget <= mediumRange)
            {
                curTargetDistance = 1;
            }
            else if (distanceFromTarget > mediumRange)
            {
                curTargetDistance = 2;
            }
        }
       
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

    private void HandleAbilityTimer() 
    {
        if (shoutTimer > 0)
        {
            shoutTimer -= Time.deltaTime;
        }

        if (shouted)
        {
            if (shoutTimer <= 0)
            {
                shouted = false;
            }
        }
    }

    public void PlayHittedSound() 
    {
        enemyAnimatorManager.hittedAudio.clip = enemyAnimatorManager.boss_sfx.hittedSFX_List[Random.Range(0, 4)];
        enemyAnimatorManager.hittedAudio.Play();
    }

    public void HandleRangeAttack() 
    {
        var obj = Instantiate(arrow, transform, false);
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        obj.StartFlyingObj(target);
    }

    void OnDrawGizmosSelected()
    {
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(transform.position, shoutRadius);
    }
}
