using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    
    private TextMeshPro textMesh;
    private SaveData save;

    [SerializeField]
    private string nextLevel = "Level2";
    private Player player;
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
            if (nextLevel != "" && nextLevel != null)
            {
                textMesh.text = "";
                save.SavePlayer(player.gold, player.GetWeaponLevel());
                SceneManager.LoadScene(nextLevel);
            }
            else
            {
                textMesh.text = "You won!";
                Destroy(player);
            }
        }

    }
}
