using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 2.0f;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(new Vector3(0,0,0), new Vector3(0,1,0), speed);
    }
}
