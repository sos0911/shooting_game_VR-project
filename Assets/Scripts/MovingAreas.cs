using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class MovingAreas : MonoBehaviour
{

    public int speed;
    Vector3 init_pos;
    GameObject area0;
    GameObject area1;
    GameObject area2;

    // Start is called before the first frame update
    void Start()
    {
       init_pos = transform.position;
       area0 = transform.GetChild(0).gameObject;
       area1 = transform.GetChild(1).gameObject;
       area2 = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if(transform.position.x < -247.07916)
        {
            transform.position = init_pos;
        }
    }
}
