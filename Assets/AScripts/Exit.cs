using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var eventData = new Dictionary<string, object>();
            eventData.Add("position", gameObject.transform.position);
            eventData.Add("timeOnLevel", Time.timeSinceLevelLoad);
            eventData.Add("timeSinceGameStart", Time.time);
            eventData.Add("weaponLevel", player.weaponLevel);
            AnalyticsEvent.LevelComplete(SceneManager.GetActiveScene().name, eventData);
                
            if (!string.IsNullOrEmpty(nextLevel))
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
