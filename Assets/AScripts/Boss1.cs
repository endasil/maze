using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : EnemyController
{

    enum CombatState
    {
        Summon,
        Teleport

    }
    
    [Header("Audio")]
    public AudioClip startSummonSound;
    public AudioClip endSummonSound;

    [Header("Other")]
    private CombatState combatState = CombatState.Summon;
    private float rotSpeed = 0.01f;
    private float nextActionTime = 0;
    private float timeBetweenTeleportations = 1f;
    private float timeToKeepSummoning = 13.0f;
    private GameObject summonSpell;
    private Transform summonHolder;
    private int skeletonsToSummon = 10;
    private readonly List<Vector3> summonPos = new List<Vector3>()
    {
        new Vector3(40, 0.5f, 27),
        new Vector3(-2, 0.5f, 27),
        new Vector3(-42, 0.5f, 27)

    };

    [SerializeField]
    private EnemyController enemyTypeToSpawn;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        summonSpell = transform.Find("SummonSpell").gameObject;
        summonHolder = GameObject.Find("SummonHolder").transform;
        player = FindObjectOfType<Player>();
        anim = GetComponentInChildren<Animator>();
        ActivateSummonState();
        nextActionTime = Time.time + 5;
        transform.position = summonPos[1];

    }

    public void TeleportToGrave()
    {
        int graveNr = Random.Range(0, 3);
        transform.position = summonPos[graveNr];
    }
    public void ActivateSummonState()
    {
        anim.SetBool("idle_combat", true);
        anim.SetBool("idle_normal", false);
        summonSpell.SetActive(true);
        nextActionTime = Time.time + timeToKeepSummoning;
        combatState = CombatState.Summon;
        AudioSource.PlayClipAtPoint(startSummonSound, transform.position,1.0f);

    }

    public void ActivateTeleportState()
    {
        anim.SetBool("idle_combat", false);
        anim.SetBool("idle_normal", true);
        summonSpell.SetActive(false);
        nextActionTime = Time.time + timeBetweenTeleportations;
        combatState = CombatState.Teleport;

    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time > nextActionTime )
        {
            
            if (combatState == CombatState.Teleport)
            {
                nextActionTime = Time.time + timeBetweenTeleportations;
                // If no enemies left, go to graveyard and start summon animation
                if (summonHolder.transform.childCount == 0)
                {
                    TeleportToGrave();
                    ActivateSummonState();
                }
                else
                {
                    StartCoroutine("Teleport");
                }
            }

            else if (combatState == CombatState.Summon)
            {

                // At the end of summon time, spawn the actual enemies
                Summon(skeletonsToSummon);
                ActivateTeleportState();
            }
        }
    }

    
    public void Summon(int nrOfSummons)
    {
        var position = transform.position;
        position += transform.forward * 9f;
        AudioSource.PlayClipAtPoint(endSummonSound, transform.position,1.0f);
        for (int skeletonNr = 0; skeletonNr < nrOfSummons; skeletonNr++)
        {
            EnemyController clone = Instantiate(enemyTypeToSpawn, new Vector3(position.x+skeletonNr, position.y, position.z),
                Quaternion.identity, summonHolder);
            clone.findPlayerWithoutRangeOfSight = true;
        }
    }

    IEnumerator Teleport()
    {
        int angle = Random.Range(0, 360);
        transform.position = player.gameObject.transform.position + Vector3.forward * 21;
        transform.RotateAround(player.transform.position, Vector3.up, angle);
        transform.position.Set(transform.position.x, 3, transform.position.z);
        transform.LookAt(player.gameObject.transform);
        yield return new WaitForSeconds(timeBetweenTeleportations / 4);
        FireProjectileTowardsPlayer(3);
    }


}
