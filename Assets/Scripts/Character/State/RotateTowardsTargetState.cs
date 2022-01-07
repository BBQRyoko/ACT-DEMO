using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsTargetState : State
{
    public CombatStanceState combatStanceState;

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        enemyAnimatorManager.animator.SetFloat("Vertical", 0);
        enemyAnimatorManager.animator.SetFloat("Horizontal", 0);

        Vector3 targetDirection = enemyManager.curTarget.transform.position - enemyManager.transform.position;
        float viewableAngle = Vector3.SignedAngle(targetDirection, enemyManager.transform.forward, Vector3.up);

        if (enemyManager.isInteracting) 
        {
            return this;
        }

        if (viewableAngle >= 100 && viewableAngle <= 180 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn180", true);
            return combatStanceState;
        }
        else if (viewableAngle <= -101 && viewableAngle >= -180 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("Turn180", true);
            return combatStanceState;
        }
        else if (viewableAngle <= -45 && viewableAngle >= -100 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("TurnRight90", true);
            return combatStanceState;
        }
        else if (viewableAngle >= 45 && viewableAngle <= 100 && !enemyManager.isInteracting)
        {
            enemyAnimatorManager.PlayTargetAnimationWithRootRotation("TurnLeft90", true);
            return combatStanceState;
        }


        return combatStanceState;
    }
}
