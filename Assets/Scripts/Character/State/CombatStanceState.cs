using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public IdleState idleState;
    public AttackState attackState;
    public PursueState pursueState;
    public EnemyAttackAction[] enemyAttacks;
    public CombatCooldownManager combatCooldownManager;

    //特殊条件处理
    public List<SpecialCondition> conditionList;
    public bool specialConditionTriggered;

    bool canCounterAttack;
    [SerializeField] bool dummyMode;
    public bool isStraightDummy;
    [SerializeField] float defaultWalkingTimer = 1.5f;
    public float walkingTimer;
    [SerializeField] bool notFirstWalking;
    [SerializeField] bool isWalkingStop;
    [SerializeField] bool attackingAdjustment;
    public float distanceFromTarget;
    public bool randomDestinationSet = false;
    float verticalMovementVaule = 0;
    float horizontalMovementVaule = 0;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        if (enemyManager.isPhaseChaging)
        {
            return idleState;
        }

        if (enemyManager.curTarget.GetComponent<PlayerManager>().isDead) //玩家死亡(临时的, 之后要改)
        {
            enemyAnimatorManager.PlayTargetAnimation("Unarm", true, true);
            enemyManager.curTarget = null;
            enemyStats.currHealth = enemyStats.maxHealth; //回满血
            return idleState;
        } //判断玩家是否死亡

        if (enemyManager.isInteracting) //首先确认是否处在互动状态
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0);
            enemyAnimatorManager.animator.SetFloat("Horizontal", 0);
            return this;
        }
        //最大侦测距离调整
        if (enemyManager.curDetectionRadius != enemyManager.detectionRadius) enemyManager.curDetectionRadius = enemyManager.detectionRadius;
        //重置alert
        if (enemyManager.isAlerting)
        {
            enemyManager.isAlerting = false;
            enemyManager.alertingTarget = null;
            enemyManager.alertTimer = 0;
        }
        //确认单位与目标间的距离
        distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        enemyAnimatorManager.animator.SetFloat("Vertical", verticalMovementVaule, 0.2f, Time.deltaTime);
        enemyAnimatorManager.animator.SetFloat("Horizontal", horizontalMovementVaule, 0.2f, Time.deltaTime);
        attackState.hasPerformedAttack = false;
        DamageTakenWindow(enemyManager, enemyAnimatorManager); //位置可能要改
        SpecialActionWatcher(enemyManager);

        if (dummyMode) 
        {
            attackingAdjustment = true;
            if(!isStraightDummy) HandleRotateTowardsTarger(enemyManager);
            GetNewAttack(enemyManager);
        } 
        if (specialConditionTriggered) //根据条件观测是否触发特殊条件, 触发则直接发动攻击并停止踱步行为
        {
            randomDestinationSet = false;
            return attackState;
        }
        if (enemyManager.curRecoveryTime <= 0 && !enemyManager.attackLock && attackState.curAttack != null)
        {
            if (enemyManager.curTarget.GetComponent<PlayerManager>().cantBeInterrupted) //玩家在正好开始攻击时/或者防御状态时
            {
                if (enemyManager.curTarget.GetComponent<PlayerManager>().isDefending) 
                {
                    float attackRandomNum = Random.Range(0.001f, 1);
                    if (attackRandomNum >= 0.45f)
                    {
                        if (distanceFromTarget > attackState.curAttack.minDistanceNeedToAttack && !attackingAdjustment && !dummyMode)
                        {
                            notFirstWalking = true;
                            attackingAdjustment = true;
                            WalkAroundTarget(enemyManager, enemyAnimatorManager);
                        }
                        else
                        {
                            walkingTimer = 0f;
                            randomDestinationSet = false;
                            notFirstWalking = false;
                            isWalkingStop = false;
                            attackingAdjustment = false;
                            return attackState;
                        }
                    }
                    else 
                    {
                        attackState.curAttack = null;
                        enemyManager.curRecoveryTime += 0.5f;
                        WalkAroundTarget(enemyManager, enemyAnimatorManager);
                    }
                }
            }
            else 
            {
                if (distanceFromTarget > attackState.curAttack.minDistanceNeedToAttack && !attackingAdjustment && !dummyMode)
                {
                    notFirstWalking = true;
                    attackingAdjustment = true;
                    WalkAroundTarget(enemyManager, enemyAnimatorManager);
                }
                else
                {
                    walkingTimer = 0f;
                    randomDestinationSet = false;
                    notFirstWalking = false;
                    isWalkingStop = false;
                    attackingAdjustment = false;
                    return attackState;
                }
            }
        }//当GCD转完且有了攻击并且玩家为非攻击状态(以免撞上)

        if (!randomDestinationSet && !enemyManager.isFirstStrike && !dummyMode)
        {
            randomDestinationSet = true;
            DecideCirclingAction(enemyManager, enemyAnimatorManager);
        }//如果不在踱步状态, 那就进入踱步状态
        //先看踱步时间是否大于0
        if (walkingTimer > 0) //踱步时间大于0时 
        {
            walkingTimer -= Time.fixedDeltaTime;
            GetNewAttack(enemyManager);
            //暂时不要了, 会让敌人变得太passive
            //if (enemyManager.curRecoveryTime <= 0.25f)//当敌人GCD转完以后
            //{
            //    GetNewAttack(enemyManager);
            //    //ReadyToAttack(enemyManager);
            //}
            //else //不然则继续踱步
            //{
            //    walkingTimer -= Time.fixedDeltaTime; 
            //}
        }
        else //如果踱步时间小于0
        {
            if (dummyMode) return this;

            notFirstWalking = true;
            attackingAdjustment = false;
            WalkAroundTarget(enemyManager, enemyAnimatorManager);
        }
        //这里感觉有问题
        if (distanceFromTarget > enemyManager.combatPursueStartRange && !enemyManager.isFirstStrike && walkingTimer>0)//距离大于攻击范围后退回追踪状态
        {
            if (conditionList.Count != 0 && enemyManager.firstStrikeTimer <= 0 && conditionList[0].condition == SpecialCondition.conditionType.先制型) 
            {
                enemyManager.isFirstStrike = true;
            }
            return pursueState;
        }
        HandleRotateTowardsTarger(enemyManager); //保持面对目标的朝向
        return this;
    }
    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //快速转向
    {
        Vector3 direction = enemyManager.curTarget.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        if (direction == Vector3.zero)
        {
            direction = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed);
    }
    private void DecideCirclingAction(EnemyManager enemyManager, EnemyAnimatorManager enemyAnimator)
    {
        WalkAroundTarget(enemyManager, enemyAnimator);
    }
    private void WalkAroundTarget(EnemyManager enemyManager, EnemyAnimatorManager enemyAnimator) //每次进入设置2s的移动时间, 然后根据情况去转变移动时的策略
    {
        walkingTimer = defaultWalkingTimer;
        float defendRandomNum = Random.Range(0.001f, 1);
        EnemyStats enemyStats = enemyManager.GetComponent<EnemyStats>();
        float defendProbility = (1- ((float)enemyStats.currHealth / (float)enemyStats.maxHealth)) * 0.75f * enemyManager.defensiveRatio;

        if (defendProbility >= 1)
        {
            defendProbility = 0.95f;
        }
        //无防御踱步
        if (enemyManager.defPriority<=0 || defendRandomNum >= defendProbility)
        {
            if (enemyManager.stayedRangeEnemy) return;
            if (attackingAdjustment)
            {
                verticalMovementVaule = 0.5f;

                horizontalMovementVaule = Random.Range(-1, 1);
                if (horizontalMovementVaule <= 1 && horizontalMovementVaule >= 0)
                {
                    horizontalMovementVaule = 0.5f;
                }
                else if (horizontalMovementVaule >= -1 && horizontalMovementVaule < 0)
                {
                    horizontalMovementVaule = -0.5f;
                }
            }
            else
            {
                if (!notFirstWalking)
                {
                    //需要精修距离与最大攻击距离间的关系
                    if (distanceFromTarget >= 0 && distanceFromTarget <= enemyManager.minCombatRange)
                    {
                        verticalMovementVaule = -0.5f;
                    }
                    else if (distanceFromTarget > enemyManager.minCombatRange && distanceFromTarget < enemyManager.maxCombatRange)
                    {
                        verticalMovementVaule = 0;
                    }
                    else if (distanceFromTarget > enemyManager.maxCombatRange)
                    {
                        verticalMovementVaule = 0.5f;
                    }

                    horizontalMovementVaule = Random.Range(-1, 1);
                    if (horizontalMovementVaule <= 1 && horizontalMovementVaule >= 0)
                    {
                        horizontalMovementVaule = 0.5f;
                    }
                    else if (horizontalMovementVaule >= -1 && horizontalMovementVaule < 0)
                    {
                        horizontalMovementVaule = -0.5f;
                    }
                }
                else
                {
                    float randomNum = Random.Range(0.001f, 1f);
                    if (randomNum <= 0.15f && !isWalkingStop)
                    {
                        horizontalMovementVaule = 0f;
                        verticalMovementVaule = 0f;
                        isWalkingStop = true;
                        walkingTimer = 1f;
                    }
                    else
                    {
                        if (verticalMovementVaule < 0f)
                        {
                            if (distanceFromTarget > enemyManager.minCombatRange)
                            {
                                verticalMovementVaule = 0f;
                            }
                        }
                        else if (verticalMovementVaule > 0f)
                        {
                            if (distanceFromTarget <= enemyManager.maxCombatRange)
                            {
                                verticalMovementVaule = 0f;
                            }
                        }
                        else if (verticalMovementVaule == 0f)
                        {
                            if (distanceFromTarget < enemyManager.minCombatRange)
                            {
                                verticalMovementVaule = -0.5f;
                            }
                            else if (distanceFromTarget <= enemyManager.maxCombatRange)
                            {
                                verticalMovementVaule = 0.5f;
                            }
                        }

                        horizontalMovementVaule = Random.Range(-1, 1);
                        if (horizontalMovementVaule <= 1 && horizontalMovementVaule >= 0)
                        {
                            horizontalMovementVaule = 0.5f;
                        }
                        else if (horizontalMovementVaule >= -1 && horizontalMovementVaule < 0)
                        {
                            horizontalMovementVaule = -0.5f;
                        }

                        isWalkingStop = false;
                    }
                }
            }
        }
        //防御踱步
        else if (enemyManager.defPriority>0 && defendRandomNum < defendProbility && !enemyManager.isTaijied)
        {
            walkingTimer += 1f;
            if (!notFirstWalking)
            {
                //需要精修距离与最大攻击距离间的关系
                if (distanceFromTarget >= 0 && distanceFromTarget <= enemyManager.minCombatRange)
                {
                    verticalMovementVaule = -0.5f;
                }
                else if (distanceFromTarget > enemyManager.minCombatRange && distanceFromTarget < enemyManager.maxCombatRange)
                {
                    verticalMovementVaule = 0;
                }
                else if (distanceFromTarget > enemyManager.maxCombatRange)
                {
                    verticalMovementVaule = 0.5f;
                }

                horizontalMovementVaule = Random.Range(-1, 1);
                if (horizontalMovementVaule <= 1 && horizontalMovementVaule >= 0)
                {
                    horizontalMovementVaule = 2.5f;
                }
                else if (horizontalMovementVaule >= -1 && horizontalMovementVaule < 0)
                {
                    horizontalMovementVaule = 1.5f;
                }
            }
            else
            {
                float randomNum = Random.Range(0.001f, 1f);
                if (randomNum <= 0.8f && !isWalkingStop)
                {
                    horizontalMovementVaule = 2f;
                    verticalMovementVaule = 0f;
                    isWalkingStop = true;
                    walkingTimer = 1.5f;
                }
                else 
                {
                    if (verticalMovementVaule < 0f)
                    {
                        if (distanceFromTarget > enemyManager.minCombatRange)
                        {
                            verticalMovementVaule = 0f;
                        }
                    }
                    else if (verticalMovementVaule > 0f)
                    {
                        if (distanceFromTarget <= enemyManager.maxCombatRange)
                        {
                            verticalMovementVaule = 0f;
                        }
                    }
                    else if (verticalMovementVaule == 0f)
                    {
                        if (distanceFromTarget > enemyManager.minCombatRange)
                        {
                            verticalMovementVaule = 0.5f;
                        }
                        else if (distanceFromTarget <= enemyManager.minCombatRange)
                        {
                            verticalMovementVaule = -0.5f;
                        }
                    }

                    horizontalMovementVaule = Random.Range(-1, 1);
                    if (horizontalMovementVaule <= 1 && horizontalMovementVaule >= 0)
                    {
                        horizontalMovementVaule = 2.5f;
                    }
                    else if (horizontalMovementVaule >= -1 && horizontalMovementVaule < 0)
                    {
                        horizontalMovementVaule = 1.5f;
                    }
                    isWalkingStop = false;
                }
            }
        }
    }
    private void ReadyToAttack(EnemyManager enemyManager) //仿佛用不到
    {
        if (horizontalMovementVaule < 1) //普通走路
        {
            horizontalMovementVaule = 0f;
        }
        else //防御状态
        {
            horizontalMovementVaule = 2f;
        }
        verticalMovementVaule = 0f;
    }
    private void GetNewAttack(EnemyManager enemyManager) //根据距离和位置主动决策攻击(会进行权重测试)
    {
        Vector3 targetDirection = enemyManager.curTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, transform.position);
        int maxScore = 0;
        for (int i = 0; i < enemyAttacks.Length; i++) //算总权重
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if (distanceFromTarget <= enemyAttackAction.maxDistanceNeedToAttack && distanceFromTarget >= enemyAttackAction.minDistanceNeedToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maxAttackAngle && viewableAngle >= enemyAttackAction.minAttackAngle)
                {
                    if (combatCooldownManager.regularAttackCooldownTimer[i] <= 0) //只算cd转好的攻击
                    {
                        maxScore += enemyAttackAction.attakPriority;
                    }
                }
            }
        }
        int randomValue = Random.Range(0, maxScore);
        int tempScore = 0;
        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if (distanceFromTarget <= enemyAttackAction.maxDistanceNeedToAttack && distanceFromTarget >= enemyAttackAction.minDistanceNeedToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maxAttackAngle && viewableAngle >= enemyAttackAction.minAttackAngle)
                {
                    if (combatCooldownManager.regularAttackCooldownTimer[i] <= 0) //只算cd转好的攻击
                    {
                        if (attackState.curAttack != null)
                            return;

                        tempScore += enemyAttackAction.attakPriority;

                        if (tempScore > randomValue)
                        {
                            attackState.curAttack = enemyAttackAction;
                            attackState.curRegularIndex = i;
                        }
                    }
                }
            }
        }
    }
    void DamageTakenWindow(EnemyManager enemyManager, EnemyAnimatorManager enemyAnimatorManager) 
    {
        float dodgeProbility = 0;
        float defendProbility = 0;
        float rollAttackProbility = 0;
        //受击后行动变化
        if (enemyManager.isBlocking)
        {
            enemyManager.isBlocking = false;
            enemyManager.curRecoveryTime += 0.75f;
            horizontalMovementVaule = 2f;
            verticalMovementVaule = 0f;
            isWalkingStop = true;
            walkingTimer = 1f;
        }
        else if (enemyManager.isDamaged && !enemyManager.getingExecute)
        {
            EnemyStats enemyStats = enemyManager.GetComponent<EnemyStats>();
            PlayerStats playerStats = enemyManager.curTarget.GetComponent<PlayerStats>();
            if (enemyManager.dodgePriority>0 && enemyManager.defPriority>0)
            {
                dodgeProbility = (1- (float)enemyStats.currHealth / (float)enemyStats.maxHealth) * enemyManager.dodgePriority;
                defendProbility = (1- (float)enemyStats.currHealth / (float)enemyStats.maxHealth) * enemyManager.defPriority;
            }
            else if (enemyManager.dodgePriority > 0 && enemyManager.defPriority <= 0) 
            {
                dodgeProbility = (1 - (float)enemyStats.currHealth / (float)enemyStats.maxHealth) * enemyManager.dodgePriority;
            }
            else if (enemyManager.dodgePriority <= 0 && enemyManager.defPriority > 0)
            {
                defendProbility = (1 - (float)enemyStats.currHealth / (float)enemyStats.maxHealth) * enemyManager.defPriority;
            }
            if (enemyManager.rollAtkPriority > 0) 
            {
                rollAttackProbility = (1 - (float)enemyStats.currHealth / (float)enemyStats.maxHealth) * enemyManager.rollAtkPriority;
                //rollAttackProbility = (1 - (float)playerStats.currHealth / (float)playerStats.maxHealth) * enemyManager.rollAtkPriority;
            }
            float TriggerProbility = Random.Range(0.001f,1f);
            float DamageTakenRandomNum = Random.Range(0.001f, dodgeProbility + defendProbility + rollAttackProbility);
            if (!enemyManager.isInteracting && !enemyManager.isTaijied) 
            {
                if (TriggerProbility <= dodgeProbility + defendProbility + rollAttackProbility)
                {
                    if (DamageTakenRandomNum > 0 && DamageTakenRandomNum <= dodgeProbility) //躲避
                    {
                        if (enemyManager.curTarget.GetComponent<PlayerManager>().cantBeInterrupted && distanceFromTarget <= enemyManager.minCombatRange)
                        {
                            enemyManager.isDamaged = false;
                            enemyManager.curRecoveryTime = 0.75f;
                            enemyAnimatorManager.animator.SetBool("isDodging", true);
                            enemyAnimatorManager.PlayTargetAnimation("AttackDodge", true, true);
                            isWalkingStop = false;
                            walkingTimer = 0.25f;
                        }
                    }
                    else if (DamageTakenRandomNum > dodgeProbility && DamageTakenRandomNum <= dodgeProbility + defendProbility) //格挡
                    {
                        enemyManager.isDamaged = false;
                        //积累到一定程度后, 切换到格挡
                        enemyManager.curRecoveryTime += 0.75f;
                        horizontalMovementVaule = 2f;
                        verticalMovementVaule = 0f;
                        isWalkingStop = true;
                        walkingTimer = 1.5f;
                    }
                    else if (DamageTakenRandomNum > dodgeProbility + defendProbility && DamageTakenRandomNum <= dodgeProbility + defendProbility + rollAttackProbility) //滚击
                    {
                        if (enemyManager.curTarget.GetComponent<PlayerManager>().cantBeInterrupted && distanceFromTarget <= enemyManager.maxCombatRange)
                        {
                            enemyManager.isDamaged = false;
                            enemyAnimatorManager.animator.SetBool("isDodging", true);
                            enemyAnimatorManager.PlayTargetAnimation("Roll", true, true);
                            enemyManager.curRecoveryTime = 0.25f;
                        }
                    }
                }
                //else 
                //{
                //    isDamaged = false;
                //}
            }
        }
    }
    void SpecialActionWatcher(EnemyManager enemyManager) 
    {
        if (conditionList != null && enemyManager.curRecoveryTime<=0.5f && !enemyManager.isTaijied) 
        {
            foreach (SpecialCondition specialCondition in conditionList) 
            {
                int index = conditionList.IndexOf(specialCondition);
                if (combatCooldownManager.specialAttackCooldownTimer[index] <= 0)
                {
                    if (specialCondition.condition == SpecialCondition.conditionType.距离型)
                    {
                        if (distanceFromTarget >= specialCondition.minRange && distanceFromTarget <= specialCondition.maxRange && enemyManager.curRecoveryTime>=specialCondition.requiredRecoveryTime)
                        {
                            attackState.curSpecialIndex = index;
                            attackState.curAttack = specialCondition;
                            specialConditionTriggered = true;
                        }
                    }
                    else if (specialCondition.condition == SpecialCondition.conditionType.玩家攻击型)
                    {
                        if (randomDestinationSet && enemyManager.curTarget.GetComponent<PlayerManager>().isAttacking && canCounterAttack)
                        {
                            enemyManager.GetComponentInChildren<Animator>().SetBool("isDoding", specialCondition.canDodge);
                            canCounterAttack = false;
                            enemyManager.isDamaged = false;
                            attackState.curSpecialIndex = index;
                            attackState.curAttack = specialCondition;
                            specialConditionTriggered = true;
                            enemyManager.curRecoveryTime = 0.25f;
                        }
                        if (randomDestinationSet && enemyManager.curTarget.GetComponent<PlayerManager>().isAttacking && distanceFromTarget <= 2.5f && !enemyManager.isParrying && enemyManager.curTarget.GetComponent<PlayerManager>().cantBeInterrupted)
                        {
                            enemyManager.GetComponentInChildren<Animator>().SetBool("isDoding", specialCondition.canDodge);
                            canCounterAttack = false;
                            enemyManager.isDamaged = false;
                            attackState.curSpecialIndex = index;
                            attackState.curAttack = specialCondition;
                            specialConditionTriggered = true;
                            enemyManager.curRecoveryTime = 0.25f;
                        }
                    }
                    else if (specialCondition.condition == SpecialCondition.conditionType.玩家蓄力硬直型) //玩家在aiming holding的时候
                    {
                        if (randomDestinationSet && enemyManager.curTarget.GetComponent<PlayerManager>().isHolding && enemyManager.curTarget.GetComponent<PlayerInventory>().curEquippedWeaponItem.Id == 2) //玩家正在防御时
                        {
                            attackState.curSpecialIndex = index;
                            attackState.curAttack = specialCondition;
                            specialConditionTriggered = true;
                        }
                    }
                    else if (specialCondition.condition == SpecialCondition.conditionType.玩家防御型)
                    {
                        if (randomDestinationSet && enemyManager.curTarget.GetComponent<PlayerManager>().isDefending) //玩家正在防御时
                        {
                            attackState.curSpecialIndex = index;
                            attackState.curAttack = specialCondition;
                            specialConditionTriggered = true;
                        }
                    }
                    else if (specialCondition.condition == SpecialCondition.conditionType.飞行道具型)
                    {
                    }
                    else if (specialCondition.condition == SpecialCondition.conditionType.先制型) 
                    {
                        if (distanceFromTarget <= specialCondition.firstStrikeDistance && enemyManager.isFirstStrike && enemyManager.isEquipped) 
                        {
                            attackState.curSpecialIndex = index;
                            attackState.curAttack = specialCondition;
                            specialConditionTriggered = true;
                            enemyManager.firstStrikeTimer = enemyManager.defaultFirstStrikeTime;
                        }
                    }
                    else if (specialCondition.condition == SpecialCondition.conditionType.处决型)
                    {
                        if (enemyManager.curTarget.GetComponent<PlayerManager>().isStunned && enemyManager.canExecute) //当玩家处于晕眩状态, 且处决trigger内有玩家单位
                        {
                            attackState.isExecution = true;
                            attackState.curSpecialIndex = index;
                            attackState.curAttack = specialCondition;
                            specialConditionTriggered = true;
                            //加一个额外的bool值让attackState知道，然后开始攻击时让玩家同时播放被处决动画, 玩家位置位移至boss的被处决位置
                            //同时播放处决音效
                        }
                    }
                }
            }
        }
    }
}

