using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;



public class Player : DamagableObject
{
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Dying = Animator.StringToHash("dying");

    [Header("Stats")]
    [SerializeField]
    public int gold = 0;
    [SerializeField]
    public int Keys = 0;
    [SerializeField] 
    private int weaponLevel = 0;
    [SerializeField]
    private float attackCooldown = 1.0f;
    [Header("Audio")]
    public AudioClip projectileSound;
    public AudioClip noKey;
    public AudioClip noMoney;
    [Header("Other")] 
    [SerializeField]
    private Light clickLight;
    private Animator anim;
    
    private NavMeshAgent navAgent;
    [SerializeField]
    private LayerMask clickableLayer;
    [SerializeField]
    private List<Projectile> ProjectileTypes;
    [SerializeField]
    private float fowForwardOffsetMultiplier = 2f;
    [SerializeField]
    private float fowRevealRadius = 15;

    private float attackTimer = 0;

    new void Awake()
    {
        base.Awake();
        if (DataKeeper.instance)
        {
            DataKeeper.instance.LoadPlayer(this);
        }
    }
    // Start is called before the first frame update
    protected void Start()
    { 
        nextHitSoundTime = Time.time + hitSoundRepeatDelay;
        audioSource = GetComponent<AudioSource>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        InvokeRepeating(nameof(Tick), 0, 0.5f);
    }



    // ReSharper disable once UnusedMember.Local
    void ClearFoW(Vector3 revealCenter, float revealRadius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(revealCenter, revealRadius, 1 << LayerMask.NameToLayer("FOW"), QueryTriggerInteraction.Collide);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Debug.Log(hitColliders[i].gameObject);
            hitColliders[i].gameObject.SetActive(false);
            i++;
        }
    }
    
    void Tick()
    {
        if (dead) return;
        // Remove the light that shows where the player is going when the player has stopped.

        var dist = (navAgent.destination - transform.position).sqrMagnitude; 
        if (dist <= 2.0f)
        {
            clickLight.transform.position = new Vector3(-200, 3, -200);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dead) return;
            
        anim.SetFloat(Speed,navAgent.velocity.magnitude);

        // Remove fog of war slightly ahead of the player
        // ClearFOW(transform.position + transform.forward * fowForwardOffsetMultiplier, fowRevealRadius);
        attackTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0))
        {

            // Check if the user clicked somewhere the player can move to.
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hitInfo, 100, clickableLayer))
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
            if (Physics.Raycast(ray, out var hitInfo, 100, clickableLayer))
            {
                anim.SetTrigger(Attack);
                Vector3 direction = (hitInfo.point - transform.position).normalized;
                direction.y = 0.0f;
                var position = transform.position + direction * 1.1f;
                
                Quaternion lookDirection = Quaternion.LookRotation((new Vector3(direction.x, 0, direction.z)));
                transform.rotation = lookDirection;

                audioSource.PlayOneShot(projectileSound, 0.03f);
                GameObject clone = Instantiate(ProjectileTypes[weaponLevel].gameObject, new Vector3(position.x, position.y, position.z),
                    Quaternion.LookRotation(direction, Vector3.up));
                clone.SetActive(true);
            }
        }

    }


    public bool PayMoney(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        AudioSource.PlayClipAtPoint(noMoney, transform.position, 1f);
        return false;
    }

    public bool UseKey()
    {
        if (Keys > 0)
        {
            Keys--;
            return true;
        }

        AudioSource.PlayClipAtPoint(noKey, transform.position, 1f);

        return false;
    }

    

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * fowForwardOffsetMultiplier); 
        Gizmos.DrawWireSphere(transform.position + transform.forward * fowForwardOffsetMultiplier, fowRevealRadius);
    }


    public override void Die()
    {
        
        var dict = new Dictionary<string,object>
        {
            { "position", gameObject.transform.position },
            { "timeOnLevel", Time.timeSinceLevelLoad },
            { "timeSinceGameStart", Time.time },
            { "weaponLevel", weaponLevel }
        };
        dead = true;
        Destroy(navAgent);
        anim.SetTrigger(Dying);        
        AnalyticsEvent.GameOver(SceneManager.GetActiveScene().name, dict);
        if (deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }
        
        StartCoroutine(ReloadSceneAfterTime(4.5f));
    }


    private IEnumerator ReloadSceneAfterTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public override void TakeDamage(int damage)
    {
        if (dead) return;
            base.TakeDamage(damage);


        if (nextHitSoundTime < Time.time && hitSounds.Count > 0)
        {
            nextHitSoundTime = Time.time + hitSoundRepeatDelay;
            var soundToPlay = Random.Range(0, hitSounds.Count);
            audioSource.PlayOneShot(hitSounds[soundToPlay], 0.2f);
        }
    }


    public void SetGold(int amount)
    {
        gold = amount;
    }
    public bool GiveGold(int amount)
    {
        gold += amount;
        return true;
    }


    public int GetWeaponLevel()
    {
        return weaponLevel;
    }
    public bool UpgradedWeaponLevel(int level)
    {

        if (weaponLevel <= level)
        {
            weaponLevel = level;
            return true; 
        }

        return false;
    }


    public int GetGold()
    {
        return gold;
    }
}

