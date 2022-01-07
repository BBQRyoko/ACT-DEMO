using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public PursueState pursueState;
    public LayerMask detectionLayer;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        if (enemyManager.idleType == EnemyManager.IdleType.Stay) //原地状态的敌人
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
                enemyAnimatorManager.animator.SetFloat("Vertical", 1f, 0.1f, Time.deltaTime);   //朝着目标单位进行移动
            }
            else if (distanceFromTarget <= 0.5f)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);   //站着idle状态
            }

            HandleRotateTowardsTarger(enemyManager);
            enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
        }
        else if(enemyManager.idleType == EnemyManager.IdleType.Patrol) //巡逻状态的敌人
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

        #region 敌人的可侦测范围设置
        Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, enemyManager.detectionRadius, detectionLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

            if (characterStats != null)
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
            enemyAnimatorManager.PlayTargetAnimation("Equip", true, true);
            enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
            return pursueState; //当发现目标后, 进入追踪模式
        }
        else 
        {
            return this;
        }
        #endregion
    }
    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //移动时保持朝着目标方向
    {

            Vector3 direction = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed);
        
    }
}
