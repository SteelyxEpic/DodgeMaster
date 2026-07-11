using UnityEngine;

public class attack : StateMachineBehaviour
{
    public bool dashAttack = false;
    public bool smashAttack = false;
    public float time = 0.5f;
    bool hasTriggered = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {  
        if (smashAttack) animator.SetFloat("smashspeed", animator.GetFloat("AttackSpeed"));
        hasTriggered = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
           if (stateInfo.normalizedTime >= time && !hasTriggered && stateInfo.normalizedTime < time + 0.1f)
        {
            UIStuff.ins.overheatDisplayTrigger();
            animator.gameObject.GetComponent<playermovement>().col.enabled = true;
            if(smashAttack)
            {
                animator.SetFloat("smashspeed", 0);
            }
            hasTriggered = true;
        }else if (stateInfo.normalizedTime >= time + 0.1f && hasTriggered)
        {
            animator.gameObject.GetComponent<playermovement>().col.enabled = false;
            hasTriggered = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dashAttack)
        {
            animator.gameObject.GetComponent<playermovement>().currentState = playermovement.PlayerState.Idle;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
