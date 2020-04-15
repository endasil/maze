using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    private static int goldsTaken = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        goldsTaken++;
        Debug.Log("Gold taken: " + goldsTaken);
        Destroy(gameObject);

    }
}
