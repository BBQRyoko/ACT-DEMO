using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_ResetState : State
{
    public Boss_IdleState boss_IdleState;
    public float distanceFromTarget;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager) 
    {
        if (enemyManager.isPreformingAction)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            return this;
        }

        Vector3 targetDirection = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - enemyManager.transform.position;
        distanceFromTarget = Vector3.Distance(enemyManager.patrolPos[enemyManager.curPatrolIndex].position, enemyManager.transform.position);

        if (distanceFromTarget > 0.5f)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0.5f, 0.1f, Time.deltaTime);   //朝着目标单位进行移动
            return this;
        }
        else if (distanceFromTarget <= 0.5f)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);   //站着idle状态
            Destroy(enemyManager.gameObject);
            return boss_IdleState;
        }

        HandleRotateTowardsTarger(enemyManager);
        enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
        enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;

        return this;
    }

    public void HandleRotateTowardsTarger(EnemyManager enemyManager) //移动时保持朝着目标方向
    {
        if (enemyManager.isPreformingAction)
        {
            Vector3 direction = enemyManager.patrolPos[enemyManager.curPatrolIndex].position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyManager.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, enemyManager.rotationSpeed);
        }
        //Roate with pathfinding
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyManager.enemyRig.velocity;

            enemyManager.navMeshAgent.enabled = true;
            enemyManager.navMeshAgent.SetDestination(enemyManager.patrolPos[enemyManager.curPatrolIndex].position);
            enemyManager.enemyRig.velocity = targetVelocity;
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
