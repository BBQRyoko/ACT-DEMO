using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    EnemyManager enemyManager;
    Animator animator;
    IdleState idleState;
    EnemyAnimatorManager animatorManager;

    //Boss
    public HealthBar healthBar;

    public int stage; // 0 - 1, 1-2...
    public int phase2RequiredHealth;
    public bool canBeDamaged;

    public int staminaGauge;
    public int curStamina;

    bool phaseChanged;

    public GameObject phaseVFX;
    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        animator = GetComponentInChildren<Animator>();
        animatorManager = GetComponentInChildren<EnemyAnimatorManager>();
        if(!enemyManager.isBoss) idleState = GetComponentInChildren<IdleState>();
    }
    private void Start()
    {
        currHealth = maxHealth;
        if (healthBar) 
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
    }
    public void TakeDamage(int damage, Vector3 collisionDir, CharacterStats characterStats = null)
    {
        float viewableAngle = Vector3.SignedAngle(collisionDir, enemyManager.transform.forward, Vector3.up);
        currHealth = currHealth - damage;
        healthBar.SetCurrentHealth(currHealth);

        if (currHealth <= 0)
        {
            currHealth = 0;
            animatorManager.PlayTargetAnimation("Dead", true);
            enemyManager.isDead = true;
        }
        else
        {
            if (!enemyManager.isImmuneAttacking)
            {
                if (viewableAngle >= 91 && viewableAngle <= 180)
                {
                    animatorManager.PlayTargetAnimation("Hit_B", true, true);
                }
                else if (viewableAngle <= -91 && viewableAngle >= -180)
                {
                    animatorManager.PlayTargetAnimation("Hit_B", true, true);
                }
                else if (viewableAngle >= -90 && viewableAngle <= 0)
                {
                    animatorManager.PlayTargetAnimation("Hit_F", true, true);
                }
                else if (viewableAngle <= 90 && viewableAngle > 0)
                {
                    animatorManager.PlayTargetAnimation("Hit_F", true, true);
                }
            }


            enemyManager.curTarget = characterStats;
        }
    }

    void PhaseChange() 
    {
        //播放转阶段动画
        phaseChanged = true;
        animatorManager.PlayTargetAnimation("FallDown", true);
        //无法被伤害
        stage = 1;
        enemyManager.maxComboCount += 1;
        staminaGauge = staminaGauge / 2; //二阶段的耐力上限改变
        canBeDamaged = false;
        StartCoroutine(phaseChangingTimer());
    }

    IEnumerator phaseChangingTimer() 
    {
        yield return new WaitForSeconds(5f);
        animator.SetTrigger("phaseChanged");
        phaseVFX.SetActive(true);
        enemyManager.shoutTimer = 12f;
        enemyManager.shouted = true;
        canBeDamaged = true;
    }
}
