using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ms_director : MonoBehaviour
{
    private MapNumber song_number;

    // Start is called before the first frame update
    void Start()
    {
        song_number = GameObject.Find("MapNumber").GetComponent<MapNumber>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Map1_Select()
    {
        song_number.map_number = 0;
        SceneManager.LoadScene("Stage Scene");
    }

    public void Map2_Select()
    {
        song_number.map_number = 1;
        SceneManager.LoadScene("Stage Scene");
    }

    public void Map3_Select()
    {
        song_number.map_number = 2;
        SceneManager.LoadScene("Stage Scene");
    }
}
