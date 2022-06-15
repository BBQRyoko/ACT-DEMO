using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCooldownManager : MonoBehaviour
{
    public CombatStanceState combatStanceState;
    public List<float> regularAttackCooldownTimer;
    public List<float> specialAttackCooldownTimer;

    private void Awake()
    {
        foreach (EnemyAttackAction attackAction in combatStanceState.enemyAttacks) 
        {
            regularAttackCooldownTimer.Add(0);
        }

        foreach (EnemyAttackAction specialAction in combatStanceState.conditionList)
        {
            specialAttackCooldownTimer.Add(0);
        }
    }

    public void CombatCooldownReset() 
    {
        regularAttackCooldownTimer.Clear();
        specialAttackCooldownTimer.Clear();
        foreach (EnemyAttackAction attackAction in combatStanceState.enemyAttacks)
        {
            regularAttackCooldownTimer.Add(0);
        }

        foreach (EnemyAttackAction specialAction in combatStanceState.conditionList)
        {
            specialAttackCooldownTimer.Add(0);
        }
    }

    public void CountDownAllTimer() 
    {
        for (int i = 0; i < regularAttackCooldownTimer.Count; i++) 
        {
            if (regularAttackCooldownTimer[i] > 0)
            {
                regularAttackCooldownTimer[i] -= Time.deltaTime;
            }
            else 
            {
                regularAttackCooldownTimer[i] = 0;
            }
        }

        for (int i = 0; i < specialAttackCooldownTimer.Count; i++)
        {
            if (specialAttackCooldownTimer[i] > 0)
            {
                specialAttackCooldownTimer[i] -= Time.deltaTime;
            }
            else
            {
                specialAttackCooldownTimer[i] = 0;
            }
        }
    }
}
