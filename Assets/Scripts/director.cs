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
        //��Ż ���ھ� ǥ��. ���� ���𿡼��� ��� ȭ������ ���ư���.

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif


    }

}
