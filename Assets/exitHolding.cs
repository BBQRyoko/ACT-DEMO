using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitHolding : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isHolding", false);
    }
}
