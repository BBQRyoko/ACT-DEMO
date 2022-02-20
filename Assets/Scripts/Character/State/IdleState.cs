using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public PursueState pursueState;
    public LayerMask detectionLayer;
    public LayerMask hearingLayer;
    public LayerMask blockingLayer;

    [SerializeField] Collider[] colliders;


    [Header("待机专属")]
    [SerializeField] float defaultRotatePeriod = 3.5f;
    float rotateTimer;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        if (enemyManager.idleType == EnemyManager.IdleType.Stay || enemyManager.idleType == EnemyManager.IdleType.Boss) //站岗的敌人
        {
            enemyAnimatorManager.animator.SetFloat("Horizontal", -2, 0.1f, Time.deltaTime);

            if (enemyManager.isPreformingAction)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            Vector3 targetDirection = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.patrolPos[enemyManager.curPatrolIndex].position, enemyManager.transform.position);

            if (distanceFromTarget > 0.5f)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 1f, 0.1f, Time.deltaTime);   //跑回初始点
            }
            else if (distanceFromTarget <= 0.5f)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);   //站着idle状态
            }

            HandleRotateTowardsTarger(enemyManager);

            enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
        }
        else if(enemyManager.idleType == EnemyManager.IdleType.Patrol) //巡逻的敌人
        {
            enemyAnimatorManager.animator.SetFloat("Horizontal", -2, 0.1f, Time.deltaTime);

            if (enemyManager.isPreformingAction)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                return this;
            }

            Vector3 targetDirection = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - enemyManager.transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.patrolPos[enemyManager.curPatrolIndex].position, enemyManager.transform.position);

            if (distanceFromTarget > 0.5f)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0.5f, 0.1f, Time.deltaTime);   //朝着目标单位进行移动
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

        #region 敌人听力影响设置
        Collider[] hearCollider = Physics.OverlapSphere(enemyManager.transform.position, enemyManager.hearRadius, hearingLayer);
        for (int i = 0; i < hearCollider.Length; i++)
        {
            EnemyManager enemyManager1 = hearCollider[i].transform.GetComponent<EnemyManager>();

            if (enemyManager1 != null && enemyManager1.curTarget != null && !enemyManager1.curTarget.GetComponent<PlayerManager>().isDead)
            {
                EnemyStats enemyStats1 = enemyManager1.GetComponent<EnemyStats>();
                Vector3 targetDir = new Vector3(enemyStats1.eyePos.position.x - enemyStats.eyePos.transform.position.x, enemyStats1.eyePos.position.y - enemyStats.eyePos.transform.position.y, enemyStats1.eyePos.position.z - enemyStats.eyePos.transform.position.z);
                float distance = Vector3.Distance(enemyStats.eyePos.transform.position, enemyStats1.eyePos.position);
                bool hitInfo = Physics.Raycast(enemyStats.eyePos.position, targetDir, distance, blockingLayer);
                if (!hitInfo) 
                {
                    enemyManager.curTarget = enemyManager1.curTarget;
                }
            }
        }
        #endregion

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

                if (viewableAngle > enemyManager.minDetectionAngle && viewableAngle < enemyManager.maxDetectionAngle)
                {
                    if (enemyManager.alertTimer < 5)
                    {
                        enemyManager.alertTimer += Time.deltaTime;
                    }
                    else
                    {
                        enemyManager.curTarget = characterStats;
                    }
                }
                else
                {
                    if (enemyManager.alertTimer > 0)
                    {
                        enemyManager.alertTimer -= Time.deltaTime;
                    }
                    else
                    {
                        enemyManager.alertTimer = 0;
                    }
                }
            }
            else
            {
                if (enemyManager.alertTimer > 0)
                {
                    enemyManager.alertTimer -= Time.deltaTime;
                }
                else
                {
                    enemyManager.alertTimer = 0;
                }
            }
        }

        if (alertCollider.Length <= 0) 
        {
            if (enemyManager.alertTimer > 0)
            {
                enemyManager.alertTimer -= Time.deltaTime;
            }
            else
            {
                enemyManager.alertTimer = 0;
            }
        }
        #endregion

        #region 敌人的直接侦测范围设置
        colliders = Physics.OverlapSphere(enemyManager.transform.position, enemyManager.detectionRadius, detectionLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();
            Vector3 targetDir = new Vector3(characterStats.eyePos.position.x - enemyStats.eyePos.transform.position.x, characterStats.eyePos.position.y- enemyStats.eyePos.transform.position.y, characterStats.eyePos.position.z - enemyStats.eyePos.transform.position.z);
            float distance = Vector3.Distance(enemyStats.eyePos.transform.position, characterStats.eyePos.position);
            bool hitInfo = Physics.Raycast(enemyStats.eyePos.position, targetDir, distance, blockingLayer);
            if (characterStats != null && characterStats.currHealth>0 && !hitInfo && !characterStats.GetComponent<PlayerManager>().isInGrass)
            {
                //Check Character ID
                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minDetectionAngle && viewableAngle < enemyManager.maxDetectionAngle)
                {
                    enemyManager.curTarget = characterStats;
                }
            }
        }
        #endregion

        #region 切换至追踪模式
        if (enemyManager.curTarget != null)
        {
            if (!enemyManager.isEquipped) 
            {
                enemyAnimatorManager.PlayTargetAnimation("Equip", true, true);
            }
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
    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //朝着设置的目标点进行移动
    {
        Vector3 direction = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - transform.position;
        direction.y = 0;
        direction.Normalize();


        Vector3 targetDirection = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - enemyManager.transform.position;
        float distanceFromTarget = Vector3.Distance(enemyManager.patrolPos[enemyManager.curPatrolIndex].position, enemyManager.transform.position);

        Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
        Vector3 targetVelocity = enemyManager.enemyRig.velocity;

        if (direction == Vector3.zero)
        {
            direction = transform.forward;
        }

        if (enemyManager.idleType == EnemyManager.IdleType.Stay) //站岗类专属的原地旋转
        {
            if (!enemyManager.curTarget && enemyManager.alertTimer <= 0) //在非警戒状态才会转
            {
                if (rotateTimer > 0)
                {
                    rotateTimer -= Time.deltaTime;
                }
                else
                {
                    enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimationWithRootRotation("Turn180", true);
                    rotateTimer = defaultRotatePeriod;
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
}