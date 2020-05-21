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
    [SerializeField] private DamagableObject boss;
    private Scrollbar healthBar;
    private Scrollbar bossHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        statsText = GetComponentInChildren<TextMeshProUGUI>();
        Transform canavas = GetComponentInChildren<Canvas>().transform;
        healthBar = canavas.Find("PlayerHealthBar").gameObject.GetComponentInChildren<Scrollbar>();
        if (boss)
        {
            bossHealthBar = canavas.Find("BossHealthBar").gameObject.GetComponentInChildren<Scrollbar>();
        }

        Debug.Log(statsText.name);
    }

    // Update is called once per frame
    void Update()
    {
        statsText.SetText($"{player.Keys} Gold: {player.gold}");
        healthBar.size = (float)player.hp / player.maxHealth;
        
        
        if (bossHealthBar)
        {
            bossHealthBar.size = (float)boss.hp / boss.maxHealth;
            if (boss.hp <= 0)
            {
                Destroy(bossHealthBar.gameObject);
            }
            
        }
    }
}
