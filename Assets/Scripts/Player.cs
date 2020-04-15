using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Player : DamagableObject
{

    public LayerMask clickableLayer;
    public GameObject projectile;
    private NavMeshAgent navAgent;

    public Interactable focus;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, 100, clickableLayer))
            {
                navAgent.SetDestination(hitInfo.point);
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, 100))
            {
                Vector3 relativePos = (hitInfo.point - transform.position).normalized;
                relativePos.y = 0.0f;
                Instantiate(projectile, new Vector3(transform.position.x, 0.5f, transform.position.z),
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

    

}

