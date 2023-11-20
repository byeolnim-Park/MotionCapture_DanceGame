using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNumber : MonoBehaviour
{
    public int map_number;
    private void Awake()
    {
        var obj = FindObjectsOfType<MapNumber>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
