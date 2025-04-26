using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSale : ItemSale
{

    public int weaponLevel = 1;
    // Start is called before the first frame update
    public new void Start()
    {
        var player = FindAnyObjectByType<Player>();
        if (player.GetWeaponLevel() >= weaponLevel)
        {
            gameObject.SetActive(false);
        }
        else
        {
            base.Start();
        }
        
    }
}
