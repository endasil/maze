using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed = 10.0f;

    public int power = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            return;
        }

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
