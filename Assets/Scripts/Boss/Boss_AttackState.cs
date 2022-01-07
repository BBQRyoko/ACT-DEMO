using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_AttackState : State
{
    //public Boss_RotateTowardsTargetState boss_RotateTowardsTargetState;
    public Boss_CombatStanceState boss_CombatStanceState;
    //public Boss_PursueState boss_PursueState;
    public EnemyAttackAction curAttack;

    bool willDoComboOnNextAttack = false;
    public bool hasPerformedAttack = false;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        Vector3 targetDirection = enemyManager.curTarget.transform.position - enemyManager.transform.position;
        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        RotateTowardsTargetWhileAttacking(enemyManager);

        enemyManager.comboCount = enemyManager.maxComboCount;

        //if (distanceFromTarget > enemyManager.maxAttackRange)
        //{
        //    return boss_PursueState;
        //}

        if (willDoComboOnNextAttack && enemyManager.canDoCombo)
        {

        }

        if (!hasPerformedAttack)
        {
            RotateTowardsTargetWhileAttacking(enemyManager);
            AttackTarget(enemyAnimatorManager, enemyManager);
        }

        if (willDoComboOnNextAttack && hasPerformedAttack)
        {
            return this;
        }

        return boss_CombatStanceState;
        //return boss_RotateTowardsTargetState;

        //Vector3 targetDirection = enemyManager.curTarget.transform.position - transform.position;
        //float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        //float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

        //HandleRotateTowardsTarger(enemyManager);

        //if (enemyManager.isPreformingAction)
        //    return combatStanceState;

        //if (curAttack != null)
        //{
        //    //If we are too close to the enemy to preform attack, get a new attack
        //    if (distanceFromTarget < curAttack.minDistanceNeedToAttack)
        //    {
        //        return this;
        //    }
        //    else if (distanceFromTarget < curAttack.maxDistanceNeedToAttack)
        //    {
        //        if (viewableAngle <= curAttack.maxAttackAngle && viewableAngle >= curAttack.minAttackAngle)
        //        {
        //            if (enemyManager.curRecoveryTime <= 0 && enemyManager.isPreformingAction == false)
        //            {
        //                //确认是否为霸体状态的攻击
        //                if (curAttack.isImmune)
        //                    enemyManager.isImmuneAttacking = true;
        //                else
        //                    enemyManager.isImmuneAttacking = false;

        //                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
        //                enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);

        //                enemyAnimatorManager.PlayTargetAnimation(curAttack.actionAnimation, true);
        //                if (enemyManager.curEnemyType == EnemyManager.enemyType.range) 
        //                {
        //                    enemyManager.HandleRangeAttack();
        //                }


        //                enemyManager.isPreformingAction = true;
        //                if (distanceFromTarget > curAttack.maxDistanceNeedToAttack)
        //                {
        //                    curAttack = null;
        //                    return combatStanceState;
        //                }
        //                else 
        //                {
        //                    enemyManager.curRecoveryTime = curAttack.recoveryTime;
        //                    curAttack = null;
        //                    return combatStanceState;
        //                }
        //            }
        //        }
        //    }
        //}
        //else 
        //{
        //    GetNewAttack(enemyManager);
        //}

        //return combatStanceState;
    }
    private void AttackTarget(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager)
    {
        enemyAnimatorManager.PlayTargetAnimation(curAttack.actionAnimation, true, true);
        enemyManager.curRecoveryTime = curAttack.recoveryTime;
        hasPerformedAttack = true;
        curAttack = null;
        RotateTowardsTargetWhileAttacking(enemyManager);
    }

    public void RotateTowardsTargetWhileAttacking(EnemyManager enemyManager) //攻击始终朝着目标方向
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
