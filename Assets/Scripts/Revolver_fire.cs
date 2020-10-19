using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Revolver_fire : MonoBehaviour
{
    private weapons weaponInfo = new weapon_revolver();
    // 상속할까.. 다중상속도 안되고 진짜.
    private int maxshootammo = 0;
    private int maxleftammo = 0;
    // 장전됨 + 남은 탄약 포함
    private int shootammo;
    private int leftammo;
    private float cooltime;
    // 한발당 데미지
    private int damage;


    public SteamVR_Action_Boolean shoot_act;
    // 마음같아선 손을 흔들어서 재장전 하고 싶은데 자료가 진짜 없다..
    // 결국 키누름으로 대체
    public SteamVR_Action_Boolean reload_act;
    // 왼손 or 오른손?
    public SteamVR_Input_Sources handType;
    public ParticleSystem gunfire_smoke;
    public Transform shootpoint;
    private Animator revolver_animator = null;

    private Interactable interactable;
    private Rigidbody revolver_rigidbody = null;

    // 원래 있어야 할 총알 홀스터의 transform
    public Transform bullet_origin_transform; 

    public TextMeshProUGUI ammotext;

    private void Start()
    {
        // 콜백함수를 안에 넣어서 호출
        shoot_act.AddOnStateDownListener(shoot, handType);
        reload_act.AddOnStateDownListener(reloadpose, handType);
        revolver_animator = gameObject.GetComponent<Animator>();

        maxshootammo = weaponInfo.shootammo;
        maxleftammo = weaponInfo.leftammo;
        cooltime = weaponInfo.cooltime;
        damage = weaponInfo.damage;

        shootammo = maxshootammo;
        leftammo = maxleftammo;

        interactable = GetComponent<Interactable>();
        revolver_rigidbody = gameObject.GetComponent<Rigidbody>();

        revolver_animator.SetBool("doing_fire", false);
    }
    private void shoot(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        //Debug.Log("shoot func!");

        // 남은 장탄이 0발이거나 reload 중, 발사 중, 유휴 중이라면 skip
        if (shootammo > 0 && PlayerController.Instance.state == PlayerController.playerState.grapweapon)
        {

            shootammo--;
            ammotext.text = shootammo + " / " + leftammo;

            RaycastHit hitinfo;
            int layerMask = 1 << 8;
            // 8번째 layer만 빼고 모두 check
            layerMask = ~layerMask;

            gunfire_smoke.Play();

            // 무언가 총알에 맞음!
            if (Physics.Raycast(shootpoint.position, shootpoint.forward, out hitinfo, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(shootpoint.position, shootpoint.forward * hitinfo.distance, Color.yellow);
               
                if (hitinfo.collider.gameObject.tag.Contains("Enemy"))
                {
                    // enemy 클래스는 만들 예정.
                    Destroy(hitinfo.collider.gameObject);
                }
            }

            // 총 발사 애니메이션 재생
            PlayerController.Instance.state = PlayerController.playerState.fire;
            revolver_animator.SetBool("doing_fire", true);
            //// 코루틴으로 1초를 기다립시다. 그다음 해제
            StartCoroutine(waitseconds_and_set(cooltime));

        }
    }

    IEnumerator waitseconds_and_set(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerController.Instance.state = PlayerController.playerState.grapweapon;
        revolver_animator.SetBool("doing_fire", false);
    }

    private void reloadpose(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        if (PlayerController.Instance.state == PlayerController.playerState.grapweapon || PlayerController.Instance.state == PlayerController.playerState.reload)
        {
            // 애니메이션 변경
            if (PlayerController.Instance.state == PlayerController.playerState.grapweapon)
            {
                PlayerController.Instance.state = PlayerController.playerState.reload;
                revolver_animator.SetBool("should_reload", true);
            }
            // 장전을 다 한다음 다시 키를 누르면 다시 들어감.
            else if (PlayerController.Instance.state == PlayerController.playerState.reload)
            {
                PlayerController.Instance.state = PlayerController.playerState.grapweapon;
                revolver_animator.SetBool("should_reload", false);
            }

        }

    }

    // 실제 reload 처리
    // UI 변경 포함
    public void reload(GameObject hitother)
    {
        // hitorder : 탄약
        // 탄약도 제자리에 갖다 놓아야 한다.
        // 손으로 탄약 집은 것을 풀어야 하는데..??
        // 충돌하는 순간 ammo 늘리고, 탄약집으로 순간이동하는 처리로 하자.
        // 한발씩 장전.

        if (shootammo == maxshootammo || leftammo == 0)
            return;

        shootammo++;
        leftammo--;
        ammotext.text = shootammo + " / " + leftammo;

        // 총알 원위치
        hitother.transform.SetPositionAndRotation(bullet_origin_transform.position, bullet_origin_transform.rotation);
    }

    // 물체를 들었을 때 호출됨.
    // Main scene에서 홀스터에 차면 kinematic과 gravity 체크 변경되므로 다시 해줄 것.

    private void OnAttachedToHand(Hand hand)
    {
        PlayerController.Instance.state = PlayerController.playerState.grapweapon;
        revolver_rigidbody.isKinematic = false;
        revolver_rigidbody.useGravity = true;

    }

    private void OnDetachedFromHand(Hand hand)
    {
        Debug.Log("OnDetachedFromHand revolver");
        if (PlayerController.Instance.weapon_should_locked)
        {
            gameObject.transform.parent.GetComponentInChildren<weapon_receptor>().fix_weapon();
            PlayerController.Instance.weapon_should_locked = false;
        }
        PlayerController.Instance.state = PlayerController.playerState.idle;
    }
    //private void OnHandHoverBegin(Hand hand)
    //{
    //    Debug.Log("OnHandHoverBegin called!");
    //    // player 상태 변경
    //    if(PlayerController.Instance.state == PlayerController.playerState.grapweapon)
    //        PlayerController.Instance.state = PlayerController.playerState.idle;
    //    else
    //        PlayerController.Instance.state = PlayerController.playerState.grapweapon;
    //}

    //private void OnHandHoverEnd(Hand hand)
    //{
    //    Debug.Log("OnHandHoverEnd called!");
    //    //// player 상태 변경
    //    //PlayerController.Instance.state = PlayerController.playerState.idle;
    //}

    //private void OnHandHoverUpdate(Hand hand)
    //{
    //    //// player 상태 변경

    //    //if(hand.GetGrabStarting() == GrabTypes.None)
    //    //    PlayerController.Instance.state = PlayerController.playerState.idle;
    //    //else
    //    //    PlayerController.Instance.state = PlayerController.playerState.grapweapon;

    //}
}
