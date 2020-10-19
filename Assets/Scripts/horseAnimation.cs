using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class horseAnimation : MonoBehaviour
{

    Vector3 axis;
    Vector3 pos;
    Vector3 eulerAngles;
    public float speed;
    public int check;
    // Start is called before the first frame update
    void Start()
    {
        check = 1;
        pos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        eulerAngles = transform.eulerAngles;
        
        if (eulerAngles.x >= 270.0f && check == 1)
        {
            transform.RotateAround(pos, Vector3.forward, speed * Time.deltaTime);
            if (eulerAngles.x >= 273.0f)
                check = 0;
        }
        else if (eulerAngles.x >= 270.0f && check == 0)
        {
            transform.RotateAround(pos, Vector3.forward, -speed * Time.deltaTime);
            if (eulerAngles.x <= 271.0f)
                check = 1;
        }



    }
}