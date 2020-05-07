using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    private TextMeshPro textMesh;
    public SaveData save;

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        player = FindObjectOfType<Player>();
        save = FindObjectOfType<SaveData>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            
            textMesh.text = "";
            save.SavePlayer(player.gold, player.GetWeaponLevel());
            SceneManager.LoadScene("Level2");
        }

    }
}
