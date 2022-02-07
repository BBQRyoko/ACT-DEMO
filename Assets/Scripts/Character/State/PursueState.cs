using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueState : State
{
    public IdleState idleState;
    public CombatStanceState combatStanceState;
    public RotateTowardsTargetState rotateTowardsTargetState;

    public float distanceFromTarget;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        Vector3 targetDirection = enemyManager.curTarget.transform.position - enemyManager.transform.position;
        distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);
        HandleRotateTowardsTarger(enemyManager);


        //问题应该在这块, 应该用不到, 动画转身只用在CombatState
        //if (viewableAngle > 80 || viewableAngle < -80)
        //{
        //    return rotateTowardsTargetState;
        //}

        if (enemyManager.isInteracting)
            return this;

        if (enemyManager.isPreformingAction) 
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0,0.1f, Time.deltaTime);
            return this;
        }

        //这里之后改成isSprint, 通过其它地方来改变是否处于sprint状态来切换
        if (enemyManager.isFirstStrike)
        {
            if (distanceFromTarget > 6f)
            {
                enemyAnimatorManager.animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
                enemyAnimatorManager.animator.SetFloat("Vertical", 2f, 0.1f, Time.deltaTime);   //朝着目标单位进行移动
            }
        }
        else
        {
            if (distanceFromTarget > enemyManager.maxCombatRange)
            {
                enemyAnimatorManager.animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
                enemyAnimatorManager.animator.SetFloat("Vertical", 1f, 0.1f, Time.deltaTime);
            }

        }

        //貌似用不到
        //enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
        //enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;

        //一会儿根据距离和攻击再改
        if (!enemyManager.isFirstStrike)
        {
            if (distanceFromTarget <= enemyManager.maxCombatRange) //追到目标
            {
                return combatStanceState;
            }
            else if (distanceFromTarget >= enemyManager.pursueMaxDistance) //丢失目标, 回到待机态
            {
                enemyAnimatorManager.PlayTargetAnimation("Unarm", true, true);
                enemyManager.curTarget = null;
                return idleState;
            }
            else //持续追击
            {
                return this;
            }
        }
        else 
        {
            if (distanceFromTarget <= combatStanceState.conditionList[0].firstStrikeDistance) //完成首次攻击的要求
            {
                return combatStanceState;
            }
            else
            {
                return this;
            }
        }
    }

    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //转向目标玩家单位(瞬间转向, 无转向动画)
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
