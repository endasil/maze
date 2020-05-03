using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Player : DamagableObject
{

    public LayerMask clickableLayer;
    public GameObject projectile;
    public Light clickLight;
    private NavMeshAgent navAgent;
    public float attackCooldown = 1.0f;
    private float attackTimer = 0;
    public Interactable focus;
    public GameObject floor;
    public AudioClip projectileSound;
    public AudioClip noKey;
    public AudioClip noMoney;
    public int gold = 0;
    public Transform floorParent;
    public Transform wallToRepeat;

    public Transform wallParent;

    public int Keys = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>();
        GameObject flooors = new GameObject();


        //        DrawLvl2Walls();


        //for (var x = -60; x < 64; x += 4)
        //{
        //    Instantiate(wallToRepeat, new Vector3(x, 0, 2), Quaternion.identity, wallParent);
        //    Instantiate(wallToRepeat, new Vector3(x, 0, -120), Quaternion.identity, wallParent);
        //    Instantiate(wallToRepeat, new Vector3(60, 0, -60 + x), Quaternion.identity, wallParent);
        //    Instantiate(wallToRepeat, new Vector3(-60, 0, -60 + x), Quaternion.identity, wallParent);
        //}
        //foreach (Transform item in floorParent)
        //{
        //    //item.rotation.Set(item.rotation.x, item.rotation.y, item.rotation.z);
        //    item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, item.transform.position.z);
        //}
        //for (var z = -60; z < 60; z += 4)
        //{
        //    for (var x = -60; x < 60; x += 4)
        //    {
        //        Instantiate(floor, new Vector3(x, 0, z), Quaternion.identity, flooors.transform);
        //    }
        //}

    }



    // Update is called once per frame
    void Update()
    {
        if (navAgent.velocity.magnitude <= 0)
        {
            clickLight.transform.position = new Vector3(-200, 3, -200);
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, 100, clickableLayer))
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(hitInfo.point);
                clickLight.transform.position = new Vector3(hitInfo.point.x, 3, hitInfo.point.z);
            }
        }

        attackTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(1) && attackTimer <= 0)
        {
            attackTimer = attackCooldown;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 100))
            {
                Vector3 direction = (hitInfo.point - transform.position).normalized;
                direction.y = 0.0f;
                var position = transform.position;
                position += direction * 1.1f;
                Quaternion lookDirection = Quaternion.LookRotation((new Vector3(direction.x, 0, direction.z)));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * 80);

                AudioSource.PlayClipAtPoint(projectileSound, transform.position, 0.02f);
                GameObject clone = Instantiate(projectile, new Vector3(position.x, position.y, position.z),
                    Quaternion.LookRotation(direction, Vector3.up)) as GameObject;
                clone.SetActive(true);
                ;
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

    private void SetFocus(Interactable newFocus)
    {
        focus = newFocus;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Ground")
        {

        }
    }

    public override void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        base.Die();
    }

    private void DrawLvl2Walls()
    {
        // Spawn 33 north 
        const int WALL_SIZE = 4;
        for (var wallNr = 0; wallNr < 33; wallNr++)
        {
            (Instantiate(wallToRepeat, new Vector3(wallNr * WALL_SIZE, 0, 0), Quaternion.identity, wallParent)).gameObject
                .name = $"{wallToRepeat.gameObject.name}-NORTH";
        }


        // Spawn 33 south

        for (var wallNr = 0; wallNr < 33; wallNr++)
        {
            (Instantiate(wallToRepeat, new Vector3(wallNr * WALL_SIZE, 0, 30 * 4), Quaternion.identity, wallParent))
                .gameObject.name = $"{wallToRepeat.gameObject.name}-SOUTH";
        }

        // West
        for (var wallNr = 1; wallNr < 30; wallNr++)
        {
            (Instantiate(wallToRepeat, new Vector3(0, 0, wallNr * WALL_SIZE), Quaternion.identity, wallParent)).gameObject
                .name = $"{wallToRepeat.gameObject.name}-WEST";
        }

        // East
        for (var wallNr = 1; wallNr < 30; wallNr++)
        {
            (Instantiate(wallToRepeat, new Vector3(32 * 4, 0, wallNr * WALL_SIZE), Quaternion.identity, wallParent))
                .gameObject.name = $"{wallToRepeat.gameObject.name}-EAST";
        }


        int rowNr = 1;
        // ROW 1
        (Instantiate(wallToRepeat, new Vector3(7 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";


        // ROW 2
        rowNr = 2;
        (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R3 
        rowNr = 3;
        (Instantiate(wallToRepeat, new Vector3(5 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(9 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(29 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(30 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(31 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R4
        rowNr = 4;
        (Instantiate(wallToRepeat, new Vector3(4 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(8 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R5
        rowNr = 5;
        (Instantiate(wallToRepeat, new Vector3(3 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(7 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(11 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(14 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(15 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(18 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(19 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";


        (Instantiate(wallToRepeat, new Vector3(22 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(23 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(24 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(24 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(26 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(27 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R6
        rowNr = 6;
        (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(19 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R7
        rowNr = 7;
        (Instantiate(wallToRepeat, new Vector3(1 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(5 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(09 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(29 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(31 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R8
        rowNr = 8;
        (Instantiate(wallToRepeat, new Vector3(08 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        // R9
        rowNr = 9;
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(07 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        for (int x = 11; x < 25; x++)
        {
            (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
                wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        }

        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R10
        rowNr = 10;
        (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(11 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        // R11
        rowNr = 11;
        (Instantiate(wallToRepeat, new Vector3(1 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(2 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        (Instantiate(wallToRepeat, new Vector3(4 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(5 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R12
        rowNr = 12;
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R13
        rowNr = 13;
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R14
        rowNr = 14;
        (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        for (int x = 13; x < 19; x++)
        {
            (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
                wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        }

        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(26 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(27 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R15
        rowNr = 15;
        (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R16
        rowNr = 16;
        for (int x = 6; x < 11; x++)
        {
            (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
                wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        }

        (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R17
        rowNr = 17;
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R18
        rowNr = 18;
        (Instantiate(wallToRepeat, new Vector3(02 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(04 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";


        // R19
        rowNr = 19;
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        for (int x = 13; x < 25; x++)
        {
            (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
                wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        }

        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R20
        rowNr = 20;
        (Instantiate(wallToRepeat, new Vector3(03 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R21
        rowNr = 21;
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R22
        rowNr = 22;
        (Instantiate(wallToRepeat, new Vector3(01 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(05 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R23
        rowNr = 23;
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        for (int x = 10; x < 28; x++)
        {
            (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
                wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        }

        // R24
        rowNr = 24;
        (Instantiate(wallToRepeat, new Vector3(03 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R25
        rowNr = 25;
        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

        // R26
        rowNr = 26;

        for (int x = 06; x < 18; x++)
        {
            (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
                wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        }

        for (int x = 20; x < 31; x++)
        {
            (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
                wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        }


        (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
        (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
    }
}

