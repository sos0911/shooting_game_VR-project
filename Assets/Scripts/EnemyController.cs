using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // 다 총 쏘는 놈들로 채우자..
    // 칼든 애들도 하고 싶은데, 시간이 그렇게는 없음
    public int hp = 50;
    private Animator enemy_animator;
    public ParticleSystem shoot_effect;
    public ParticleSystem hit_effect;
    public Transform Player_transform;
    public Transform gun_pivot;
    public Transform right_hand_coord;

    public GameObject gun;

    public int damage = 15;

    public Transform shoot_coord; // 총알 나가는 좌표

    public enum EnemyState { idle, fire, hit, injured, death};

    EnemyState enemystate = EnemyState.idle;

    // 적이 플레이어를 쏘는 건..
    // 쏠때마다 총구 방향이 플레이어 주변 랜덤으로 가게 하고
    // 레이캐스팅

    private void Start()
    {
        enemy_animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (enemystate == EnemyState.death)
            return;

        // 총을 항상 손에 붙인다.
        // 회전 상태는 캐릭터를 따른다.
        gun_pivot.position = right_hand_coord.position;

        // 만약 정해진 반경 안에 플레이어가 있다면 쏜다.
        // 몸도 그 방향으로 회전해야 함. lerp()?

        if (enemy_animator.GetBool("Combat"))
        {
            Vector3 dir = Player_transform.position - gameObject.transform.position;

            // 계속 몸을 회전해야 한다.
            //animator.gameObject.transform.rotation = Quaternion.Lerp(animator.gameObject.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotatespeed);
            Quaternion newquater = Quaternion.LookRotation(dir);
            gameObject.transform.rotation = newquater;

            Debug.Log("enemy : " + gameObject.transform.rotation.eulerAngles);
        }

        // enemy 상태변경

        if (hp>0 && hp <= 25)
        {
            Debug.Log("triggered Injured");
            enemystate = EnemyState.injured;
            enemy_animator.SetBool("Combat", false);
            enemy_animator.SetTrigger("Injured");
        }

        if (hp <= 0)
        {
            enemystate = EnemyState.death;
            gun.GetComponent<Rigidbody>().useGravity = true;
            //코루틴 
            enemy_animator.SetTrigger("Dead");
            StartCoroutine(disable_collider());
        }

    }

    IEnumerator disable_collider()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

    public void Fire_weapon()
    {
        // 광선을 쏨
        // 근데 좀 랜덤하게
        Vector3 newdir = shoot_coord.forward;
        newdir.x = Random.Range(newdir.x - 1.0f, newdir.x + 1.0f);

        RaycastHit hitinfo;

        int layerMask = 1 << 8;
        // 8번째 layer만 빼고 모두 check
        layerMask = ~layerMask;

        // 발사 이펙트
        shoot_effect.Play();

        if (Physics.Raycast(shoot_coord.position, newdir, out hitinfo, Mathf.Infinity, layerMask))
        {
            if(hitinfo.collider.gameObject.tag == "Player")
                hitinfo.transform.GetComponent<PlayerController>().hit(damage, hitinfo.point);
        }
    }

    public void hit(int damage, Vector3 hitpoint) {

        if (enemystate == EnemyState.death)
            return;

        // 피격 처리
        hp -= damage;
        hit_effect.transform.position = hitpoint;
        hit_effect.Play();
        
        enemy_animator.SetBool("GetHit2", true);
        enemystate = EnemyState.hit;
        // 코루틴
        StartCoroutine(waitseconds_and_set(1.0f, EnemyState.idle, "GetHit2")); 
    }

    IEnumerator waitseconds_and_set(float time, EnemyState newstate, string paraname)
    {
        yield return new WaitForSeconds(time);
        enemystate = newstate;
        enemy_animator.SetBool(paraname, false);
    }



}
