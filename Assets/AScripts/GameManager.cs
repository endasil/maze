using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager>
{
    
    private TextMeshProUGUI statsText;
    public int dummy = 20;
    private Player player;
    private Scrollbar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        
        statsText = GetComponentInChildren<TextMeshProUGUI>();
        healthBar = GetComponentInChildren<Scrollbar>();
        Debug.Log(statsText.name);
    }

    // Update is called once per frame
    void Update()
    {
        statsText.SetText($"{player.Keys} Gold: {player.gold}");
        healthBar.size = (float)player.hp / player.maxHealth;
    }
}
