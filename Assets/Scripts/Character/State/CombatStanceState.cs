using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStanceState : State
{
    public AttackState attackState;
    public PursueState pursueState;
    public EnemyAttackAction[] enemyAttacks;

    bool canCounterAttack;
    float distanceFromTarget;
    public bool randomDestinationSet = false;
    float verticalMovementVaule = 0;
    float horizontalMovementVaule = 0;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        //确认单位与目标间的距离
        distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        enemyAnimatorManager.animator.SetFloat("Vertical", verticalMovementVaule, 0.2f, Time.deltaTime);
        enemyAnimatorManager.animator.SetFloat("Horizontal", horizontalMovementVaule, 0.2f, Time.deltaTime);
        attackState.hasPerformedAttack = false;

        if (enemyManager.isInteracting) 
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0);
            enemyAnimatorManager.animator.SetFloat("Horizontal", 0);
            return this;
        }

        if (distanceFromTarget > enemyManager.maxAttackRange)
        {
            return pursueState; //距离大于攻击范围后退回追踪状态
        }

        if (!randomDestinationSet && !enemyManager.isFirstAttack)
        {
            randomDestinationSet = true;
            DecideCirclingAction(enemyAnimatorManager);
        }

        HandleRotateTowardsTarger(enemyManager); //保持面对目标的朝向

        if (randomDestinationSet && enemyManager.curTarget.GetComponent<PlayerManager>().isAttacking && canCounterAttack) 
        {
            enemyAnimatorManager.PlayTargetAnimation("Step_Back", true, true); //后撤垫步
            enemyManager.isDodging = true;
            canCounterAttack = false;
            randomDestinationSet = false;
            GetNewAttack(enemyManager);
        }
        else if (enemyManager.curRecoveryTime <= 0 && attackState.curAttack != null)
        {
            randomDestinationSet = false;
            return attackState; //距离小于攻击范围且攻击间隔完成后进入攻击状态
        }
        else
        {
            GetNewAttack(enemyManager);
        }

        return this;
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
    } 

    private void DecideCirclingAction(EnemyAnimatorManager enemyAnimator) 
    {
        WalkAroundTarget(enemyAnimator);
    }
    private void WalkAroundTarget(EnemyAnimatorManager enemyAnimator) 
    {
        if ((int)enemyAnimator.GetComponentInParent<EnemyManager>().curEnemyType == 0) //近战敌人
        {
            float randomNum = Random.Range(0, 3);//随机模式, 1/3的概率是纯随机移动
            if (distanceFromTarget <= 1.75)
            {
                verticalMovementVaule = -0.5f;
                if (enemyAnimator.GetComponentInParent<EnemyManager>().dodgeTimer <= 0) 
                {
                    canCounterAttack = true;
                }
            }
            else if (distanceFromTarget > 1.75 && distanceFromTarget < 2.25)
            {
                enemyAnimator.animator.SetTrigger("canCombo");
                verticalMovementVaule = 0;
            }
            else if (distanceFromTarget >= 2.25 && distanceFromTarget < 2.75)
            {
                enemyAnimator.animator.SetTrigger("canCombo");
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
        else if ((int)enemyAnimator.GetComponentInParent<EnemyManager>().curEnemyType == 1) //远程敌人
        {
            float randomNum = Random.Range(0, 3);//随机模式, 1/3的概率是纯随机移动
            if (distanceFromTarget >= 0 && distanceFromTarget <= 3*enemyAnimator.GetComponentInParent<EnemyManager>().maxAttackRange/4)
            {
                verticalMovementVaule = -0.5f;
            }
            else if (distanceFromTarget > 3 * enemyAnimator.GetComponentInParent<EnemyManager>().maxAttackRange / 4 && distanceFromTarget < enemyAnimator.GetComponentInParent<EnemyManager>().maxAttackRange)
            {
                verticalMovementVaule = 0;
            }
            else if (distanceFromTarget > enemyAnimator.GetComponentInParent<EnemyManager>().maxAttackRange)
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

    }
    private void GetNewAttack(EnemyManager enemyManager) //攻击从设置好的攻击列表中随机挑选下一次的攻击动画(近战)
    {
        Vector3 targetDirection = enemyManager.curTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, transform.position);

        int maxScore = 0;

        if (!enemyManager.isFirstAttack)
        {
            //随机攻击方式(未来重写方法，当前内容太少)
            for (int i = 1; i < enemyAttacks.Length; i++)
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

            for (int i = 1; i < enemyAttacks.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttacks[i];

                if (distanceFromTarget <= enemyAttackAction.maxDistanceNeedToAttack && distanceFromTarget >= enemyAttackAction.minDistanceNeedToAttack)
                {
                    if (viewableAngle <= enemyAttackAction.maxAttackAngle && viewableAngle >= enemyAttackAction.minAttackAngle)
                    {
                        if (attackState.curAttack != null)
                            return;

                        tempScore += enemyAttackAction.attackScore;

                        if (tempScore > randomValue)
                        {
                            attackState.curAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }
        else 
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[0];

            attackState.curAttack = enemyAttackAction;

            enemyManager.isFirstAttack = false;
        }
    }

}
