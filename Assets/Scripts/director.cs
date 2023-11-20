using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class director : MonoBehaviour
{
    GameObject PlayerChar;
    GameObject DancerChar;
    PlayerLandmarkWebcam PlayerLD;
    DancerLandmarkLoad DancerLD;
    GameObject connect_txt;

    public AudioClip[] music = new AudioClip[3];
    bool player_connect_tog = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerChar = GameObject.FindWithTag("Player");
        //DancerChar = GameObject.FindWithTag("Dancer");
        PlayerLD = PlayerChar.GetComponent<PlayerLandmarkWebcam>();
        //DancerLD = DancerChar.GetComponent<DancerLandmarkLoad>();
        int song_number = GameObject.Find("MapNumber").GetComponent<MapNumber>().map_number;

        PlayerChar.GetComponent<AudioSource>().clip = music[song_number];

        connect_txt = GameObject.Find("connect_Error");
        connect_txt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Game_Finish()
    {
        //토탈 스코어 표시. 게임 데모에서는 목록 화면으로 돌아가기.
        //종료 전 웹캠 끌 것.
        PlayerLD.Socket_Close();
        SceneManager.LoadScene("Map Select Scene");
    }

    public void Connect_Error()
    {
        float start_time = Time.time;

        player_connect_tog = false;
        //플레이어 웹 캠 오류 시 해당 상황을 알리며 재 실행
        UnityEngine.Debug.Log("랜드마크 서버와의 연결이 끊겼습니다.");
        //PlayerChar.SetActive(false);
        connect_txt.SetActive(true);
        
        //웹캠 재실행
        //PlayerChar.SetActive(true);
        PlayerLD.webcam_Process_Start();
        connect_txt.SetActive(false);
    }

    public void Connect_Success()
    {
        player_connect_tog = true;
    }

}
