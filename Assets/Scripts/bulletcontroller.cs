using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class bulletcontroller : MonoBehaviour
{
    private Transform bullet_origin_coord = null;
    private Rigidbody bullet_rigidbody = null;
    // Start is called before the first frame update
    void Start()
    {
        bullet_origin_coord = transform.parent.Find("bullet_origin_coord");
        bullet_rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void OnAttachedToHand(Hand hand)
    {
        // kinematic 해제
        bullet_rigidbody.isKinematic = false;
    }
    void OnDetachedFromHand(Hand hand)
    {
        // 놓으면 원래 자리로 돌아가기..
        gameObject.transform.position = bullet_origin_coord.position;
        gameObject.transform.rotation = bullet_origin_coord.rotation;
        bullet_rigidbody.isKinematic = true;
        PlayerController.Instance.Isbulletloaded = false;
    }
}
