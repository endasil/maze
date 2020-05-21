using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 10.0f;

    public int power = 10;
    public float range = 10;
    
    public AudioClip wallImpact;
    public AudioClip damagableImpact;
    [SerializeField]
    private Vector3 startPosition;

    public string ownerTag = "Player";
    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
    }

    
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
        float distance = Vector3.Distance(startPosition, transform.position);
        if (distance > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter" + other.tag);
        if (other.tag == ownerTag || other.tag == "Ground" || other.tag == "Treasure" || other.gameObject.layer == 2)
        {
            return;
        }
        
        var damagable = other.gameObject.GetComponent<DamagableObject>();
        
        
        if (damagable)
        {
            

            // Play a default sound when hitting something that can be damaged to ensure there is always
            // feedback sound even if there is nothing specific for the object.
            if (damagable.hitSounds.Count == 0)
            {
                AudioSource.PlayClipAtPoint(damagableImpact, transform.position);
            }
            damagable.TakeDamage(power);
        }
        else
        {
            AudioSource.PlayClipAtPoint(wallImpact, transform.position);
        }
        Debug.Log("Projectile destroyed by " + other.name);
        //if (other.tag == "Wall")
        //{
            Destroy(gameObject);
        //}
    }
}
