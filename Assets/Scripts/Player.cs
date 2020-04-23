using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player : DamagableObject
{

    public LayerMask clickableLayer;
    public GameObject projectile;
    public Light clickLight;
    private NavMeshAgent navAgent;
    public float attackCooldown = 1.0f;
    private float attackTimer = 0;
    public Interactable focus;
    public GameObject floor;
    public AudioClip projectileSound;
    public int Gold = 0;

    public Transform floorParent;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>();
        GameObject flooors = new GameObject();

        foreach (Transform item in floorParent)
        {
            float tmp = item.position.y;
            item.position.z = item.position.y;
            item.position.y = tmp;
        }
        //for (var z = -60; z < 60; z+=4)
        //{
        //    for (var x = -60; x < 60; x+=4)
        //    {
        //        Instantiate(floor, new Vector3(x, 0,z), Quaternion.identity, flooors.transform);
        //    }
        //}

    }

    // Update is called once per frame
    void Update()
    {
        if (navAgent.velocity.magnitude <= 0)
        {
            clickLight.transform.position = new Vector3(-200, 3, -200);
        }
        
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, 100, clickableLayer))
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(hitInfo.point);
                clickLight.transform.position = new Vector3(hitInfo.point.x, 3, hitInfo.point.z);
            }
        }

        attackTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(1) && attackTimer <= 0)
        {
            attackTimer = attackCooldown;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 100))
            {
                Vector3 relativePos = (hitInfo.point - transform.position).normalized;
                relativePos.y = 0.0f;
                AudioSource.PlayClipAtPoint(projectileSound, transform.position, 0.5f);
                Instantiate(projectile, new Vector3(transform.position.x, transform.position.y, transform.position.z),
                    Quaternion.LookRotation(relativePos, Vector3.up));
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, 100))
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    SetFocus(interactable);
                }
            }
        }
    }

    private void SetFocus(Interactable newFocus)
        {
            focus = newFocus;
        }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Ground")
        {
            
        }
    }

    public override void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        base.Die();
    }
}

