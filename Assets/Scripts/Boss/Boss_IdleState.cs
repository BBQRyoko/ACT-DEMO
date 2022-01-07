using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_IdleState : State
{
    public Boss_PursueState boss_PursueState;
    public LayerMask detectionLayer;
    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
    {
        if (enemyManager.isPreformingAction)
        {
            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            return this;
        }

        #region 敌人的可侦测范围设置
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

            if (characterStats != null)
            {
                //Check Character ID
                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minDetectionAngle && viewableAngle < enemyManager.maxDetectionAngle)
                {
                    enemyManager.curTarget = characterStats;
                }
            }
        }
        #endregion

        #region 切换至追踪模式
        if (enemyManager.curTarget != null)
        {
            enemyAnimatorManager.PlayTargetAnimation("Shout", true);
            enemyManager.shoutTimer = 15f;
            enemyManager.shouted = true;
            return boss_PursueState; //当发现目标后, 进入追踪模式
        }
        else
        {
            return this;
        }
        #endregion
    }
}
