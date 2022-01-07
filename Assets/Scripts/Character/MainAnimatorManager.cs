using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAnimatorManager : MonoBehaviour
{
    public Animator animator;
    public bool canRotate;

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting, bool useRootMotion = false) //播放指定动画, 并确定是否锁定玩家的输入, 是否开启rootMotion模式
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.SetBool("isUsingRootMotion", useRootMotion);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    public void PlayTargetAnimationWithRootRotation(string targetAnimation, bool isInteracting) 
    {
        animator.applyRootMotion = isInteracting;
        animator.SetBool("isRotatingWithRootMotion", true);
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }
}
