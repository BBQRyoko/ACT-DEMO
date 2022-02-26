using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsBlocking : StateMachineBehaviour
{
    public string isBlockingBool;
    public bool isBlockStatus;

    public string cantBeInterruptedBool;
    public bool cantBeInterrupted;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isBlockStatus = true;
        animator.SetBool(isBlockingBool, isBlockStatus);
        cantBeInterrupted = true;
        animator.SetBool(cantBeInterruptedBool, cantBeInterrupted);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isBlockStatus = false;
        animator.SetBool(isBlockingBool, isBlockStatus);
        cantBeInterrupted = false;
        animator.SetBool(cantBeInterruptedBool, cantBeInterrupted);
    }
}
