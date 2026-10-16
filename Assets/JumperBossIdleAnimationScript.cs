
using UnityEngine;


public class JumperBossIdleAnimationScript : StateMachineBehaviour
{
    private JumperLogic enemyScript;
    private float? totalHp;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyScript = animator.GetComponent<JumperLogic>();
        if (totalHp == null)
        {
            totalHp = enemyScript.HitPoints;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SpriteRenderer spriteRenderer = animator.GetComponent<SpriteRenderer>();
        float green = Mathf.Clamp(enemyScript.HitPoints / totalHp ?? 1f, .2f, 1f);
        Debug.Log("Should be " + green + " green");
        spriteRenderer.material.color = new Color(spriteRenderer.material.color.r, green, spriteRenderer.material.color.b, 1f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<SpriteRenderer>().material.color = Color.white;
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
