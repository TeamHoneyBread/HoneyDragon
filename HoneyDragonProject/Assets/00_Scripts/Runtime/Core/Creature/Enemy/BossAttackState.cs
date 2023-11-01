using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : StateMachineBehaviour
{
    Boss boss;
    Rigidbody rigidBody;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        rigidBody = animator.GetComponent<Rigidbody>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss.IsTargetInAttackRange() == false)
        {
            animator.ResetTrigger("RangeOver");
            animator.SetTrigger("RangeOver");
            return;
        }
        var direction = (boss.Target.position - boss.position).normalized;
        rigidBody.rotation = Quaternion.LookRotation(direction);
        animator.SetTrigger("Chase");
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}