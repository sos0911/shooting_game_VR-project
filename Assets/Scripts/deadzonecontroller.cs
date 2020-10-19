using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadzonecontroller : MonoBehaviour
{
    // 닿으면 바로 사망
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            PlayerController.Instance.hp = 0;
    }
}
