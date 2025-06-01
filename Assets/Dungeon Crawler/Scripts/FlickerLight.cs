using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{

    public int seedOffset;
    void Start()
    {
        seedOffset = Random.Range(1, 999999);
        Light light = GetComponent<Light>();
        light.type = LightType.Point;
//        light.range = 5f;
    }
    void Update()
    {
        GetComponent<Light>().intensity = Mathf.PerlinNoise(Time.time + seedOffset, 0) *3;
        //GetComponent<Light>().range  = 5 + Mathf.PerlinNoise(Time.time, 0);
    }
    
}
