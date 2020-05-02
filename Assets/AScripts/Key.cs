using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public AudioClip pickupSound;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider other)
    {

        // Todo make player object pick up instead
        if (other.gameObject.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);


            var player = other.gameObject.GetComponent<Player>();
            player.Keys++;
            Destroy(gameObject);
        }
    }
}