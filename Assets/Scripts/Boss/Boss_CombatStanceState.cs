using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_CombatStanceState : State
{
    public Boss_AttackState boss_AttackState;
    public Boss_PursueState boss_PursueState;

    public EnemyAttackAction[] enemyAttacks;

    //bool randomDestinationSet = false;
    float verticalMovementVaule = 0;
    float horizontalMovementVaule = 0;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        //确认方向, 目标距离, 目标方位
        Vector3 targetDirection = enemyManager.curTarget.transform.position - enemyManager.transform.position;
        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

        enemyAnimatorManager.animator.SetFloat("Vertical", verticalMovementVaule, 0.2f, Time.deltaTime);
        enemyAnimatorManager.animator.SetFloat("Horizontal", horizontalMovementVaule, 0.2f, Time.deltaTime);
        boss_AttackState.hasPerformedAttack = false;

        if (enemyManager.isInteracting)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0);
            enemyAnimatorManager.animator.SetFloat("Horizontal", 0);
            return this;
        }

        if (distanceFromTarget <= enemyManager.maxAttackRange)
        {
            verticalMovementVaule = 0;

            if (viewableAngle >= 110 && viewableAngle <= 180 && !enemyManager.isInteracting)
            {
                Debug.Log("在背后有效范围, 可以接扫");
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Attack(3)", true);
                //HandleRotateTowardsTarger(enemyManager);
                return this;
            }
            else if (viewableAngle <= -110 && viewableAngle >= -180 && !enemyManager.isInteracting)
            {
                Debug.Log("在背后有效范围, 可以接扫");
                enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Attack(3)", true);
                //HandleRotateTowardsTarger(enemyManager);
                return this;
            }
        }


        if (distanceFromTarget > 6f)
        {
            HandleRotateTowardsTarger(enemyManager);
            return boss_PursueState; //距离大于攻击范围后退回追踪状态
        }
        else if (enemyManager.curTargetDistance == 1 && distanceFromTarget > enemyManager.maxAttackRange)
        {
            HandleRotateTowardsTarger(enemyManager);
            verticalMovementVaule = 0.5f;
        }

        if (enemyManager.curTargetDistance == 1 && !enemyManager.shouted)
        {
            HandleRotateTowardsTarger(enemyManager);
            enemyAnimatorManager.PlayTargetAnimation("Shout", true);
            enemyManager.shoutTimer = 15f;
            enemyManager.shouted = true;
            return this;
        }

        HandleRotateTowardsTarger(enemyManager); //保持面对目标的朝向

        if (enemyManager.curRecoveryTime <= 0 && boss_AttackState.curAttack != null && distanceFromTarget < enemyManager.maxAttackRange)
        {
            return boss_AttackState; //距离小于攻击范围且攻击间隔完成后进入攻击状态
        }
        else
        {
            GetNewAttack(enemyManager);
            return this;
        }
    }

    public void HandleRotateTowardsTarger(EnemyManager enemyManager)
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
        
        //else
        //{
        //    Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
        //    Vector3 targetVelocity = enemyManager.enemyRig.velocity;

        //    enemyManager.navMeshAgent.enabled = true;
        //    enemyManager.navMeshAgent.SetDestination(enemyManager.curTarget.transform.position);
        //    enemyManager.enemyRig.velocity = targetVelocity;
        //    enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        //}
    } //控制单位的朝向始终对着目标

    private void GetNewAttack(EnemyManager enemyManager) //攻击从设置好的攻击列表中随机挑选下一次的攻击动画(近战)
    {
        Vector3 targetDirection = enemyManager.curTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, transform.position);

        int maxScore = 0;

        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];

            if (distanceFromTarget <= enemyAttackAction.maxDistanceNeedToAttack && distanceFromTarget >= enemyAttackAction.minDistanceNeedToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maxAttackAngle && viewableAngle >= enemyAttackAction.minAttackAngle)
                {
                    maxScore += enemyAttackAction.attackScore;
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
                    if (boss_AttackState.curAttack != null)
                        return;

                    tempScore += enemyAttackAction.attackScore;

                    if (tempScore > randomValue)
                    {
                        boss_AttackState.curAttack = enemyAttackAction;
                    }
                }
            }
        }
    }

}
