using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

    
    
public class ScoreKeeper : Singleton<ScoreKeeper>
{
    
    public TextMeshProUGUI statsText;
    
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        statsText.SetText($"HP: {player.hp} gold: {player.gold} Keys:  {player.Keys} ");
    }
}
