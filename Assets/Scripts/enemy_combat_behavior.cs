using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class enemy_combat_behavior : StateMachineBehaviour
{
    public float rotatespeed = 5.0f;

    private Transform target;
    private Transform enemy_transform;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = GameObject.Find("Player").transform;
        enemy_transform = animator.gameObject.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("OnStateUpdate called!");
        //Debug.Log("target : " + target.position);

        //Vector3 dir = target.position - enemy_transform.position;

        //// 계속 몸을 회전해야 한다.
        ////animator.gameObject.transform.rotation = Quaternion.Lerp(animator.gameObject.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotatespeed);
        //Quaternion newquater = Quaternion.LookRotation(dir);
        //enemy_transform.rotation = newquater;

        //Debug.Log("enemy : " + enemy_transform.rotation.eulerAngles);
    }
}
