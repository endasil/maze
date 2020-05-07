using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{

    public static SaveData instance;
    
    [SerializeField]
    private int gold;
    [SerializeField]
    public int weaponLevel;

    public void SavePlayer(int savegold, int saveWeaponLevel)
    {
        gold = savegold;
        weaponLevel = saveWeaponLevel;
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
            DestroyObject(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
