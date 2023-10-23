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
        //�� ���帶ũ ���
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
        if(Time.time-start_time > delay) // ������ ������ ����
        {
            //Debug.Log("delay time = "+ (Time.time - start_time));
            start_time = Time.time;

            one_landmark = ret_landmark(textValue);
            tmp = Time.time;
            //�غ�� ���帶ũ ��Ʈ�� �� �� �������� ����Ͽ� ���ĸó.
            //����ȭ �ʿ��غ���...
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
                //���ھ ����
                score.DTW_CosSim_Score(landmark_stream);
            }

            //Debug.Log("tmp = " + (Time.time - tmp));
        }
        //�������� ������ �����ϴ� �ڵ� �ʿ�
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
