using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MainAnimatorManager
{
    PlayerManager playerManager;
    PlayerLocmotion playerLocmotion;
    public PlayerAttacker playerAttacker;
    public AudioSource attackAudio;
    public AudioSource hittedAudio;
    int horizontal;
    int vertical;

    //VFX
    public Sample_VFX sample_VFX_S;
    public Sample_SFX sample_SFX;

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
        float h = 0;
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
        #endregion

        if (isSprinting) 
        {
            v = 2;
            h = horizontalMovement;
        }

        if (playerManager.isWeaponEquipped)
        {
            h = 2;
        }
        else 
        {
            h = 0;
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
                    StartCoroutine(Pause(10));
                    hittedAudio.clip = sample_SFX.hittedSFX_List[0];
                    hittedAudio.Play();
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
    private void AnimatorPlaySound(int clipNum) //选择播放的音频
    {
        //attackAudio.volume = 1;
        attackAudio.clip = sample_SFX.curSFX_List[clipNum];
        attackAudio.Play();
    }
    private void AnimatorPlayVFX(int num) //选择播放的特效
    {
        sample_VFX_S.curVFX_List[num].Play();
    }
    private void AnimatorStop(int stopDuration) //播放器暂停与暂停的时间
    {
        StartCoroutine(Pause(stopDuration));
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
    private void ChargingLevelUpEvent() 
    {
        playerAttacker.chargingTimer = 0;
        playerAttacker.chargingLevel += 1;
    }
    private void DamageAvoidActive() 
    {
        playerManager.damageAvoid = true;
    }

    private void DamageAvoidDeactive()
    {
        playerManager.damageAvoid = false;
    }
    IEnumerator Pause(int dur) //播放器暂停
    {
        float pauseTime = dur / 60f;
        animator.speed = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        animator.speed = 1;
    }
}