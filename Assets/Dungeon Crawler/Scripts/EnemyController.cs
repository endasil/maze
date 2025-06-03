using Assets.AScripts.Enums;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : DamagableObject
{
    public AIType TypeOfAI = AIType.GUARD;
    public AIState State = AIState.DEFAULT_STATE;
    protected NavMeshAgent navAgent;
    public float aggroRange = 100.0f;
    
    protected Player player;

    [SerializeField] private int attackPower;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float attackTimer = 0;
    [SerializeField] private Projectile projectile;
    protected Animator anim;
    private int ignoreLayer;
    [SerializeField] private float visibleRange = 40f;
    [SerializeField] private Vector3 navAgentDestination;
    [SerializeField] public float timeAsACorpse = 5;

    public bool findPlayerWithoutRangeOfSight = false;
    public bool alwaysVisible = false;
    [SerializeField] private bool hasBeenSeen = false;
    private readonly float maxTimeToWaitAtWaypoint = 6;
    public float timeLeftToWaitAtWaypoint = 0;

    private float distanceToPlayer;
    
    protected void Start()
    {
        //alwaysVisible = true;
        attackTimer = attackCooldown;
        navAgent = GetComponent<NavMeshAgent>();
        player = FindAnyObjectByType<Player>();
        anim = GetComponent<Animator>();
        ignoreLayer = ~(
            (1 << LayerMask.NameToLayer("Treasure")) |
            (1 << LayerMask.NameToLayer("Ignore Raycast")) |
            (1 << LayerMask.NameToLayer("Enemy")));

        InvokeRepeating("Tick", 0, 0.5f);
        
        if (!alwaysVisible)
            foreach (var r in rendererList)
                r.enabled = false;
    }

    
    // Update called twice per second. For things like raycasting that are a bit costly to do every frame with many enemies and
    // things that are less time sensitive.
    private void Tick()
    {
        if (dead)
            return;

        HandleRandomWalkerAI();
        
        // If this agent should find the player on the level regardless of distance and walls
        if (findPlayerWithoutRangeOfSight == true) navAgent.SetDestination(player.transform.position);

        var directionToPlayer = (player.transform.position - transform.position).normalized;
        if (PlayerIsVisible(directionToPlayer))
        {
            // Enable rendering of this enemy the first time the player sees it.
            if (!hasBeenSeen)
            {
                hasBeenSeen = true;
                foreach (var r in rendererList) r.enabled = true;
            }

            // When player either gets into aggro range or is already hunting the player, update target for navagent
            if ((State == AIState.HUNTING_PLAYER || distanceToPlayer <= aggroRange) && distanceToPlayer > navAgent.stoppingDistance + 1)
            {
                State = AIState.HUNTING_PLAYER;
                //Debug.Log("Player pos" + player.transform.position + " state for trying to set nav target: " + navAgent.SetDestination(player.transform.position));
                navAgent.SetDestination(player.transform.position);
            }
        }
    }

    protected void Update()
    {
        if (dead == true)
            return;

        anim.SetFloat("Speed_f", navAgent.velocity.magnitude);
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        attackTimer -= Time.deltaTime;
        timeLeftToWaitAtWaypoint -= Time.deltaTime;

        // Trigger attack animation when within melee range. The animation will have a
        // callback to AttackEvent at the right position in the animation where damage
        // will be applied if player within range.
        if (distanceToPlayer <= navAgent.stoppingDistance + .8 && attackTimer <= 0)
        {
            anim.SetTrigger("Attack_t");
            attackTimer = attackCooldown;
        }
    }
    
    // Triggered by animations
    public void AttackEvent()
    {
        if (dead == true)
            return;

        if (distanceToPlayer <= navAgent.stoppingDistance + 0.8)
        {
            FacePlayer();
            player.TakeDamage(attackPower);
        }
    }
    public override void Die()
    {
        if (deathSound) AudioSource.PlayClipAtPoint(deathSound, transform.position);

        dead = true;
        anim.SetFloat("Speed_f", 0);
        anim.SetTrigger("Die_t");
        Debug.Log("Dying..");
        Destroy(GetComponent<BoxCollider>());
        Destroy(navAgent);
        Destroy(gameObject, timeAsACorpse);
    }

    protected void FacePlayer()
    {
        var direction = (player.transform.position - transform.position).normalized;
        var lookDirection = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation =
            lookDirection; //= Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * 5.0f);
    }

    protected void FireProjectileTowardsPlayer(float height = 1.0f)
    {
        var start = transform.position;
        start.y = height;
        var direction = (player.gameObject.transform.position - start).normalized;

        var position = transform.position;
        position += direction * 1.1f;
        position.y = height;
        var clone = Instantiate(projectile, new Vector3(position.x, position.y, position.z),
            Quaternion.LookRotation(direction, Vector3.up));
        clone.ownerTag = tag;
    }
    private void HandleRandomWalkerAI()
    {
        if (TypeOfAI != AIType.RANDOM_WALKER)
            return;

        // A random walker lost track of the player, wait for a while and then resume random walk.
        if (State == AIState.HUNTING_PLAYER && navAgent.velocity.sqrMagnitude < 0.1f)
        {
            State = AIState.DEFAULT_STATE;
            timeLeftToWaitAtWaypoint = maxTimeToWaitAtWaypoint;
        }
        
        if (State == AIState.DEFAULT_STATE && 
            navAgent.velocity.sqrMagnitude < 0.1 && timeLeftToWaitAtWaypoint <= 0)
        {
            timeLeftToWaitAtWaypoint = Random.Range(1.0f, maxTimeToWaitAtWaypoint);
            var rp = Random.insideUnitCircle * 20;
            navAgent.destination = transform.position + new Vector3(rp.x, 0, rp.y);
        }

    }
    private bool PlayerIsVisible(Vector3 directionToPlayer)
    {
        
        return distanceToPlayer < visibleRange && Physics.Raycast(
                   new Ray(transform.position, directionToPlayer),
                   out var hitInfo, visibleRange, ignoreLayer) && hitInfo.collider.gameObject.tag == "Player";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        if (player)
        {
            var direction = (player.transform.position - transform.position).normalized;
            var ray = new Ray(transform.position + direction, direction);
            Physics.Raycast(ray, out var hitInfo, visibleRange, ignoreLayer);
            Gizmos.DrawLine(transform.position, hitInfo.point);
            //if (hitInfo.transform)
            //{
            //    Debug.Log($"Raycasting collides with ${hitInfo.transform.gameObject}");
            //}
        }


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, visibleRange);
        if (navAgent) Gizmos.DrawWireSphere(transform.position, navAgent.stoppingDistance);
    }
}