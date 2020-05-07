using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

    
    
public class GameManager : Singleton<GameManager>
{
    
    private TextMeshProUGUI statsText;

    public int dummy = 20;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        
        player = FindObjectOfType<Player>();
        
        statsText = GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(statsText.name);
    }

    // Update is called once per frame
    void Update()
    {
        statsText.SetText($"HP: {player.hp} gold: {player.gold} Keys:  {player.Keys} ");
    }
}
