using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueState : State
{
    public IdleState idleState;
    public CombatStanceState combatStanceState;
    public RotateTowardsTargetState rotateTowardsTargetState;

    [SerializeField] float maxPursueDistance;
    public float distanceFromTarget;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        Vector3 targetDirection = enemyManager.curTarget.transform.position - enemyManager.transform.position;
        distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);
        HandleRotateTowardsTarger(enemyManager);

        if (enemyManager.isInteracting)
            return this;

        if (enemyManager.isPreformingAction)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            enemyManager.navMeshAgent.enabled = false;
            return this;
        }

        if (!idleState.announcedByOther)   //确定是否为第一发现者
        {
            maxPursueDistance = enemyManager.pursueMaxDistance;
        }
        else
        {
            maxPursueDistance = enemyManager.announcedPursueDistance;
        }


        if (distanceFromTarget >= maxPursueDistance) //先判断是否在追击距离内, 不在就收武器会Idle
        {
            enemyAnimatorManager.PlayTargetAnimation("Unarm", true, true);
            enemyManager.curTarget = null;
            return idleState;
        }
        else if (distanceFromTarget < maxPursueDistance)
        {
            if (enemyManager.isFirstStrike) //在范围内就判断是否有先制攻击 
            {
                if (distanceFromTarget > combatStanceState.conditionList[0].firstStrikeDistance) //有先制且距离大于先制范围时
                {
                    enemyAnimatorManager.animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
                    enemyAnimatorManager.animator.SetFloat("Vertical", 2f, 0.1f, Time.deltaTime);   //朝着目标单位进行疾跑
                }
                else //进入范围就直接接战并释放先制攻击
                {
                    return combatStanceState;
                }
            }
            else //如果没有先制攻击
            {
                if (distanceFromTarget > enemyManager.maxCombatRange) //如果范围大于最大接战距离就慢跑过去
                {
                    enemyAnimatorManager.animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
                    enemyAnimatorManager.animator.SetFloat("Vertical", 1f, 0.1f, Time.deltaTime);
                }
                else //进入范围后就接战并开始踱步
                {
                    combatStanceState.walkingTimer = 0.5f;
                    return combatStanceState;
                }
            }
        }

        enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
        enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;
        return this;
    }

    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //转向目标玩家单位(瞬间转向, 无转向动画)
    {
        if (enemyManager.isPreformingAction)
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
        else 
        {
            Vector3 relativeDirection = transform.TransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRig.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.curTarget.transform.position);
            enemyManager.enemyRig.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed);
        }
    }
}
