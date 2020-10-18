using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlDoorMove : MonoBehaviour
{
    private Animator dooranimator = null;
    private bool IsAniPlaying = false;
    
    // 문의 움직임을 컨트롤하는 스크립
    // Start is called before the first frame update
    void Start()
    {
        dooranimator = gameObject.transform.parent.GetComponent<Animator>();
    }

    private void Update()
    {
        if (dooranimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            IsAniPlaying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        // 플레이어에 닿으면 상호작용
        if(!IsAniPlaying && other.gameObject.transform.root.tag == "Player")
        {
            if(!dooranimator.GetBool("should_open"))
                dooranimator.SetBool("should_open", true);
            else
                dooranimator.SetBool("should_open", false);

            IsAniPlaying = true;
        }
    }
}
