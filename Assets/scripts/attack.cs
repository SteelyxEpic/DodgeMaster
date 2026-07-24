using UnityEngine;

public class attack : StateMachineBehaviour
{
    public bool dashAttack = false;
    public bool smashAttack = false;
    public float[] time;
    bool[] hasTriggered = {false};
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {  
        hasTriggered = new bool[]{false};
        if (smashAttack){ 
            time = new float[]{animator.gameObject.GetComponent<playermovement>().weapon.Stime};
            animator.SetFloat("smashspeed", animator.GetFloat("AttackSpeed"));
        }else if (dashAttack){
            
            time = new float[]{animator.gameObject.GetComponent<playermovement>().weapon.Dtime};
            Debug.Log(time[0]);
        }
        else
        {
            hasTriggered = new bool[animator.gameObject.GetComponent<playermovement>().weapon.comboValues.Length];
        time = animator.gameObject.GetComponent<playermovement>().weapon.comboValues;
        }

        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for (int i = 0; i < time.Length; i++)
        {
            float t = time[i];
            if (stateInfo.normalizedTime >= t && !hasTriggered[i])
            {
                

                if(smashAttack)
            {
                if(animator.gameObject.GetComponent<playermovement>().currentState == playermovement.PlayerState.Smashing)animator.SetFloat("smashspeed", 0);
                }
                else
                {
                    UIStuff.ins.overheatDisplayTrigger();
                    animator.gameObject.GetComponent<playermovement>().attack();
                }
            hasTriggered[i] = true;
        }}
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<playermovement>().currentState = playermovement.PlayerState.Idle;
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
