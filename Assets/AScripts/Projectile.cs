using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 10.0f;

    public int power = 10;
    public float range = 10;
    public AudioClip wallImpact;
    [SerializeField]
    private Vector3 startPosition;
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
        if (other.tag == "Player" || other.tag == "Ground")
        {
            return;
        }
        AudioSource.PlayClipAtPoint(wallImpact, transform.position);
        var damagable = other.gameObject.GetComponent<DamagableObject>();
        if (damagable)
        {
            damagable.TakeDamage(power);

        }
        //if (other.tag == "Wall")
        //{
            Destroy(gameObject);
        //}
    }
}
