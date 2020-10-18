using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour
{
    public SteamVR_Action_Vector2 input;
    public float speed = 1;
    private CharacterController charactercontroller;

    public bool Isbulletloaded = false; // 한번에 많이 장전하는 것을 막는다.

    // 싱글톤
    private static PlayerController instance = null;

    // 홀스터
    public GameObject ammo_holster;
    public GameObject weapon_holster;
    public Transform VR_camera;

    /// <summary>
    /// 지금 플레이어가 무기에 대해 어떤 상태인지 나타낸다.
    /// </summary>
    public enum playerState {idle, grapweapon, reload, fire};
    public playerState state;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
                return null;
            else
                return instance;
        }
    }

    private void Start()
    {
        charactercontroller = GetComponent<CharacterController>();
        state = playerState.idle;

        //ammo_holster = transform.Find("ammo_holster").gameObject;
        //weapon_holster = transform.Find("weapon_holster").gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        // 위아래로는 움직이지 않는다..
        // 나중에 rigidbody나 character controller 추가하면 어떻게 되겠지..?
        Vector3 direction = Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
        // 중력에 맞게..
        charactercontroller.Move(speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up) - new Vector3(0, 9.81f, 0) * Time.deltaTime);


        // 홀스터들 같이 이동, 회전
        // 만약 재장전을 시도한다면 고정시키자..!
        if (PlayerController.instance.state == playerState.reload)
            return;

        Vector3 criteria_pos = VR_camera.transform.TransformPoint(new Vector3(-0.2f, -0.3f, 0));
        Vector3 criteria_angle = VR_camera.transform.eulerAngles;
        ammo_holster.transform.position = criteria_pos;
        ammo_holster.transform.rotation = Quaternion.Euler(90, criteria_angle.y, criteria_angle.z);

        criteria_pos = VR_camera.transform.TransformPoint(new Vector3(0.2f, -0.3f, 0));
        weapon_holster.transform.position = criteria_pos;
        weapon_holster.transform.rotation = Quaternion.Euler(90, criteria_angle.y, criteria_angle.z);
    }
}
