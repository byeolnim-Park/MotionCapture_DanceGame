using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System;


public class PlayerLandmarkWebcam : MonoBehaviour
{
    private CharacterController char_controller;
    private director director;
    private AudioSource music;
    float[,] landmark = new float[34, 3];
    float[,,] landmark_stream = new float[50, 34, 3];

    float delay = 0.0f;
    float start_time;
    int scale = -1;
    string[] textValue;
    Socket client;
    bool is_init = true;

    int sub_frame = 0;

    Process process;

    // Start is called before the first frame update
    void Start()
    {
        process = new Process();
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.FileName = @"python";
        //process.StartInfo.Arguments = @"Assets/StreamingAssets/webcam_landmark_socket_server.py";
        process.StartInfo.Arguments = Application.streamingAssetsPath + "/webcam_landmark_socket_server.py";
        process.Start();
        //Thread.Sleep(1000); // 모든 유니티 프로세스가 멈춰버림!!
        webcam_Process_Start();
        char_controller = GetComponent<CharacterController>();
        music = GetComponent<AudioSource>();
        director = GameObject.Find("Director").GetComponent<director>();
        start_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (client.Connected == true)
        {
            //UnityEngine.Debug.Log("player_delay = "+ (Time.time - start_time)); //fps 확인용
            //start_time = Time.time;
            music.UnPause();
            director.Connect_Success();
            var data = Encoding.UTF8.GetBytes("this message is sent from C# client. Please send landmark data.");

            client.Send(BitConverter.GetBytes(data.Length));
            client.Send(data);

            data = new byte[4];
            client.Receive(data, data.Length, SocketFlags.None);
            Array.Reverse(data);
            data = new byte[BitConverter.ToInt32(data, 0)];
            client.Receive(data, data.Length, SocketFlags.None);

            if (Encoding.UTF8.GetString(data).Equals("not found")) { }
            else
            {
                //랜드마크 추출 및 시퀀스 저장
                landmark = ret_landmark(Encoding.UTF8.GetString(data));
                sub_frame++;

                //float[,,] landmark = new float[sequence_len, 34, 3];
                if (is_init)
                {
                    music.Play();
                    char_controller.set_initpos(landmark);
                    is_init = false;
                }
                char_controller.PoseUpdate(char_controller.Init(), landmark);
            }
        }
        else
        {
            //게임이 끝나서 소켓 통신을 끊은 경우.
            music.Pause(); //왜안되..
            client.Close();
            director.Connect_Error();
        }
    }

    public void webcam_Process_Start()
    {
        //플레이어 랜드마크 취득
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));
    }

    //랜드마크셋 순서를 입력으로 해당 순서의 랜드마크 반환
    public float[,] ret_landmark(string textValue)
    {
        float[,] landmark_ = new float[34, 3];
        float tmp;

        textValue = textValue.Replace("[", "");
        textValue = textValue.Replace("]", "");
        textValue = textValue.Replace("{", "");
        textValue = textValue.Replace("}", "");

        string[] splited = textValue.Split(',');
        delay = Convert.ToSingle(splited[0]);
        for (int j = 0; j < 34; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                tmp = Convert.ToSingle(splited[j * 3 + k + 1]) * scale;
                landmark_stream[sub_frame, j, k] = tmp;
                landmark_[j, k] = tmp;
            }
        }

        return landmark_;
    }

    //스코어링 신호 발생 시 랜드마크를 반환
    public float[,,] ret_landmark_stream()
    {
        //남은 공간 지우기
        float[,,] ld_stream = new float[sub_frame + 1, 34, 3];

        for(int i=0;i<=sub_frame; i++)
        {
            for (int j = 0; j < 34; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    ld_stream[i, j, k] = landmark_stream[i, j, k];
                }
            }
        }

        sub_frame = 0 ;

        return ld_stream;
    }

    public void Socket_Close()
    {
        UnityEngine.Debug.Log("Socket_Close");
        client.Close();
        process.Kill();
    }

}
