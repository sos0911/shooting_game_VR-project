using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 다 총 쏘는 놈들로 채우자..
    // 칼든 애들도 하고 싶은데, 시간이 그렇게는 없음
    public int hp = 50;
    public Animator enemy_animator;
    public ParticleSystem shoot_effect;
    public ParticleSystem hit_effect;


    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
