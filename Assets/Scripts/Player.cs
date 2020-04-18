using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Player : DamagableObject
{

    public LayerMask clickableLayer;
    public GameObject projectile;
    public Light clickLight;
    private NavMeshAgent navAgent;
    public float attackCooldown = 1.0f;
    private float attackTimer = 0;
    public Interactable focus;

    public int Gold = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (navAgent.velocity.magnitude <= 0)
        {
            clickLight.transform.position = new Vector3(-200, 3, -200);
        }
        attackTimer -= Time.deltaTime;
        //float distance = Vector3.Distance(player.transform.position, transform.position);
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

        if (Input.GetMouseButtonDown(1) && attackTimer <= 0)
        {
            attackTimer = attackCooldown;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //navAgent.isStopped = true;
            
            if (Physics.Raycast(ray, out var hitInfo, 100))
            {
                Vector3 relativePos = (hitInfo.point - transform.position).normalized;
                relativePos.y = 0.0f;
                Instantiate(projectile, new Vector3(transform.position.x, 1f, transform.position.z),
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
            Debug.Log(other.gameObject.name);
        }
    }

    public override void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        base.Die();
    }
}

