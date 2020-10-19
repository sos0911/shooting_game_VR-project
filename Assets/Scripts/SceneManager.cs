using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{

    // 싱글톤
    private static SceneManager instance = null;
    // 게임오버 UI
    public Canvas GameOverUI;

    public bool Isgameover = false;
    public bool AllEnd = false;

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
    public static SceneManager Instance
    {
        get
        {
            if (instance == null)
                return null;
            else
                return instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AllEnd)
            return;

        if (Isgameover)
        {
            // 게임오버 UI 등장 & 플레이어 정지 & 시간 정지
            // 재시작 기능은 없다.
            Time.timeScale = 0;
            // 이거 손봐야 하는데..
            PlayerController.Instance.gameObject.SetActive(false);
            Transform criteria_trans = PlayerController.Instance.VR_camera;
            Instantiate(GameOverUI, criteria_trans.position, criteria_trans.rotation);
            AllEnd = true;
        }
    }
}
