using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DancerLandmarkLoad : MonoBehaviour
{
    private director director;
    private CharacterController char_controller;
    private scoring score;
    public string song_name;
    string[] textValue;

    int sequence_len = 30;
    float[,,] landmark_stream;
    float[,] one_landmark;

    float delay=0;
    float start_time;
    int total_frame;
    int frame_count = 0 ;
    int sub_frame = 0;

    float tmp;
    
    int scale = -1;
    bool is_init = true;

    // Start is called before the first frame update
    void Start()
    {
        //댄서 랜드마크 취득
        string path = @"Assets//Scripts//result//" + song_name + ".txt";
        textValue = System.IO.File.ReadAllLines(path);
        total_frame = textValue.Length;

        if (total_frame > 0)
        {
            Debug.Log("landmark Reading success");
        }
        else
        {
            Debug.Log("landmark Reading failed. Please Try Again!");
            Application.Quit();
        }

        char_controller = GetComponent<CharacterController>();
        score = GameObject.Find("score calculator").GetComponent<scoring>();
        director = GameObject.Find("Director").GetComponent<director>();
        landmark_stream = new float[sequence_len, 34, 3];

        start_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-start_time > delay) // 프레임 딜레이 조절
        {
            //Debug.Log("delay time = "+ (Time.time - start_time));
            start_time = Time.time;

            one_landmark = ret_landmark(textValue);
            tmp = Time.time;
            //준비된 랜드마크 스트림 중 한 프레임을 취득하여 모션캡처.
            //간소화 필요해보임...
            if (is_init)
            {
                char_controller.set_initpos(one_landmark);
                is_init = false;
            }
            char_controller.PoseUpdate(char_controller.Init(), one_landmark);

            frame_count++;
            sub_frame++;
            if (sub_frame == sequence_len)
            {
                sub_frame = 0;
                //스코어링 실행
                score.DTW_CosSim_Score(landmark_stream);
            }

            //Debug.Log("tmp = " + (Time.time - tmp));
        }
        //프레임이 끝나면 종료하는 코드 필요
        if(frame_count == total_frame)
        {
            Debug.Log("game finished");
            director.Game_Finish();
        }

    }

    public float[,] ret_landmark(string[] textValue)
    {
        float[,] landmark = new float[34, 3];
        float tmp;
        float fps =0 ;

        textValue[frame_count] = textValue[frame_count].Replace("[", "");
        textValue[frame_count] = textValue[frame_count].Replace("]", "");
        textValue[frame_count] = textValue[frame_count].Replace("{", "");
        textValue[frame_count] = textValue[frame_count].Replace("}", "");

        string[] splited = textValue[frame_count].Split(',');
        fps = Convert.ToSingle(splited[0]);
        for (int j = 0; j < 34; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                tmp = Convert.ToSingle(splited[j * 3 + k + 1]) * scale;
                landmark_stream[sub_frame, j, k] = tmp;
                landmark[j, k] = tmp;
            }
        }

        delay = fps / 1000;
        //Debug.Log("delay = "+delay);

        return landmark;
    }
}
