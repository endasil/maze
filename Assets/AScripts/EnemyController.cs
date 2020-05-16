using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : DamagableObject
{
    public float detectPlayerRadius = 100.0f;
    protected Player player;
    protected AudioClip deathSound;
    protected  NavMeshAgent navAgent;
    public float rotationSpeed = 5.0f;
    public int attackPower = 1;
    public float attackCooldown = 1.0f;
    private float attackTimer = 0;
    //protected Projectile
    [SerializeField]
    private LayerMask ignoredLayers;

    public Projectile projectile;
    private Animator anim;

    public bool findPlayerWithoutRangeOfSight = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        attackTimer = attackCooldown;
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
    }


    protected void FireProjectileTowardsPlayer(float height = 1.0f)
    {

        var start = transform.position;
        start.y = height;
        Vector3 direction = (player.gameObject.transform.position - start).normalized;
        
        var position = transform.position;
        position += direction * 1.1f;
        position.y = height;
        Projectile clone = Instantiate(projectile, new Vector3(position.x, position.y, position.z),
            Quaternion.LookRotation(direction, Vector3.up));
        clone.ownerTag = tag;
        Debug.Log($"ownertag {clone.ownerTag}");
    }

    // Update is called once per frame
    protected void Update()
    {
        if (dead == true)
        {
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);

        
        attackTimer -= Time.deltaTime;


        if (distance <= navAgent.stoppingDistance + 1 && attackTimer <= 0)
        {
            //anim.SetFloat("Speed_f", 0);
            anim.SetTrigger("Attack_t");
            //Debug.Log("Starting attack Anim" + gameObject.name);
        }

        if (distance <= navAgent.stoppingDistance + 0.3)
        {
            FacePlayer();
            if (attackTimer <= 0)
            {
                attackTimer = attackCooldown;
                Debug.Log($"{this.name} attacking player for {attackPower} damage. attackTimer {attackTimer}");

                // Set this to 0 to avoid going to run anim
                //anim.SetFloat("Speed_f", 0);
                //anim.SetTrigger("Attack_t");
                player.TakeDamage(attackPower);

            }
        }
        else // Not within combat range, 
        {
            anim.SetFloat("Speed_f", navAgent.velocity.magnitude);
            var direction = (player.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position+direction, direction);
            //Convert Layer Name to Layer Number
            int treasureLayer = LayerMask.NameToLayer("Treasure");
            int EnemyLayer = LayerMask.NameToLayer("Enemy");


            int ignoreLayer = ~((1 << treasureLayer) | (1 << EnemyLayer));

            
            if(findPlayerWithoutRangeOfSight == true || 
               Physics.Raycast(ray, out RaycastHit hitInfo, detectPlayerRadius, ignoreLayer) && hitInfo.collider.gameObject.tag == "Player")
            {
                navAgent.SetDestination(player.transform.position);
            }
        }
    }



    protected void FacePlayer()
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
        if (navAgent)
        {
            Gizmos.DrawWireSphere(transform.position, navAgent.stoppingDistance);
        }
    }

    public override void Die()
    {
        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        dead = true;
        anim.SetFloat("Speed_f", 0);
        anim.SetTrigger("Die_t");
        Debug.Log("Dying..");
        Destroy(GetComponent<BoxCollider>());
        Destroy(navAgent);
        Destroy(gameObject, 5);

    }

}