using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon_receptor : MonoBehaviour
{
    public Transform weapon_origin_coord;
    private Transform weapon_current_coord;

    private Rigidbody weapon_rigid = null;
    private Animator weapon_animator = null;
    private Transform weapon_transform = null;
    // 자신에게 무기가 닿으면 수납하는 것으로 인식한다.
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("OnCollisionEnter with something!!");
        if (collision.gameObject.tag == "weapon")
        {
            if (PlayerController.Instance.state == PlayerController.playerState.grapweapon)
            {
                weapon_rigid = collision.gameObject.GetComponent<Rigidbody>();
                weapon_animator = collision.gameObject.GetComponent<Animator>();
                weapon_transform = collision.gameObject.transform;

                // 실제로 고정은 revolver_fire에서..
                // 여기는 flag만 세우기
                weapon_current_coord = collision.gameObject.transform;
                PlayerController.Instance.weapon_should_locked = true;
            }
        }
    }

    /// <summary>
    /// 무기 홀스터에 고정
    /// </summary>
    public void fix_weapon()
    {
        // 홀스터에 고정
        // 장전 애니메이션 상태도 풀어주어야 함
        weapon_animator.SetBool("should_reload", false);
        PlayerController.Instance.state = PlayerController.playerState.idle;

        weapon_rigid.useGravity = false;
        weapon_rigid.isKinematic = true;

        Debug.Log("fix_weapon");
        weapon_current_coord.position = weapon_origin_coord.position;
        weapon_current_coord.rotation = weapon_origin_coord.rotation;
    }
}
