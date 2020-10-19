using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Opendoor : MonoBehaviour
{

    Vector3 axis;
    Vector3 pos;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        pos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.eulerAngles.y > -90.0f)
        {
            transform.RotateAround(pos, Vector3.up, -speed * Time.deltaTime);
        }
    }
}
