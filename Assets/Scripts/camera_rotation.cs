using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_rotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0.01f,0);
        if (transform.rotation.y == 360) transform.rotation = Quaternion.Euler(0,0,0) ;
    }
}
