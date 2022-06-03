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
    EnemyStats enemyStats;

    [Header("Reset")]
    [SerializeField] GameObject enemyResetPrefab;
    [SerializeField] Transform enemyOriginalTransform;
    [SerializeField] Vector3 enemyOriginalPosition;
    [SerializeField] Quaternion enemyOriginalRotation;
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
    [SerializeField] ParryCollider parryCollider;

    //CombatRelated
    public bool beenLocked;
    public bool healtBarSpawn;
    public bool canBeExecuted;
    public bool getingExecute;
    public bool isNoWeapon;
    public bool isEquipped;
    public float defPriority;
    public float dodgePriority;
    public float rollAtkPriority;
    public float defensiveRatio;
    public bool isParrying;
    public bool isBlocking;
    public bool isDamaged;

    public bool ambushEnemy;
    public bool stayedRangeEnemy;
    public bool isFirstStrike;
    public float firstStrikeTimer;
    public float defaultFirstStrikeTime;
    public bool isDodging;

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
    public CharacterStats alertingTarget;
    public bool isAlerting;
    public float alertingPeriod;
    public bool alertIconSpawn;
    public float detectionRadius = 10;
    public float minCombatRange = 3f;
    public float maxCombatRange = 3f;
    public float combatPursueStartRange = 6f;
    public float pursueMaxDistance = 20;
    public float announcedPursueDistance = 25;

    public float maxDetectionAngle = 70;
    public float minDetectionAngle = -70;
    public bool attackLock;
    public float curRecoveryTime = 0;

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

    private void Awake()
    {
        enemyHolder = transform.parent.gameObject;
        cameraManager = FindObjectOfType<CameraManager>();
        inputManager = FindObjectOfType<InputManager>();
        enemyLocomotion = GetComponent<EnemyLocomotion>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        enemyStats = GetComponent<EnemyStats>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRig = GetComponent<Rigidbody>();
        combatCooldownManager = GetComponentInChildren<CombatCooldownManager>();
        combatStanceState = GetComponentInChildren<CombatStanceState>();
        navMeshAgent.enabled = false;
        enemyResetPrefab = transform.parent.gameObject;
        enemyOriginalTransform = transform.transform;
        enemyOriginalPosition = transform.position;
        enemyOriginalRotation = transform.rotation;
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
        //DistanceCheck
        HandleRecoveryTimer();
        AmbushEnemy();
        ExecutedArea();
        GeneralTimerManager();
        HandleAlertIcon();
        HandleParryCollider();
        HandleHealthBar();
        ItemDrop();

        //if (!alertIconSpawn) 
        //{
        //    alertingTarget = null;
        //}
        if (isDead)
        {
            curTarget = null;
            collider_Self.enabled = false;
            collider_Combat.enabled = false;
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

        if (isAlerting)
        {
            alertTimer = 5f;
            alertingPeriod += Time.deltaTime;
            if (alertingPeriod >= 5f)
            {
                isAlerting = false;
                alertingPeriod = 0;
            }
        }
        else
        {
            alertingPeriod = 0;
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
        isInteracting = enemyAnimatorManager.animator.GetBool("isInteracting");
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
        Debug.Log(obj);

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
        enemyStats.currStamina -= staminaDamage * 0.8f;
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
    void HandleAlertIcon() 
    {
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
            backStabArea.GetComponent<Collider>().enabled = true;
            canBeExecuted = true;
        }
        else 
        {
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
    public void AutoLockOn() 
    {
        if (!cameraManager.currentLockOnTarget)
        {
            cameraManager.currentLockOnTarget = targetMarkTransform;
            inputManager.lockOn_Flag = true;
        }
    }
    public void EnemyReset() 
    {
        if (enemyActivated) 
        {
            enemyActivated = false;
            transform.position = enemyOriginalPosition;
            transform.rotation = enemyOriginalRotation;
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
            //攻击范围
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxCombatRange);    
    }
}
