using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MainAnimatorManager
{
    PlayerManager playerManager;
    PlayerLocmotion playerLocmotion;
    public PlayerAttacker playerAttacker;
    public AudioSource attackAudio;
    public AudioSource generalAudio;
    int horizontal;
    int vertical;

    //Combat
    public int pauseDuration;

    //VFX
    public Sample_VFX sample_VFX_S;
    public Sample_SFX sample_SFX;

    //Roll
    public DamageCollider rollDamager;

    public float animatorPlaySpeed = 1;

    public bool ifSpeedChanged;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponentInParent<PlayerManager>();
        playerLocmotion = GetComponentInParent<PlayerLocmotion>();
        playerAttacker = GetComponentInParent<PlayerAttacker>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        animator.applyRootMotion = false;
    }
    public void UpdateAnimatorVaules(float horizontalMovement, float verticalMovement, bool isSprinting) 
    {
        //数值判断是走还是跑
        float v = 0;
        float h = 2;
        #region Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }
        #endregion
        #region Horizontal;

        if (playerManager.isCrouching)
        {
            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = -2.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = -2;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -3.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -4;
            }
            else
            {
                h = -3;
            }
        }
        else if (playerManager.isHolding)
        {
            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 3.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 4;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = 2.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = 2;
            }
            else
            {
                h = 3;
            }
        }
        else if (playerManager.isHanging) 
        {
            if (horizontalMovement > 0)
            {
                h = -5f;
            }
            else if (horizontalMovement < 0)
            {
                h = -7f;
            }
            else
            {
                h = -6;
            }
        }
        else
        {
            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
        }
        #endregion

        if (isSprinting)
        {
            v = 2;
            //h = horizontalMovement;
        }

        animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    } //Locomotion数值变化(如果有需要在locomotion中加入额外的状态在这里改)
    private void OnAnimatorMove()
    {
        if (playerManager.isUsingRootMotion)
        {
            if (playerManager.isGround)
            {
                playerLocmotion.rig.drag = 0;
                Vector3 deltaPosition = animator.deltaPosition;
                deltaPosition.y = 0;
                Vector3 velocity = deltaPosition / Time.deltaTime;

                if (playerManager.isHitting)
                {
                    playerLocmotion.rig.velocity = new Vector3(0, playerLocmotion.rig.velocity.y, 0);
                    StartCoroutine(Pause(pauseDuration));
                }
                else
                {
                    playerLocmotion.rig.velocity = velocity;
                    if (!playerManager.isGround)
                    {
                        playerLocmotion.rig.velocity = new Vector3(0, playerLocmotion.rig.velocity.y, 0);
                        playerLocmotion.HandleGravity();
                    }
                }
            }
            else
            {
                //想想办法
            }
        }
    }
    //Animator Events Editor  
    private void AnimatorPlaySpeed(float playRate) //控制动画器的播放速度
    {
        animator.speed = playRate;
    }
    private void MovingDuringAnimation(float movingForce) 
    {
        playerManager.GetComponent<Rigidbody>().AddForce(Vector3.forward* movingForce, ForceMode.Impulse);
    }
    private void AnimatorPlaySound(int clipNum) //选择播放的音频
    {
        attackAudio.volume = 0.07f;
        attackAudio.clip = sample_SFX.curSFX_List[clipNum];
        attackAudio.Play();
    }
    private void PlayEquipSound() 
    {
        generalAudio.volume = 0.1f;
        generalAudio.clip = sample_SFX.EquipSFX;
        generalAudio.Play();
    }
    private void AnimatorPlayVFX(int num) //选择播放的特效
    {
        sample_VFX_S.curVFX_List[num].Play();
    }
    private void AnimatorStop(int stopDuration) //播放器暂停与暂停的时间
    {
        StartCoroutine(Pause(stopDuration));
    }
    private void AttackRotateAllow() 
    {
        if (!playerManager.attackRotate)
        {
            playerManager.attackRotate = true;
        }
        else 
        {
            playerManager.attackRotate = false;
        }
    }
    private void HandleRotateTowardsTarget() 
    {
        if (playerLocmotion.willRotateTowardsTarget)
        {
            playerLocmotion.willRotateTowardsTarget = false;
        }
        else 
        {
            playerLocmotion.willRotateTowardsTarget = true;
        }
    }
    private void hitRecoverAnnounce(int recoverLevel) 
    {
        if (recoverLevel >= 2)
        {
            playerManager.hitRecover = true;
        }
        else if (recoverLevel == 0) 
        {
            playerManager.hitRecover = false;
        }
    }
    private void RangeAttack(int index) 
    {
        playerManager.HandleRangeAttack(index);
    }
    private void ChargingLevelUpEvent() 
    {
        playerAttacker.chargingTimer = 0;
        playerAttacker.chargingLevel += 1;
    }
    private void RollDamageAvoid() 
    {
        if (!playerManager.damageAvoid)
        {
            playerManager.damageAvoid = true;
            rollDamager.EnableDamageCollider();
        }
        else 
        {
            playerManager.damageAvoid = false;
            rollDamager.DisableDamageCollider();
        }
    }
    private void HangingCheck() 
    {
        //if (playerManager.isHanging)
        //{
        //    playerManager.isHanging = false;
        //}
        //else 
        //{
        //    playerManager.isHanging = true;
        //}
    }
    IEnumerator Pause(int dur) //播放器暂停
    {
        float pauseTime = dur / 60f;
        animator.speed = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        animator.speed = 1;
    }
}