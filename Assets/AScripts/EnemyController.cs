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

    protected NavMeshAgent navAgent;
    public float facePlayerSpeed = 5.0f;
    public int attackPower = 1;
    public float attackCooldown = 1.0f;
    private float attackTimer = 0;
    public Projectile projectile;
    protected Animator anim;
    private int ignoreLayer;

    [SerializeField]
    private Vector3 navAgentDestination;
    public float timeAsACorpse = 5;
    // Test
    public bool findPlayerWithoutRangeOfSight = false;
    public bool alwaysVisible = false;
    public bool foundPlayer = false;
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
        //        ignoreLayer = ~(1 << LayerMask.NameToLayer("Enemy")); // ignore collisions with layerX


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
        else  // Not within combat range, 
        {
            anim.SetFloat("Speed_f", navAgent.velocity.magnitude);

            var direction = (player.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position + direction, direction);

            if (findPlayerWithoutRangeOfSight == true)
            {
                navAgent.SetDestination(player.transform.position);
            }
            else if (Physics.Raycast(ray, out RaycastHit hitInfo, detectPlayerRadius, ignoreLayer) && hitInfo.collider.gameObject.tag == "Player")
            {
                if (!foundPlayer)
                {
                    foundPlayer = true;
                    foreach (var r in rendererList)
                    {

                        r.enabled = true;
                        

                    }
                }

                bool okDesiDestination = navAgent.SetDestination(player.transform.position);
                Debug.Log(
                    $"Navmesh destination is object {hitInfo.transform.gameObject.name} result is {okDesiDestination} position of target is {navAgent.destination}");

            }
        }
    }



    protected void FacePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookDirection = Quaternion.LookRotation((new Vector3(direction.x, 0, direction.z)));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * facePlayerSpeed);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectPlayerRadius);
        if (player)
        {
            var direction = (player.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position + direction, direction);
            Physics.Raycast(ray, out RaycastHit hitInfo, detectPlayerRadius, ignoreLayer);
            Gizmos.DrawLine(transform.position, hitInfo.point);
            Debug.Log($"Raycasting collides with ${hitInfo.transform.gameObject}");
        }


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
        Destroy(gameObject, timeAsACorpse);

    }

}