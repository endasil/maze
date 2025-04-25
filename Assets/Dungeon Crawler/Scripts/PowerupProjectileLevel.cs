using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupProjectileLevel : MonoBehaviour
{
    public AudioClip pickupSound;

    [SerializeField] private int level;

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
         
            var player = other.gameObject.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (player.UpgradedWeaponLevel(1))
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

                Destroy(gameObject);
            }
        }
    }
}
