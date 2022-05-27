using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isAttacking : StateMachineBehaviour
{
    public string cantBeInterruptedBool;
    public bool cantBeInterrupted;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cantBeInterrupted = true;
        animator.SetBool(cantBeInterruptedBool, cantBeInterrupted);
        animator.SetBool("isAttacking", true);
    }
}
