using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class scoring : MonoBehaviour
{
    int[] keypoint = { 11, 12, 13, 14, 15, 16, 23, 24, 25, 26, 27, 28 };
    int[,] body_vector = { { 11, 13 }, { 12, 14 }, { 13, 15 }, { 14, 16 }
        , { 23, 25 }, { 24, 26 }, { 25, 27 }, { 26, 28 }, { 12, 23 }, { 11, 24 } };
    float[,] dtw_mat;
    float[,,] p_landmark;

    GameObject PlayerChar;
    PlayerLandmarkWebcam PlayerLD;
    public TMP_Text score_txt;
    public TMP_Text score_num;
    Animator score_anim;
    ParticleSystem score_particle;

    // Start is called before the first frame update
    void Start()
    {
        PlayerChar = GameObject.FindWithTag("Player");
        PlayerLD = PlayerChar.GetComponent<PlayerLandmarkWebcam>();
        score_anim = score_txt.GetComponent<Animator>();
        score_particle = score_txt.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DTW_CosSim_Score(float[,,] d_landmark)
    {
        p_landmark = PlayerLD.ret_landmark_stream();
        float[,] similarity;
        float score = 0;
        string scoretext;
        int d_seq = d_landmark.Length / (34 * 3);
        int p_seq = p_landmark.Length / (34 * 3);
        dtw_mat = new float[d_seq, p_seq];

        //각 keypoint 별로 dtw를 실행하여 independent dtw matrix를 산출
        for (int i = 0; i < keypoint.Length; i++)
        {
            dtw_distance(d_landmark, p_landmark, keypoint[i], 0); //x축
            dtw_distance(d_landmark, p_landmark, keypoint[i], 1); //y축
        }

        ////dtw배열 확인용
        //string distmat = "";
        //for (int i = 0; i < d_seq; i++)
        //{
        //    for (int j = 0; j < p_seq; j++) distmat += dtw_mat[i, j] + "\t";
        //    distmat += "\n";
        //}
        //Debug.Log(distmat);

        //통합된 independent dtw matrix를 이용하여 최적의 경로 인덱스를 추출
        int[,] idx_ = Distance_Index(dtw_mat, d_seq - 1, p_seq - 1);
        int end_idx = idx_[0, 0];
        int start_idx = idx_.Length / 2 - end_idx - 1;
        int[,] dist_idx = new int[end_idx + 1, 2];

        for (int i = 0; i <= end_idx; i++)
        {
            dist_idx[i, 0] = idx_[start_idx + i, 0];
            dist_idx[i, 1] = idx_[start_idx + i, 1];
        }

        ////인덱스 확인용
        //string dist = "";
        //for (int i = 0; i < dist_idx.Length / 2; i++)
        //{
        //    dist += dtw_mat[dist_idx[i, 0], dist_idx[i, 1]] + "\t";
        //}
        //Debug.Log(dist);

        //인덱스를 기반으로 스코어링 실행
        similarity = Cosine_Similarity(dist_idx, d_landmark, p_landmark);
        for (int i = 0; i < similarity.GetLength(0); i++)
        {
            for (int j = 0; j < similarity.GetLength(1); j++) score += similarity[i, j];
        }

        score = score / similarity.Length * 100;

        //perfect~bad 판정 필요.
        if (score > 85) scoretext = "Perfect";
        else if (score > 70) scoretext = "good";
        else scoretext = "bad";

        score_txt.text = scoretext;
        score_num.text = score.ToString("F3");
        score_anim.SetTrigger("score_trg");
        score_particle.Play();
        Debug.Log("score = " + score);

    }

    void dtw_distance(float[,,] a, float[,,] b, int point, int mat) // 랜드마크 두 셋, 키포인트, x(0) or y(1)
    {
        int a_seq = a.Length / (34 * 3);
        int b_seq = b.Length / (34 * 3);
        float[,] dist_matrix = new float[a_seq, b_seq];
        float cost, tmp; //거리비용, 인접노드 3칸을 비교하기 위한 임시공간.

        for (int i = 0; i < a_seq; i++)
        {
            for (int j = 0; j < b_seq; j++)
            {
                cost = Math.Abs(a[i, point, mat] - b[j, point, mat]);

                if (i == 0 && j == 0)
                {
                    dist_matrix[i, j] = cost;
                }
                else if (i == 0)
                {
                    dist_matrix[i, j] = cost + dist_matrix[i, j - 1];
                }
                else if (j == 0)
                {
                    dist_matrix[i, j] = cost + dist_matrix[i - 1, j];
                }
                else
                {
                    tmp = Math.Min(dist_matrix[i, j - 1], dist_matrix[i - 1, j - 1]);
                    dist_matrix[i, j] = cost + Math.Min(tmp, dist_matrix[i - 1, j]);
                }
                dtw_mat[i, j] += dist_matrix[i, j]; //통합 배열에 요소 더하기
            }
        }

        ////dtw배열 확인용
        //string distmat = "";
        //for (int i = 0; i < a_seq; i++)
        //{
        //    for (int j = 0; j < b_seq; j++) distmat += dist_matrix[i, j] + "\t";
        //    distmat += "\n";
        //}
        //Debug.Log(distmat);
    }

    int[,] Distance_Index(float[,] matrix, int a, int b)
    {
        int i = 0, length = a + b + 1;
        int[,] idx = new int[length + 1, 2];
        int tmpa, tmpb;

        while (true)
        {
            //Debug.Log(a + "," + b);
            if (a == 0 && b == 0)
            {
                idx[length - i, 0] = a;
                idx[length - i, 1] = b;
                break;
            }
            else if (a == 0)
            {
                idx[length - i, 0] = a;
                idx[length - i, 1] = b--;
            }
            else if (b == 0)
            {
                idx[length - i, 0] = a--;
                idx[length - i, 1] = b;
            }
            else
            {
                if (matrix[a - 1, b] > matrix[a, b - 1])
                {
                    tmpa = a;
                    tmpb = b - 1;
                }
                else
                {
                    tmpa = a - 1;
                    tmpb = b;
                }

                if (matrix[tmpa, tmpb] > matrix[a - 1, b - 1])
                {
                    idx[length - i, 0] = a--;
                    idx[length - i, 1] = b--;
                }
                else
                {
                    idx[length - i, 0] = a;
                    idx[length - i, 1] = b;
                    a = tmpa;
                    b = tmpb;
                }
            }
            i++;
        }
        idx[0, 0] = i;
        return idx;
    }

    float[,] Cosine_Similarity(int[,] idx, float[,,] a, float[,,] b)
    {
        //dtw로 매칭 된 인덱스를 사용하여 벡터 10쌍의 유사도 계산
        float[,] similarity = new float[idx.Length / 2, 10];
        Vector2 vec_a;
        Vector2 vec_b;

        for (int i = 0; i < idx.Length / 2; i++)
        {
            for (int j = 0; j < 10; j++)
            {   //프레임인덱스,랜드마크키포인트,x or y
                vec_a = new Vector2(a[idx[i, 0], body_vector[j, 1], 0] - a[idx[i, 0], body_vector[j, 0], 0]
                    , a[idx[i, 0], body_vector[j, 1], 1] - a[idx[i, 0], body_vector[j, 0], 1]);
                vec_b = new Vector2(b[idx[i, 1], body_vector[j, 1], 0] - b[idx[i, 1], body_vector[j, 0], 0]
                    , b[idx[i, 1], body_vector[j, 1], 1] - b[idx[i, 1], body_vector[j, 0], 1]);
                similarity[i, j] = cos_sim(vec_a, vec_b);
            }
        }
        return similarity;
    }

    float cos_sim(Vector2 v1, Vector2 v2)
    {
        return Vector2.Dot(v1, v2) / (v1.magnitude * v2.magnitude);
    }
}
