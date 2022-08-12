using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingBalls : MonoBehaviour
{
    private List<GameObject> _balls = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 16; i++)
        {
            CreateBall(i);
        }
    }

    private void CreateBall(int i)
    {
        _balls.Add(new GameObject("Ball_" + i, typeof(Shape)));
        _balls.Last().GetComponent<Shape>().shapeType = Shape.ShapeType.Sphere;
        _balls.Last().GetComponent<Shape>().operation = Shape.Operation.Blend;
        _balls.Last().GetComponent<Shape>().blendStrength = 0.4f;
        _balls.Last().transform.position = new Vector3(Random.Range(1, 5), Random.Range(1, 5), 2);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _balls.Count; i++)
        {
            float t = Time.fixedTime;

            float ec = 0.05f;
            _balls[i].transform.position += ec * new Vector3(Mathf.Cos(t * 1.1f + (0.3f*i)), Mathf.Cos(t * 1.3f - (0.3f * i)), Mathf.Cos(t * 1.7f + (0.3f * i)));
            //float s1 = sphere(pos - ec * vec3(cos(t * 1.1), cos(t * 1.3), cos(t * 1.7)));
            //float s2 = sphere(pos + ec * vec3(cos(t * 0.7), cos(t * 1.9), cos(t * 2.3)));
            //float s3 = sphere(pos + ec * vec3(cos(t * 0.3), cos(t * 2.9), sin(t * 1.1)));
            //float s4 = sphere(pos + ec * vec3(sin(t * 1.3), sin(t * 1.7), sin(t * 0.7)));
            //float s5 = sphere(pos + ec * vec3(sin(t * 2.3), sin(t * 1.9), sin(t * 2.9)));
        }
    }

    public float Frac(float value) { return (float)(value - Math.Truncate(value)); }
}
