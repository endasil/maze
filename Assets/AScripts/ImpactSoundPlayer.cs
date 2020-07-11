using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundPlayer : MonoBehaviour
{
    public AudioClip impactSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collisoin sound");
        if (other.CompareTag("PlayerProjectile"))
        {
            AudioSource.PlayClipAtPoint(impactSound, transform.position);
        }
    }
}