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
    private DataKeeper dataKeeper;

    [SerializeField]
    private string nextLevel = "Level2";
    private Player player;
    
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        player = FindObjectOfType<Player>();
        dataKeeper = FindObjectOfType<DataKeeper>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var eventData = new Dictionary<string, object>();
            eventData.Add("position", gameObject.transform.position);
            eventData.Add("timeOnLevel", Time.timeSinceLevelLoad);
            eventData.Add("timeSinceGameStart", Time.time);
            eventData.Add("weaponLevel", player.GetWeaponLevel());
            AnalyticsEvent.LevelComplete(SceneManager.GetActiveScene().name, eventData);
                
            if (!string.IsNullOrEmpty(nextLevel))
            {
                textMesh.text = "";
                dataKeeper.SetPlayerStats(player);
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
