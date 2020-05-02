using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private TextMeshPro textMesh;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Destroy(other.gameObject);
            textMesh.text = "You won!";
            SceneManager.LoadScene("Level2");
        }

    }
}
