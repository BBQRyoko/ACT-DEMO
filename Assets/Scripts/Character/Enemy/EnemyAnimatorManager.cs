using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : MainAnimatorManager
{
    CameraManager cameraManager;
    public EnemyManager enemyManager;

    public float animatorSpeed;

    public Collider damageCollider;

    //VFX
    public AudioSource bossAudio;
    public AudioSource hittedAudio;
    public Sample_SFX boss_sfx;

    private void Awake()
    {
        cameraManager = FindObjectOfType<CameraManager>();
        animator = GetComponent<Animator>();
        enemyManager = GetComponentInParent<EnemyManager>();
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

    private void AnimatorPlaySpeed(float playRate) //控制动画器的播放速度
    {
        animator.speed = playRate;
    }

    private void MovingDuringAnimation(float movingForce)
    {
        Vector3 dir = new Vector3(enemyManager.curTarget.transform.position.x - enemyManager.transform.position.x, enemyManager.curTarget.transform.position.y - enemyManager.transform.position.y, enemyManager.curTarget.transform.position.z - enemyManager.transform.position.z);
        dir.Normalize();
        enemyManager.GetComponent<Rigidbody>().AddForce(dir * movingForce, ForceMode.Impulse);
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

    private void RotateTowardsTarget() 
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

    private void ResetFirstStrike() 
    {
        if (enemyManager.firstStrikeTimer <= 0) 
        {
            enemyManager.isFirstStrike = true;
            enemyManager.curState = enemyManager.transform.GetComponentInChildren<PursueState>();
        }
    }

    private void DodgingEnd() 
    {
        enemyManager.isDodging = false;
    }

    private void EnemyDangerWarning() 
    {
        cameraManager.DangerWarning(enemyManager);
    }

}
