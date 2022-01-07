using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_PursueState : State
{
    public Boss_IdleState boss_IdleState;
    public Boss_CombatStanceState boss_CombatStanceState;
    //public Boss_RotateTowardsTargetState boss_RotateTowardsTargetState;
    public Boss_ResetState boss_ResetState;

    public float distanceFromTarget;
    public float chaseTimer;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        Vector3 targetDirection = enemyManager.curTarget.transform.position - enemyManager.transform.position;
        distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);
        HandleRotateTowardsTarger(enemyManager);

        //if (viewableAngle > 65 || viewableAngle < -65)
        //    return boss_RotateTowardsTargetState;
        chaseTimer += Time.deltaTime;

        if (enemyManager.isPreformingAction)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            return this;
        }

        if (distanceFromTarget > enemyManager.maxAttackRange)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 1f, 0.1f, Time.deltaTime);   //朝着目标单位进行移动
            if (chaseTimer >= 5f)
            {
                enemyAnimatorManager.PlayTargetAnimation("RangeThrow", true);
                chaseTimer = 0;
                return boss_CombatStanceState;
            }
        }
        else if (distanceFromTarget <= enemyManager.maxAttackRange)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);   //站着idle状态
        }

        enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
        enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;

        if (distanceFromTarget <= enemyManager.maxAttackRange)
        {
            return boss_CombatStanceState;
        }
        else if (distanceFromTarget >= enemyManager.pursueMaxDistance)
        {
            enemyManager.curTarget = null;
            return boss_ResetState;
        }
        else
        {
            return this;
        }

    }

    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //追踪时保持朝着目标方向
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
}
