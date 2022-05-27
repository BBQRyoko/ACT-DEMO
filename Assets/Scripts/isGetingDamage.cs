using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGetingDamage : StateMachineBehaviour
{
    public string isGettingDamageBool;
    public bool isGettingDamage;

    public string cantBeInterruptedBool;
    public bool cantBeInterrupted;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isGettingDamage = true;
        animator.SetBool(isGettingDamageBool, isGettingDamage);
        cantBeInterrupted = true;
        animator.SetBool(cantBeInterruptedBool, cantBeInterrupted);
        animator.SetBool("isDefending", false);
        animator.SetBool("isHolding", false);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isGettingDamage = false;
        animator.SetBool(isGettingDamageBool, isGettingDamage);
    }
}
