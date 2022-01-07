using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_RotateTowardsTargetState : State
{
    public Boss_CombatStanceState boss_CombatStanceState;
    public float viewableAngle;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        enemyAnimatorManager.animator.SetFloat("Vertical", 0);
        enemyAnimatorManager.animator.SetFloat("Horizontal", 0);

        Vector3 targetDirection = enemyManager.curTarget.transform.position - enemyManager.transform.position;
        viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

        if (enemyManager.isInteracting)
        {
            return boss_CombatStanceState;
        }

        if (viewableAngle >= 100 && viewableAngle <= 180 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Attack(3)", true);
            return boss_CombatStanceState;
        }
        else if (viewableAngle <= -101 && viewableAngle >= -180 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Attack(3)", true);
            return boss_CombatStanceState;
        }
        else if (viewableAngle <= -25 && viewableAngle >= -100 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("TurnRight90", true);
            return boss_CombatStanceState;
        }
        else if (viewableAngle >= 25 && viewableAngle <= 100 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("TurnLeft90", true);
            return boss_CombatStanceState;
        }


        return boss_CombatStanceState;
    }
}
