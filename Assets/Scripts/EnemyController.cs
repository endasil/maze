using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using  UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : DamagableObject
{
    public float detectPlayerRadius = 5.0f;
    public float continueChasePlayerRadius = 20f;
    private Player player;
    
    private NavMeshAgent navAgent;
    public float rotationSpeed = 5.0f;
    private Rigidbody playerRb;
    public bool chasing = false;
    public int attackPower = 1;
    public float attackCooldown = 1.0f;
    private float attackTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        attackTimer = attackCooldown;
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType< Player>();
        
        Animation anim = GetComponent<Animation>();
        anim.wrapMode = WrapMode.Loop;
    }
    



    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        
        if (distance <= detectPlayerRadius)
        {
            navAgent.SetDestination(player.transform.position);
        }

        attackTimer -= Time.deltaTime;
        if (distance <= navAgent.stoppingDistance)
        {
            if (attackTimer <= 0)
            {
                attackTimer = attackCooldown;
                Debug.Log($"{this.name} attacking player for {attackPower} damage.");
                FaceTarget();
                player.TakeDamage(attackPower);
            }
        }
    }

    
    private void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookDirection = Quaternion.LookRotation((new Vector3(direction.x, 0, direction.z)));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * rotationSpeed);
    }

    


    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, detectPlayerRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, navAgent.stoppingDistance);
    }



 
}