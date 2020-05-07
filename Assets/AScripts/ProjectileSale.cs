using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSale : MonoBehaviour
{

    public int weaponLevel = 1;
    // Start is called before the first frame update
    void Start()
    { 
        if(SaveData.instance.weaponLevel >= weaponLevel)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
