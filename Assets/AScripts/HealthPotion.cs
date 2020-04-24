using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int power = 25;

    public AudioClip pickupSound;
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
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            damagable.Heal(power);

        }
    
        Destroy(gameObject);
        
    }
}
