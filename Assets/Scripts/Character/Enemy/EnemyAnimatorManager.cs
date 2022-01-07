using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : MainAnimatorManager
{
    public EnemyManager enemyManager;
    Boss_CombatStanceState boss_CombatStanceState;

    public float animatorSpeed;

    public Collider damageCollider;

    //VFX
    public AudioSource bossAudio;
    public AudioSource hittedAudio;
    public Sample_SFX boss_sfx;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyManager = GetComponentInParent<EnemyManager>();
        boss_CombatStanceState = GetComponentInChildren<Boss_CombatStanceState>();
    }

    private void Update()
    {
        animatorSpeed = animator.speed;
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        enemyManager.enemyRig.drag = 0;
        Vector3 deltaPosition = animator.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        enemyManager.enemyRig.velocity = velocity;

        if (enemyManager.isRotatingWithRootMotion) 
        {
            enemyManager.transform.rotation *= animator.deltaRotation;
        }
    }

    private void CheckingPlayerPosition() //用于Boss的行为转变
    {
        if (enemyManager.curTargetAngle == 0 && enemyManager.curTargetDistance == 0) //Combo Check
        {
            if (enemyManager.comboCount > 0) 
            {
                animator.SetBool("canCombo", true);
                enemyManager.comboCount -= 1;
            }
        }
        else if (enemyManager.curTargetAngle == 0 && enemyManager.curTargetDistance == 1) //Combo Check
        {
            if (enemyManager.comboCount > 0)
            {
                animator.SetBool("canCombo", true);
                enemyManager.comboCount -= 1;
            }
        }
    }

    private void shoutStun() //在动画中使用该功能
    {
        Collider[] targetInArea = Physics.OverlapSphere(transform.position, enemyManager.shoutRadius, enemyManager.playerLayer);
        foreach (Collider player in targetInArea)
        {
            player.GetComponent<PlayerManager>().GetDebuff(3.5f);
        }
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void AnimatorPlaySound(int clipNum) //选择播放的音频
    {
        //attackAudio.volume = 1;
        //bossAudio.clip = boss_sfx.curSFX_List[clipNum];
        //bossAudio.Play();
    }

    private void DodgingEnd() 
    {
        enemyManager.isDodging = false;
    }


    private void RangeAttack() 
    {
        enemyManager.HandleRangeAttack();
    }
}
