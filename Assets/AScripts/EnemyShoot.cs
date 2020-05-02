using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{

    [SerializeField]
    GameObject projectile;

    private Transform target;

    [SerializeField]
    Transform shootPoint;

    [SerializeField]
    float turnSpeed = 5f;

    float fireRate = 0.2f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        fireRate -= Time.deltaTime;

            Vector3 direction = target.position- transform.position ;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);

        if (fireRate <= 0)
        {
            fireRate = 0.2f;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(projectile, shootPoint.position, shootPoint.rotation);
    }

}