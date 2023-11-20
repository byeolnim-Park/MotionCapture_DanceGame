using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DancerLandmarkLoad : MonoBehaviour
{
    private director director;
    private CharacterController char_controller;
    private scoring score;
    private int song_number;
    string[] textValue;

    string[] song_name = { "Mary Had A Little Lamb", "Springtime Family Band", "Robot Boogie" };
    int[] sequence_len = { 42,37,30 }; //**
    int[] start_frame = { 45,80,70 }; //**
    float[,,] landmark_stream;
    float[,] one_landmark;

    float delay=30;
    float init_time;
    float start_time;

    int total_frame; //�� ���帶ũ �� ������*

    int scoring_count = 0; //���ھ Ƚ��
    int current_frame = 0; //�� ���帶ũ �� ���� ������
    
    int scale = -1;
    bool is_init = true;

    // Start is called before the first frame update
    void Start()
    {
        song_number = GameObject.Find("MapNumber").GetComponent<MapNumber>().map_number;
        //�� ���帶ũ ���
        string path = @"Assets//Scripts//result//" + song_name[song_number] + "_landmarks.txt";
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

        landmark_stream = new float[sequence_len[song_number], 34, 3];
        one_landmark = ret_landmark(textValue, 0);

        init_time = Time.time;
        start_time = init_time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-start_time > delay/1000) // ������ ������ ����
        {
            //Debug.Log("dancer Delay = "+ (Time.time - start_time));
            start_time = Time.time;
            current_frame = Convert.ToInt32((Time.time-init_time) * delay) + start_frame[song_number];
            Debug.Log("frame: " + current_frame +" / "+total_frame + " / time: "+Time.time);

            if (current_frame >= total_frame)
            {
                Debug.Log(score.ret_final_score());
                Debug.Log("game finished");
                director.Game_Finish();
            }

            one_landmark = ret_landmark(textValue, current_frame);

            //�غ�� ���帶ũ ��Ʈ�� �� �� �������� ����Ͽ� ���ĸó.
            if (is_init)
            {
                char_controller.set_initpos(one_landmark);
                is_init = false;
            }
            char_controller.PoseUpdate(char_controller.Init(), one_landmark);

            if ((current_frame - start_frame[song_number]) / sequence_len[song_number] == (scoring_count+1)) 
            {
                //���ھ ����
                landmark_stream = ret_landmark_stream(textValue, scoring_count* sequence_len[song_number]);
                score.DTW_CosSim_Score(landmark_stream);
                scoring_count++;
            }
            //�������� ������ ����
        }
    }

    //���ĸó�� ���� �� ���帶ũ ����
    public float[,] ret_landmark(string[] textValue, int idx)
    {
        float[,] landmark = new float[34, 3];
        float tmp;
        float fps =0 ;

        textValue[idx] = textValue[idx].Replace("[", "");
        textValue[idx] = textValue[idx].Replace("]", "");
        textValue[idx] = textValue[idx].Replace("{", "");
        textValue[idx] = textValue[idx].Replace("}", "");

        string[] splited = textValue[idx].Split(',');
        fps = Convert.ToSingle(splited[0]);
        for (int j = 0; j < 34; j++)
        {
            for (int k = 0; k < 3; k++)
            {
                tmp = Convert.ToSingle(splited[j * 3 + k + 1]) * scale;
                landmark[j, k] = tmp;
            }
        }

        //delay = fps ;  //���̽㿡�� �߸� ���Ǿ� ����!!
        //Debug.Log("delay = "+delay);

        return landmark;
    }

    //���ھ�� ���� �� ��������ŭ ���帶ũ ����
    public float[,,] ret_landmark_stream(string[] textValue, int idx)
    {
        float[,,] landmark = new float[sequence_len[song_number], 34, 3];
        float tmp;
        float fps = 0;
        
        for(int i = 0; i < sequence_len[song_number]; i++)
        {
            textValue[idx+i] = textValue[idx+i].Replace("[", "");
            textValue[idx+i] = textValue[idx+i].Replace("]", "");
            textValue[idx+i] = textValue[idx+i].Replace("{", "");
            textValue[idx+i] = textValue[idx+i].Replace("}", "");

            string[] splited = textValue[idx+i].Split(',');
            fps = Convert.ToSingle(splited[0]);
            for (int j = 0; j < 34; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    tmp = Convert.ToSingle(splited[j * 3 + k + 1]) * scale;
                    landmark[i, j, k] = tmp;
                }
            }
        }

        return landmark;
    }
}
