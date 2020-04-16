using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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


    private Animator anim;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();

        anim = GetComponent<Animator>();
    }




    // Update is called once per frame
    void Update()
    {
        if (dead == true)
        {
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);
        Ray ray = new Ray(transform.position, player.transform.position - transform.position);
        

        anim.SetFloat("Speed_f", navAgent.velocity.magnitude);


        if (Physics.Raycast(ray, out var hitInfo, 100))
        {
            //if (distance <= detectPlayerRadius)
            if (hitInfo.collider.gameObject.tag == "Player")
            {
                Debug.Log($"{this.name} can see player");
                navAgent.SetDestination(player.transform.position);
            }
            Debug.Log($"{this.name} found a {hitInfo.collider.gameObject.tag}");
        }

        //Debug.Log("distance: " + distance);
        attackTimer -= Time.deltaTime;
        if (distance <= navAgent.stoppingDistance)
        {
            FaceTarget();
            if (attackTimer <= 0)
            {
                attackTimer = attackCooldown;
                Debug.Log($"{this.name} attacking player for {attackPower} damage.");
                
                anim.SetTrigger("Attack_t");

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

    public override void Die()
    {
        dead = true;
        anim.SetTrigger("Die_t");
        Debug.Log("Dying..");
        Destroy(GetComponent<BoxCollider>());
        navAgent.isStopped = true;
        StartCoroutine(DestroyObject(5));
    }

    private IEnumerator DestroyObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Destroying");
        //Destroy(gameObject);
    }

}