using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataKeeper : MonoBehaviour
{

    public static DataKeeper instance;
    
    [SerializeField]
    private int gold;
    [SerializeField]
    private int weaponLevel;

    public void SetPlayerStats(Player player)
    {
        gold = player.GetGold();
        weaponLevel = player.GetWeaponLevel();
    }

    public void LoadPlayer(Player player)
    {
        player.SetGold(gold);
        player.UpgradetWeaponLevel(weaponLevel);
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
