using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class director : MonoBehaviour
{
    GameObject PlayerChar;
    GameObject DancerChar;
    PlayerLandmarkWebcam PlayerLD;
    DancerLandmarkLoad DancerLD;

    // Start is called before the first frame update
    void Start()
    {
        PlayerChar = GameObject.FindWithTag("Player");
        //DancerChar = GameObject.FindWithTag("Dancer");
        PlayerLD = PlayerChar.GetComponent<PlayerLandmarkWebcam>();
        //DancerLD = DancerChar.GetComponent<DancerLandmarkLoad>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Game_Finish()
    {
        //토탈 스코어 표시. 게임 데모에서는 목록 화면으로 돌아가기.

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif


    }

}
