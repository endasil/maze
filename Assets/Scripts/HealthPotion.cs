using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int power = 25;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }


    private void OnTriggerEnter(Collider other)
    {
        var damagable = other.gameObject.GetComponent<DamagableObject>();
        if (damagable)
        {
            damagable.Heal(power);

        }
        //if (other.tag == "Wall")
        //{
        Destroy(gameObject);
        //}
    }
}
