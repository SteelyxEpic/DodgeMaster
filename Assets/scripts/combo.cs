using UnityEngine;
using TMPro;

public class combo : StateMachineBehaviour
{
    float comboValue;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        comboValue = int.TryParse(animator.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text, out int value) ? value : 0;
        comboValue++;
        animator.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = comboValue.ToString();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0.2f)
        {
            if (comboValue < animator.GetFloat("combovalue"))
            {
                animator.SetTrigger("combo");
                if(animator.GetFloat("speed") < 9.0f)
                {
                    animator.SetFloat("speed", animator.GetFloat("speed") + 0.1f);
                }
            }
            else
            {
                animator.SetFloat("speed", 0.8f);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //comboValue = 0;
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
