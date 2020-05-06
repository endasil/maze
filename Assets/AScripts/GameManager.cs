using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

    
    
public class GameManager : Singleton<GameManager>
{
    
    public TextMeshProUGUI statsText;

    public int dummy = 20;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        statsText.SetText($"HP: {player.hp} gold: {player.gold} Keys:  {player.Keys} ");
    }
}
