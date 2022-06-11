using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    PlayerStats playerStats;
    EnemyManager enemyManager;
    bool buffed;
    [SerializeField] int buffType; // 0 - normal, 1 - attack, 2 - defend, 3 - movement
    [SerializeField] int debuffType; // 0 - normal, 1 - burning
    public float duration;
    [SerializeField] float buffEffectNum;

    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }

    public void Update()
    {
        if (duration >= 0)
        {
            duration -= Time.deltaTime;
            buffEffectCheck();
        }
        else 
        {
            if (playerStats)
            {
                if (buffType == 1)
                {
                    playerStats.attackBuffRatio -= buffEffectNum;
                }
                else if (buffType == 2)
                {
                    playerStats.defendBuffRatio -= buffEffectNum;
                }
                else if (buffType == 3)
                {
                    playerStats.movementBuffRatio -= buffEffectNum;
                }
            }
            Destroy(gameObject);
        }
    }
    void buffEffectCheck() 
    {
        if (playerStats && !buffed) 
        {
            if (buffType == 1)
            {
                playerStats.attackBuffRatio += buffEffectNum;
                buffed = true;
            }
            else if (buffType == 2) 
            {
                playerStats.defendBuffRatio += buffEffectNum;
                buffed = true;
            }
            else if (buffType == 3)
            {
                playerStats.movementBuffRatio += buffEffectNum;
                buffed = true;
            }
        }
    }
}
