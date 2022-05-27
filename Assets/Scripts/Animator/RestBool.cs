using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestBool : StateMachineBehaviour
{
    [SerializeField] bool isPlayer;

    public string isInteractingBool;
    public bool isInteractingStatus;

    public string isUsingRootMotionBool;
    public bool isUsingRootMotionStatus;

    public string isRotatingWithRootMotion = "isRotatingWithRootMotion";
    public bool isRotatingWithRootMotionStatus = false;

    public string isGettingDamageBool;
    public bool isGettingDamageStatus;

    public string canCombo;
    public bool canComboStatus;

    public string cantBeInterruptedBool;
    public bool cantBeInterrupted;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isInteractingBool, isInteractingStatus);
        animator.SetBool(isUsingRootMotionBool, isUsingRootMotionStatus);
        animator.SetBool(isRotatingWithRootMotion, isRotatingWithRootMotionStatus);
        animator.SetBool(isGettingDamageBool, isGettingDamageStatus);
        animator.SetBool(canCombo, canComboStatus);
        animator.SetBool(cantBeInterruptedBool, cantBeInterrupted);
        animator.speed = 1;

        if (isPlayer) 
        {
            animator.ResetTrigger("isLeftRoll");
            animator.ResetTrigger("isRightRoll");
            animator.ResetTrigger("isFrontRoll");
            animator.ResetTrigger("isBackRoll");
        }//如果是玩家的animator时所重置的特殊条件
    }
}
