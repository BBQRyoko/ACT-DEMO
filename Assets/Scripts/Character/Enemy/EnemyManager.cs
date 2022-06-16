using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : CharacterManager
{
    public GameObject enemyHolder;
    CameraManager cameraManager;
    InputManager inputManager;
    EnemyLocomotion enemyLocomotion;
    EnemyAnimatorManager enemyAnimatorManager;
    EnemyWeaponSlotManager enemyWeaponSlotManager;
    EnemyStats enemyStats;

    [Header("Reset")]
    [SerializeField] Vector3 enemyOriginalPosition;
    [SerializeField] Quaternion enemyOriginalRotation;
    public Transform originalParent;
    public bool enemyActivated;

    [Header("OtherParameter")]
    public Transform targetMarkTransform;
    public Rigidbody enemyRig;
    public CapsuleCollider collider_Self;
    public CapsuleCollider collider_Combat;
    public NavMeshAgent navMeshAgent;
    public State curState;
    public CharacterStats curTarget;
    CombatCooldownManager combatCooldownManager;
    CombatStanceState combatStanceState;

    public Transform execute_Front;
    public Transform execute_Back;
    [SerializeField] GameObject backStabArea;
    public ParryCollider parryCollider;

    //CombatRelated
    public bool beenLocked;
    public bool healtBarSpawn;
    public bool canBeExecuted;
    public bool getingExecute;
    public bool isNoWeapon;
    public bool isEquipped;
    public bool isParrying;
    public bool isBlocking;
    public bool isDamaged;

    public bool ambushEnemy;
    public bool stayedRangeEnemy;
    public bool isFirstStrike;
    public float firstStrikeTimer;
    public float defaultFirstStrikeTime;
    public bool isDodging;
    public bool isTaijied;

    //待机模式
    public enum IdleType { Stay, Patrol, Boss };
    public IdleType idleType;
    public bool isBoss;
    public List<Transform> patrolPos = new List<Transform>();
    public int curPatrolIndex = 0;

    public bool isPreformingAction;
    public bool isInteracting;
    public bool isGround;
    public bool isImmuneAttacking;
    bool canReset;

    [Header("敌人主要参数")]
    public float rotationSpeed = 0.8f;
    public float moveSpeed = 1f;
    public float alertRadius = 15;
    public float alertTimer;
    public bool canAlertOthers = true;
    public bool calledAlready;
    public CharacterStats alertingTarget;
    public bool isAlerting;
    public bool alertIconSpawn;
    public float curDetectionRadius;
    public float detectionRadius = 10;
    public float alertDetectionRadius;
    public float minCombatRange = 3f;
    public float maxCombatRange = 3f;
    public float combatPursueStartRange = 6f;
    public float announcedPursueDistance = 25;

    public float maxDetectionAngle = 70;
    public float minDetectionAngle = -70;
    public bool attackLock;
    public float curRecoveryTime = 0;

    [Header("战斗参数")]
    public float hitRatio = 0.25f; //0~1，数值越高越难出现受击
    public float attackRatio = 1;
    public float hitGaugeRecoveryRate = 1;
    public float defPriority;
    public float dodgePriority;
    public float rollAtkPriority;
    public float defensiveRatio;

    [Header("飞行道具参数, 当前一个角色最多可以有两种不同类型/属性的飞行道具")]
    public FlyingObj arrow;
    public Transform shootPos;
    public FlyingObj arrow2;
    public Transform shootPos2;
    public Transform target;
    public Transform target2;

    [Header("道具掉落")]
    [SerializeField] GameObject[] containedItem;
    bool itemDrop;

    [Header("FinalBossOnly")]
    [SerializeField] StaminaBar bossStaminaBar;
    public bool isPhaseChaging;
    public bool phaseChanged;
    public bool canExecute;
    public GameObject windShield;
    public EnemyAttackAction[] boss2ndPhaseAttacks;
    public List<SpecialCondition> boss2ndPhaseConditionList;

    private void Awake()
    {
        enemyHolder = transform.parent.gameObject;
        cameraManager = FindObjectOfType<CameraManager>();
        inputManager = FindObjectOfType<InputManager>();
        enemyLocomotion = GetComponent<EnemyLocomotion>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyWeaponSlotManager = GetComponentInChildren<EnemyWeaponSlotManager>();
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRig = GetComponent<Rigidbody>();
        combatCooldownManager = GetComponentInChildren<CombatCooldownManager>();
        combatStanceState = GetComponentInChildren<CombatStanceState>();
        navMeshAgent.enabled = false;
        curDetectionRadius = detectionRadius;
        enemyOriginalPosition = transform.position;
        enemyOriginalRotation = transform.rotation;
        originalParent = transform.parent;
        patrolPos.Clear();
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
        AmbushEnemy();
        ExecutedArea();
        GeneralTimerManager();
        HandleAlert();
        HandleParryCollider();
        HandleHealthBar();
        FinalBossController();
        ItemDrop();

        //if (!alertIconSpawn) 
        //{
        //    alertingTarget = null;
        //}
        if (isDead && collider_Self.enabled)
        {
            curTarget = null;
            collider_Self.enabled = false;
            collider_Combat.enabled = false;
            if (enemyWeaponSlotManager.weaponDamageCollider) enemyWeaponSlotManager.weaponDamageCollider.DisableDamageCollider();
            if (enemyWeaponSlotManager.kickDamagerCollider) enemyWeaponSlotManager.kickDamagerCollider.DisableDamageCollider();
            Destroy(gameObject.transform.parent.gameObject, 10f);
        }

        if (curTarget)
        {
            shootPos = curTarget.GetComponent<PlayerManager>().beTargetedPos;
            shootPos2 = curTarget.GetComponent<PlayerManager>().beTargetedPos;
        }
        else
        {
            shootPos = null;
            shootPos2 = null;
        }
    }
    void GeneralTimerManager()
    {
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
        if (isStunned && !isToronadoCovered)
        {
            stunTimer += Time.deltaTime;
            if (stunTimer >= 5)
            {
                enemyAnimatorManager.animator.SetBool("isStunned", false);
                stunTimer = 0;
                enemyStats.currStamina = enemyStats.maxStamina;
            }
        }
    }
    private void FixedUpdate()
    {
        HandleStateMachine();
        DefendCheck();
        combatCooldownManager.CountDownAllTimer();
    }
    private void LateUpdate()
    {
        isInteracting = enemyAnimatorManager.animator.GetBool("isInteracting");
        isRotatingWithRootMotion = enemyAnimatorManager.animator.GetBool("isRotatingWithRootMotion");
        isDodging = enemyAnimatorManager.animator.GetBool("isDodging");
        canRotate = enemyAnimatorManager.animator.GetBool("canRotate");
        isStunned = enemyAnimatorManager.animator.GetBool("isStunned");
        canReset = enemyAnimatorManager.animator.GetBool("canReset");
        isPreformingAction = isInteracting;
        ClearAllStatus();
    }
    void ClearAllStatus()
    {
        if (canReset)
        {
            enemyAnimatorManager.animator.SetBool("isDodging", false);
            isImmuneAttacking = false;
            getingExecute = false;
            enemyAnimatorManager.animator.SetBool("isInteracting",false);
            if (enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().weaponDamageCollider) enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().weaponDamageCollider.DisableDamageCollider();
            enemyAnimatorManager.animator.SetBool("canReset", false);
        }
    }
    private void HandleStateMachine() //单位状态机管理
    {
        if (curState != null && !isDead && !isStunned)
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
    private void HandleHealthBar()
    {
        if (beenLocked && !healtBarSpawn)
        {
            cameraManager.GenerateUIBar(this);
            healtBarSpawn = true;
        }

        if (!cameraManager.currentLockOnTarget)
        {
            beenLocked = false;
            healtBarSpawn = false;
        }
        else if (cameraManager.currentLockOnTarget.GetComponentInParent<EnemyManager>() != this)
        {
            beenLocked = false;
            healtBarSpawn = false;
        }
        else if (cameraManager.currentLockOnTarget.GetComponentInParent<EnemyManager>() == this)
        {
            beenLocked = true;
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
    public void HandleRangeAttack() //飞行道具
    {
        var obj = Instantiate(arrow, transform, false);
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        obj.StartFlyingObj(target, false, targetMarkTransform);
    }
    public void HandleRangeAttack2() //龙卷
    {
        var obj = Instantiate(arrow2, transform, false);
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        obj.StartFlyingObj(target2, true);
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
        enemyStats.currStamina -= staminaDamage;
        if (enemyStats.currStamina > 0)
        {
            enemyAnimatorManager.PlayTargetAnimation("Block_1", true, true);
            isBlocking = true;
        }
        else
        {
            enemyStats.currStamina = 0;
            enemyAnimatorManager.PlayTargetAnimation("ParryBreak", true, true);
        }
    }
    void AmbushEnemy()
    {
        if (ambushEnemy && !isEquipped)
        {
            enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().WeaponEquip();
        }
    }
    void HandleAlert()
    {
        alertDetectionRadius = 1.5f * detectionRadius;
        if (alertingTarget) isAlerting = true;
        if (isAlerting)
        {
            curDetectionRadius = alertDetectionRadius;
        }
        if (alertTimer > 0)
        {
            if (!alertIconSpawn && !ambushEnemy)
            {
                cameraManager.GenerateAlertIcon(this);
                alertIconSpawn = true;
            }
        }
        else
        {
            alertingTarget = null;
            alertIconSpawn = false;
        }
    }
    public void ExecutedArea()
    {
        if (!curTarget && !isDead)
        {
            enemyActivated = false;
            backStabArea.GetComponent<Collider>().enabled = true;
            canBeExecuted = true;
        }
        else
        {
            enemyActivated = true;
            if (!isStunned)
            {
                backStabArea.GetComponent<Collider>().enabled = false;
                canBeExecuted = false;
            }
            else
            {
                backStabArea.GetComponent<Collider>().enabled = true;
                canBeExecuted = true;
            }
        }
    }
    public void HandleExecuted(string skillName)
    {
        enemyAnimatorManager.animator.SetBool("isDodging", false);
        isImmuneAttacking = false;
        enemyAnimatorManager.PlayTargetAnimation(skillName, true, true);
        IdleState idleState = GetComponentInChildren<IdleState>();
        idleState.PlayerNoticeAnnounce(idleState.announceDistance, true);
        if (isStunned)
        {
            enemyAnimatorManager.animator.SetBool("isStunned", false);
            stunTimer = 0;
            enemyStats.currStamina = enemyStats.maxStamina;
        }
    }
    void ItemDrop()
    {
        if (isDead && !itemDrop && (containedItem != null || containedItem[1] != null))
        {
            foreach (GameObject curItem in containedItem)
            {
                GameObject item = Instantiate(curItem, transform.position, transform.rotation);
                item.transform.parent = null;
                float angel = Random.Range(-15f, 15f);
                item.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.up * 5f;
            }
            itemDrop = true;
        }
    }
    public void AutoLockOff()
    {
        if (cameraManager.currentLockOnTarget)
        {
            cameraManager.currentLockOnTarget = null;
            inputManager.lockOn_Flag = false;
        }
    }
    public void EnemyRestartReset() 
    {
        float distanceFromTarget = Vector3.Distance(enemyOriginalPosition, transform.position);
        if (distanceFromTarget >= 0.5f)
        {
            transform.position = enemyOriginalPosition;
            transform.rotation = enemyOriginalRotation;
        }
        curTarget = null;
        if(enemyStats.currHealth < enemyStats.maxHealth) enemyStats.currHealth = enemyStats.maxHealth;
        if (curState != GetComponentInChildren<IdleState>()) curState = GetComponentInChildren<IdleState>();
        isDamaged = false;
        enemyAnimatorManager.animator.SetBool("canReset", true);
    }
    public void EnemyPosReset() 
    {
        float distanceFromTarget = Vector3.Distance(enemyOriginalPosition, transform.position);
        if (distanceFromTarget >= 0.5f)
        {
            transform.position = enemyOriginalPosition;
            transform.rotation = enemyOriginalRotation;
        }
    }
    void FinalBossController() 
    {
        if (isPhaseChaging)
        {
            IdleState idleState = GetComponentInChildren<IdleState>();
            AttackState attackState = GetComponentInChildren<AttackState>();
            collider_Self.enabled = false;
            if (attackState.curAttack) 
            {
                attackState.curAttack = null;
            }
            curState = idleState;
        }
        else 
        {
            if (!collider_Self.enabled && !isDead) collider_Self.enabled = true;
        }

        if (phaseChanged) //boss二阶段的状态
        {
            if (combatStanceState.enemyAttacks != boss2ndPhaseAttacks) 
            {
                combatStanceState.enemyAttacks = boss2ndPhaseAttacks;
                combatStanceState.conditionList = boss2ndPhaseConditionList;
                combatStanceState.combatCooldownManager.CombatCooldownReset();
                enemyStats.currHealth = enemyStats.maxHealth;
                enemyStats.maxStamina = 250f;
                enemyStats.currStamina = enemyStats.maxStamina;
                bossStaminaBar.SetMaxStamina(enemyStats.maxStamina);
                enemyStats.staminaRegen = 25f;
                attackRatio = 1.1f;
                defPriority = 1.25f;
                dodgePriority = 1.75f;
                rollAtkPriority = 2f;
                defensiveRatio = 0.85f;
                combatStanceState.walkingTimer = 0.55f;
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
            //警戒范围
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, alertRadius);
            //直接视野范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            //警戒状态直接视野范围
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, alertDetectionRadius);
            //攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxCombatRange);    
    }
}
