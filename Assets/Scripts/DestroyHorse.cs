﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHorse : MonoBehaviour
{
    public GameObject horse;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            Destroy(horse);
    }
}