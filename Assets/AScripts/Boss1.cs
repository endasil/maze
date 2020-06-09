using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss1 : EnemyController
{

    enum CombatState
    {
        Summon,
        Teleport

    }
    
    [Header("Audio")]
    [SerializeField]
    public AudioClip startSummonSound;
    [SerializeField]
    public AudioClip endSummonSound;

    [Header("Other")]
    private CombatState combatState = CombatState.Summon;
    private float nextActionTime = 0;
    private float timeBetweenTeleportations = 1.1f;
    private float timeToKeepSummoning = 12.0f;
    private GameObject summonSpell;
    [SerializeField]
    private Transform summonHolder;
    [SerializeField]
    private int skeletonsToSummon = 8;
    [SerializeField]
    private OnActivatedEvent callWhenDead;
    private readonly List<Vector3> summonPos = new List<Vector3>()
    {
        new Vector3(35, 0.5f, 23),
        new Vector3(-2, 0.5f, 23),
        new Vector3(-37, 0.5f, 23)

    };

    Vector3 minPositions = new Vector3(-40, 0.5f, 1);
    Vector3 maxPositions = new Vector3(60, 0.5f, 23);

    [SerializeField]
    private EnemyController enemyTypeToSpawn;
    // Start is called before the first frame update
    new void Start()
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
        Debug.Log($"Grave teleport to {transform.position}");
    }
    public void ActivateSummonState()
    {
        anim.SetBool("idle_combat", true);
        anim.SetBool("idle_normal", false);
        summonSpell.SetActive(true);
        nextActionTime = Time.time + timeToKeepSummoning;
        combatState = CombatState.Summon;
        AudioSource.PlayClipAtPoint(startSummonSound, transform.position,1.0f);
        transform.forward = Vector3.forward;
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
    new void Update()
    {
        if(dead)
            return;

        if (transform.position.z > maxPositions.z || transform.position.z < minPositions.z || transform.position.x > maxPositions.x|| transform.position.x < minPositions.x)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minPositions.x, maxPositions.z), transform.position.y, Mathf.Clamp(transform.position.z, minPositions.z, maxPositions.z));
            transform.LookAt(player.gameObject.transform);
            //Debug.Log("Clamp");

        }
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
            // TODO: Fix pooling instead if instantiating new ones.
            EnemyController clone = Instantiate(enemyTypeToSpawn, new Vector3(position.x+skeletonNr, position.y, position.z),
                Quaternion.identity, summonHolder);
            clone.timeAsACorpse = 1.5f;
            clone.alwaysVisible = true;
            clone.findPlayerWithoutRangeOfSight = true;
        }
    }

    public override void Die()
    {
        callWhenDead.Invoke(this.gameObject);
        summonSpell.SetActive(false);
        base.Die();
    }

    IEnumerator Teleport()
    {
        int angle = Random.Range(0, 360);
        transform.position = player.gameObject.transform.position + Vector3.forward * 21;
        transform.RotateAround(player.transform.position, Vector3.up, angle);
        transform.position.Set(transform.position.x, 3, transform.position.z);
        //Debug.Log($"Rotational teleport to {transform.position}");
        transform.LookAt(player.gameObject.transform);
        yield return new WaitForSeconds(timeBetweenTeleportations / 4);
        FireProjectileTowardsPlayer(3);
    }


}
