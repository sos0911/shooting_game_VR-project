using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reload_detector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!PlayerController.Instance.Isbulletloaded && other.transform.tag == "bullet")
        {
            PlayerController.Instance.Isbulletloaded = true;
            this.transform.parent.parent.GetComponent<Revolver_fire>().reload(other.gameObject);
        }
    }
}
