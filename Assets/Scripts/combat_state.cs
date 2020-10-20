using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class combat_state : StateMachineBehaviour
{

    Transform target;
    public float attackDelay = 3.0f;
    float timeCounter = 0.0f;
    private EnemyController enemy_controller = null;

    public float rotatespeed = 5.0f;

    // onstateenter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = GameObject.Find("Player").transform;
        enemy_controller = animator.gameObject.GetComponent<EnemyController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 dir = target.position - animator.transform.position;
        float distance = dir.magnitude;
        timeCounter += Time.deltaTime;

        // 발견될 때 거리보다 좀 더 멀어지면 풀림
        if (distance >= 5.0f)
        {
            animator.SetBool("Combat", false);
        }
        // 공격 쿨탐 차면 공격
        else if(timeCounter > attackDelay)
        {
            timeCounter = 0.0f;
            animator.SetTrigger("Shooting");
            // 공격 스크립트(레이캐스트)  호출..
            enemy_controller.Fire_weapon();
        }    

        

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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