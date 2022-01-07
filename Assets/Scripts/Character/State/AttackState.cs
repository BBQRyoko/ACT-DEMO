using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public RotateTowardsTargetState rotateTowardsTargetState;
    public CombatStanceState combatStanceState;
    public PursueState pursueState;
    public EnemyAttackAction curAttack;

    public bool hasPerformedAttack = false;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);

        RotateTowardsTargetWhiletAttacking(enemyManager);

        if (distanceFromTarget > enemyManager.maxAttackRange) 
        {
            return pursueState;
        }

        if (!hasPerformedAttack) 
        {
            if (enemyManager.curEnemyType == EnemyManager.enemyType.melee)
            {
                AttackTarget(enemyAnimatorManager, enemyManager);
            }
            else if (enemyManager.curEnemyType == EnemyManager.enemyType.range) 
            {
                enemyAnimatorManager.PlayTargetAnimation("Range_Attack", true);
                enemyManager.curRecoveryTime = enemyManager.rangeRecoveryTime;
                hasPerformedAttack = true;
            }
        }

        return rotateTowardsTargetState;
    }
    private void AttackTarget(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) 
    {
        enemyAnimatorManager.PlayTargetAnimation(curAttack.actionAnimation, true);
        enemyManager.curRecoveryTime = curAttack.recoveryTime;
        enemyManager.isImmuneAttacking = curAttack.isImmune;
        hasPerformedAttack = true;
        curAttack = null;
    }
    public void RotateTowardsTargetWhiletAttacking(EnemyManager enemyManager) //攻击始终朝着目标方向
    {
        if (enemyManager.canRotate && enemyManager.isInteracting)
        {
            Vector3 direction = enemyManager.curTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed/Time.deltaTime);
        }

    }
}
