using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Gold : MonoBehaviour
{
    public AudioClip pickupSound;
    public int goldValue = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        
        // Todo make player object pick up instead
        if(other.gameObject.tag == "Player")
        {

            if (pickupSound)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            var player = other.gameObject.GetComponent<Player>();
            player.GiveGold(goldValue);
            Destroy(gameObject);
        }
        

    }
}
