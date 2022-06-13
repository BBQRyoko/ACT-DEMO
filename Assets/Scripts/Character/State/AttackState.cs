using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public IdleState idleState;
    public RotateTowardsTargetState rotateTowardsTargetState;
    public CombatStanceState combatStanceState;
    public PursueState pursueState;
    public EnemyAttackAction curAttack;
    public int curRegularIndex;
    public int curSpecialIndex;
    public CombatCooldownManager combatCooldownManager;

    public bool isExecution;

    public bool hasPerformedAttack = false;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        if (enemyManager.isAlerting)
        {
            enemyManager.isAlerting = false;
            enemyManager.alertingTarget = null;
            enemyManager.alertTimer = 0;
        }

        if (enemyManager.isPhaseChaging) 
        {
            return idleState;
        }

        float distanceFromTarget = Vector3.Distance(enemyManager.curTarget.transform.position, enemyManager.transform.position);
        enemyManager.isParrying = false;
        RotateTowardsTargetWhiletAttacking(enemyManager);

        if (distanceFromTarget > enemyManager.maxCombatRange && !enemyManager.isFirstStrike && !enemyManager) //如果突然离开最大攻击范围, 重新进入追击
        {
            return pursueState;
        }

        if (!enemyManager.isEquipped) enemyAnimatorManager.PlayTargetAnimation("Equip", true, true);

        if (!hasPerformedAttack)
        {
            if (enemyManager.isNoWeapon)
            {
                AttackTarget(enemyAnimatorManager, enemyManager); //进行普通攻击动画的播放
            }
            else
            {
                if (enemyManager.isEquipped)
                {
                    AttackTarget(enemyAnimatorManager, enemyManager); //进行普通攻击动画的播放
                }
            }
        }

        if (!enemyManager.isInteracting)
        {
            return rotateTowardsTargetState; //攻击完进入转身state以确认玩家仍在范围之内
        }
        else 
        {
            return this;
        }
    }
    private void AttackTarget(EnemyAnimatorManager enemyAnimatorManager, EnemyManager enemyManager) 
    {
        //使用Combat State中所决定的攻击动画, 并计算攻击恢复时间,是否霸体,技能独立CD
        enemyAnimatorManager.PlayTargetAnimation(curAttack.actionAnimation, true);
        if (curAttack.isFlyingObject)
        {
            enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().flyingObjectDamager.curDamage = curAttack.damagePoint;
            enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().flyingObjectDamager.isHeavy = curAttack.isHeavyAttack;
        }
        else if(curAttack.isHeavyKick)
        {
            enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().kickDamagerCollider.curDamage = curAttack.damagePoint;
            enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().kickDamagerCollider.isHeavyAttack = curAttack.isHeavyAttack;
        }
        else 
        {
            enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().weaponDamageCollider.curDamage = curAttack.damagePoint;
            enemyAnimatorManager.GetComponent<EnemyWeaponSlotManager>().weaponDamageCollider.isHeavyAttack = curAttack.isHeavyAttack;
        }

        if (curAttack.canDodge)
        {
            enemyManager.GetComponentInChildren<Animator>().SetBool("isDodging", true);
        }

        if (isExecution) 
        {
            enemyManager.curTarget.transform.position = enemyManager.execute_Front.position;
            enemyManager.curTarget.GetComponent<PlayerManager>().HandleExecuted();
            enemyManager.canExecute = false;
            isExecution = false;
        }

        enemyManager.curRecoveryTime = curAttack.recoveryTime;
        enemyManager.isImmuneAttacking = curAttack.isImmune;
        if (!curAttack.isSpecial)
        {
            combatCooldownManager.regularAttackCooldownTimer[curRegularIndex] = curAttack.independtCooldown;
        }
        else 
        {
            combatCooldownManager.specialAttackCooldownTimer[curSpecialIndex] = curAttack.independtCooldown;
            combatStanceState.specialConditionTriggered = false;
            enemyManager.isFirstStrike = false;
        }
        hasPerformedAttack = true;
        curAttack = null;
    }

    public void RotateTowardsTargetWhiletAttacking(EnemyManager enemyManager) //攻击始终朝着目标方向, 保证出手时一定朝着目标位置(瞬间转向)
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
