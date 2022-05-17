using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public PursueState pursueState;
    public LayerMask detectionLayer;
    public LayerMask blockingLayer;

    [SerializeField] Collider[] colliders;
    public bool announcedByOther;
    public float announceDistance = 15f;
    [SerializeField] AnnounceSound announcePrefab;

    [Header("待机专属")]
    [SerializeField] float defaultRotatePeriod = 3.5f;
    [SerializeField] float rotateTimer;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        if (enemyManager.idleType == EnemyManager.IdleType.Stay || enemyManager.idleType == EnemyManager.IdleType.Boss) //站岗的敌人
        {
            if (!enemyManager.ambushEnemy)
            {
                if (!enemyManager.isEquipped)
                {
                    enemyAnimatorManager.animator.SetFloat("Horizontal", -2, 0.1f, Time.deltaTime);
                }
                else 
                {
                    enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                }
            }
            else 
            {
                enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
            }

            if (enemyManager.isPreformingAction)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            Vector3 targetDirection = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.patrolPos[enemyManager.curPatrolIndex].position, enemyManager.transform.position);

            HandleRotateTowardsTarger(enemyManager);

            if (distanceFromTarget > 1f)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 1f, 0.1f, Time.deltaTime);   //跑回初始点
            }
            else
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);   //站着idle状态
                enemyManager.EnemyReset();
            }

            enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
        }
        else if(enemyManager.idleType == EnemyManager.IdleType.Patrol) //巡逻的敌人
        {
            if (!enemyManager.ambushEnemy)
            {
                if (!enemyManager.isEquipped)
                {
                    enemyAnimatorManager.animator.SetFloat("Horizontal", -2, 0.1f, Time.deltaTime);
                }
                else
                {
                    enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                }
            }
            else
            {
                enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
            }

            if (enemyManager.isPreformingAction)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            Vector3 targetDirection = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.patrolPos[enemyManager.curPatrolIndex].position, enemyManager.transform.position);

            if (distanceFromTarget > 0.5f)
            {
                if (!enemyManager.alertingTarget)
                {
                    enemyAnimatorManager.animator.SetFloat("Vertical", 0.5f, 0.1f, Time.deltaTime);   //朝着目标单位进行移动
                }
                else 
                {
                    enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);   //站着idle状态
                }
            }
            else if (distanceFromTarget <= 0.5f)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);   //站着idle状态

                enemyManager.curPatrolIndex = enemyManager.curPatrolIndex + 1;
                if (enemyManager.curPatrolIndex >= enemyManager.patrolPos.Count) 
                {
                    enemyManager.curPatrolIndex = 0;
                }
            }

            HandleRotateTowardsTarger(enemyManager);
            enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        #region 敌人的预警范围设置
        Collider[] alertCollider = Physics.OverlapSphere(enemyManager.transform.position, enemyManager.alertRadius, detectionLayer);
        for (int i = 0; i < alertCollider.Length; i++)
        {
            CharacterStats characterStats = alertCollider[i].transform.GetComponent<CharacterStats>();
            Vector3 targetDir = new Vector3(characterStats.eyePos.position.x - enemyStats.eyePos.transform.position.x, characterStats.eyePos.position.y - enemyStats.eyePos.transform.position.y, characterStats.eyePos.position.z - enemyStats.eyePos.transform.position.z);
            float distance = Vector3.Distance(enemyStats.eyePos.transform.position, characterStats.eyePos.position);
            bool hitInfo = Physics.Raycast(enemyStats.eyePos.position, targetDir, distance, blockingLayer);

            if (characterStats != null && characterStats.currHealth > 0 && !hitInfo && !characterStats.GetComponent<PlayerManager>().isInGrass)
            {
                //Check Character ID
                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
                float crouchFactor = 1;

                if (characterStats.GetComponent<PlayerManager>().isCrouching)
                {
                    crouchFactor = 0.5f;
                }
                else 
                {
                    crouchFactor = 1f;
                }
                float alertIncreaseRate = 0.5f * ((enemyManager.alertRadius + 1.5f) - distance) * crouchFactor;
                if (viewableAngle > enemyManager.minDetectionAngle && viewableAngle < enemyManager.maxDetectionAngle)
                {
                    if (characterStats.GetComponent<PlayerManager>()) 
                    {
                        enemyManager.alertingTarget = characterStats;
                    }
                    if (enemyManager.alertTimer <= 5)
                    {
                        enemyManager.alertTimer += alertIncreaseRate*Time.deltaTime;
                    }
                    else
                    {
                        enemyManager.curTarget = characterStats;
                        enemyManager.alertingTarget = null;
                    }
                }
                else
                {
                    if (!enemyManager.isAlerting)
                    {
                        if (enemyManager.alertTimer > 0)
                        {
                            enemyManager.alertTimer -= 0.5f * Time.deltaTime;
                        }
                        else
                        {
                            enemyManager.alertTimer = 0;
                        }
                    }
                }
            }
            else
            {
                if (!enemyManager.isAlerting)
                {
                    if (enemyManager.alertTimer > 0)
                    {
                        enemyManager.alertTimer -= 0.5f * Time.deltaTime;
                    }
                    else
                    {
                        enemyManager.alertTimer = 0;
                    }
                }
            }
        }
        if (alertCollider.Length <= 0)
        {
            if (!enemyManager.isAlerting)
            {
                if (enemyManager.alertTimer > 0)
                {
                    enemyManager.alertTimer -= 0.5f * Time.deltaTime;
                }
                else
                {
                    enemyManager.alertTimer = 0;
                }
            }
        }
        #endregion

        #region 敌人的直接侦测范围设置
        colliders = Physics.OverlapSphere(enemyManager.transform.position, enemyManager.detectionRadius, detectionLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();
            Vector3 targetDir = new Vector3(characterStats.eyePos.position.x - enemyStats.eyePos.transform.position.x, characterStats.eyePos.position.y - enemyStats.eyePos.transform.position.y, characterStats.eyePos.position.z - enemyStats.eyePos.transform.position.z);
            float distance = Vector3.Distance(enemyStats.eyePos.transform.position, characterStats.eyePos.position);
            bool hitInfo = Physics.Raycast(enemyStats.eyePos.position, targetDir, distance, blockingLayer);
            if (characterStats != null && characterStats.currHealth > 0 && !hitInfo && !characterStats.GetComponent<PlayerManager>().isInGrass)
            {
                //Check Character ID
                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minDetectionAngle && viewableAngle < enemyManager.maxDetectionAngle)
                {
                    enemyManager.curTarget = characterStats;
                }
                else if (distance <= 3f && !characterStats.GetComponent<PlayerManager>().isCrouching && (characterStats.GetComponent<PlayerLocmotion>().movementVelocity.x != 0 || characterStats.GetComponent<PlayerLocmotion>().movementVelocity.z != 0)) 
                {
                    enemyManager.curTarget = characterStats;
                }
            }
        }
        #endregion

        if (enemyManager.alertingTarget != null)
        {
            if (!enemyManager.isEquipped && !enemyManager.ambushEnemy && !enemyManager.isNoWeapon) //没装备武器就把武器装上
            {
                enemyAnimatorManager.PlayTargetAnimation("Equip", true, true);
            }
        }
        else 
        {
            if (enemyManager.curTarget == null && enemyManager.isEquipped && !enemyManager.ambushEnemy && !enemyManager.isNoWeapon) 
            {
                enemyAnimatorManager.PlayTargetAnimation("Unarm", true, true);
            }
        }

        #region 切换至追踪模式
        if (enemyManager.curTarget != null)
        {
            if (!enemyManager.isEquipped && !enemyManager.isNoWeapon) //没装备武器就把武器装上
            {
                enemyAnimatorManager.PlayTargetAnimation("Equip", true, true);
            }
            if (!announcedByOther) //Option.1: 只要不是被叫的就叫人
            {
                PlayerNoticeAnnounce(announceDistance);
            }
            enemyManager.isAlerting = false;
            enemyManager.alertingTarget = null;
            enemyManager.alertTimer = 0;
            enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
            return pursueState; //当发现目标后, 进入追踪模式
        }
        else 
        {
            return this;
        }
        #endregion
    }
    public void PlayerNoticeAnnounce(float maxDistance, bool executionCall = false) 
    {
        EnemyManager enemyManager = transform.GetComponentInParent<EnemyManager>();
        EnemyAnimatorManager enemyAnimatorManager = enemyManager.GetComponentInChildren<EnemyAnimatorManager>();

        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);

        if (executionCall)
        {
            announcePrefab.gameObject.SetActive(true);
            announcePrefab.announceSoundDistance = maxDistance;
            announcePrefab.isExecutionCall = executionCall;
        }
        else 
        {
            if (enemyManager.canAlertOthers && distanceFromTarget<= enemyManager.detectionRadius) 
            {
                announcePrefab.gameObject.SetActive(true);
                announcePrefab.announceSoundDistance = maxDistance;
                announcePrefab.isExecutionCall = executionCall;
                enemyAnimatorManager.attackAudio.volume = 0.15f;
                enemyAnimatorManager.attackAudio.clip = enemyAnimatorManager.sample_SFX.EnemyCallingSFX;
                enemyAnimatorManager.attackAudio.Play();
            }
        }
    }
    public void AnnouncedByOtherEnemy(EnemyStats announcingEnemy, bool executionCall = false) 
    {
        EnemyManager announcingEnemyManager = announcingEnemy.GetComponent<EnemyManager>();
        EnemyStats selfEnemyStats = transform.GetComponentInParent<EnemyStats>();
        EnemyManager selfEnemyManager = transform.GetComponentInParent<EnemyManager>();

        Vector3 targetDir = new Vector3(announcingEnemy.eyePos.position.x - selfEnemyStats.eyePos.transform.position.x, announcingEnemy.eyePos.position.y - selfEnemyStats.eyePos.transform.position.y, announcingEnemy.eyePos.position.z - selfEnemyStats.eyePos.transform.position.z);
        float distance = Vector3.Distance(selfEnemyStats.eyePos.transform.position, announcingEnemy.eyePos.position);
        bool hitInfo = Physics.Raycast(selfEnemyStats.eyePos.position, targetDir, distance, blockingLayer);

        if (!executionCall)
        {
            if (!hitInfo) //没直接看到呼喊的敌人时
            {
                announcedByOther = true;
                selfEnemyManager.curTarget = announcingEnemyManager.curTarget;
            }
            else //但如果看到玩家的话
            {
                CharacterStats playerCharacterStates = announcingEnemyManager.curTarget;
                Vector3 targetDirToPlayer = new Vector3(playerCharacterStates.eyePos.position.x - selfEnemyStats.eyePos.transform.position.x, playerCharacterStates.eyePos.position.y - selfEnemyStats.eyePos.transform.position.y, playerCharacterStates.eyePos.position.z - selfEnemyStats.eyePos.transform.position.z);
                float distanceToPlayer = Vector3.Distance(selfEnemyStats.eyePos.transform.position, playerCharacterStates.eyePos.position);
                bool hitInfoToPlayer = Physics.Raycast(selfEnemyStats.eyePos.position, targetDir, distance, blockingLayer);
                if (!hitInfo) 
                {
                    announcedByOther = true;
                    selfEnemyManager.curTarget = announcingEnemyManager.curTarget;
                }
            }
        }
        else 
        {
            //ExecutionDeadCall
            selfEnemyManager.isAlerting = true;
            selfEnemyManager.alertingTarget = announcingEnemy;
        }
    }
    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //朝着设置的目标点进行移动
    {
        if (!enemyManager.alertingTarget)
        {
            Vector3 direction = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - transform.position;
            direction.y = 0;
            direction.Normalize();

            Vector3 targetDirection = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.patrolPos[enemyManager.curPatrolIndex].position, enemyManager.transform.position);

            Vector3 relativeDirection = transform.TransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRig.velocity;

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            if (enemyManager.idleType == EnemyManager.IdleType.Stay) //站岗类专属的原地旋转
            {
                if (!enemyManager.curTarget && enemyManager.alertTimer <= 0 && distanceFromTarget<=1f) //在非警戒状态才会转
                {
                    if (defaultRotatePeriod != 0) 
                    {
                        if (rotateTimer > 0.1f)
                        {
                            rotateTimer -= Time.deltaTime;
                        }
                        else
                        {
                            enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimationWithRootRotation("Turn180", true);
                            rotateTimer = defaultRotatePeriod;
                        }
                    }
                }
                else
                {
                    if (distanceFromTarget > 5f)
                    {
                        enemyManager.navMeshAgent.enabled = true;
                        enemyManager.navMeshAgent.SetDestination(enemyManager.patrolPos[enemyManager.curPatrolIndex].position);
                        enemyManager.enemyRig.velocity = targetVelocity;
                        enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed);
                    }
                    else
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed);
                    }
                }
            }
            else
            {
                if (distanceFromTarget > 1f)
                {
                    enemyManager.navMeshAgent.enabled = true;
                    enemyManager.navMeshAgent.SetDestination(enemyManager.patrolPos[enemyManager.curPatrolIndex].position);
                    enemyManager.enemyRig.velocity = targetVelocity;
                    enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed);
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed);
                }
            }
        }
        else 
        {
            Vector3 direction = enemyManager.alertingTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}