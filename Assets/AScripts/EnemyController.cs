using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : DamagableObject
{
    public float aggroRange = 100.0f;
    protected Player player;

    protected NavMeshAgent navAgent;
    
    [SerializeField]
    private int attackPower;
    [SerializeField]
    private float attackCooldown = 1.0f;
    [SerializeField]
    private float attackTimer = 0;
    [SerializeField]
    private Projectile projectile;
    protected Animator anim;
    private int ignoreLayer;
    [SerializeField]
    private float visibleRange = 40f;
    [SerializeField]
    private Vector3 navAgentDestination;
    [SerializeField]
    public float timeAsACorpse = 5;
    
    public bool findPlayerWithoutRangeOfSight = false;
    public bool alwaysVisible = false;
    private bool hasBeenSeen = false;

    private float distanceToPlayer;
    // Start is called before the first frame update

    protected void Start()
    {
        attackTimer = attackCooldown;
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        anim = GetComponent<Animator>();
        ignoreLayer = ~(
            (1 << LayerMask.NameToLayer("Treasure")) |
            (1 << LayerMask.NameToLayer("Ignore Raycast")) |
            (1 << LayerMask.NameToLayer("Enemy")));
        
        InvokeRepeating("Tick", 0, 0.5f);
        if (!alwaysVisible)
        {
            foreach (var r in rendererList)
            {
                r.enabled = false;
            }
        }
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
    }


    // Update called twice per second. For things like raycasting that are a bit costly to do every frame with many enemies and
    // things that are less time sensitive.
    void Tick()
    {
        if(dead)
            return;

        // If this agent should find the player on the level regardless of distance and walls
        if (findPlayerWithoutRangeOfSight == true)
        {
            navAgent.SetDestination(player.transform.position);
        }

        var directionToPlayer = (player.transform.position - transform.position).normalized;
        if (distanceToPlayer < visibleRange && Physics.Raycast(new Ray(transform.position + directionToPlayer, directionToPlayer),
                out var hitInfo, visibleRange, ignoreLayer) && hitInfo.collider.gameObject.tag == "Player")
        {
            // Enable rendering of this enemy the first time the player sees it.
            if (!hasBeenSeen)
            {
                hasBeenSeen = true;
                foreach (var r in rendererList)
                {
                    r.enabled = true;
                }
            }

            // Not within melee range but the player can be seen by the enemy and within aggroRange 
            if (distanceToPlayer <= aggroRange && distanceToPlayer > navAgent.stoppingDistance + 1 )
            {
                anim.SetFloat("Speed_f", navAgent.velocity.magnitude);
                navAgent.SetDestination(player.transform.position);
            }

        }
    }


    // Update is called once per frame
    protected void Update()
    {
        if (dead == true)
            return;

        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= navAgent.stoppingDistance + 1 && attackTimer <= 0)
        {
            anim.SetTrigger("Attack_t");
        }

        if (distanceToPlayer <= navAgent.stoppingDistance + 0.3)
        {
            FacePlayer();
            if (attackTimer <= 0)
            {
                attackTimer = attackCooldown;
                //Debug.Log($"{this.name} attacking player for {attackPower} damage. attackTimer {attackTimer}");
                player.TakeDamage(attackPower);

            }
        }
    }
    
    protected void FacePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookDirection = Quaternion.LookRotation((new Vector3(direction.x, 0, direction.z)));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * 5.0f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        if (player)
        {
            var direction = (player.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position + direction, direction);
            Physics.Raycast(ray, out RaycastHit hitInfo, visibleRange, ignoreLayer);
            Gizmos.DrawLine(transform.position, hitInfo.point);
            //if (hitInfo.transform)
            //{
            //    Debug.Log($"Raycasting collides with ${hitInfo.transform.gameObject}");
            //}
        }


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, visibleRange);
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
        Destroy(gameObject, timeAsACorpse);

    }

}