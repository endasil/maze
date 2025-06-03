using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GameObject;


public class GameManager : Singleton<GameManager>
{

    public float updateInterval = 0.5F;

    private float timeAccumulated = 0; // FPS accumulated over the interval
    private int framesDrawn = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    private TextMeshProUGUI statsText;
    public int dummy = 20;
    private Player player;
    [SerializeField] 
    private DamagableObject boss;
    private Slider healthBar;
    private Slider bossHealthBar;
    public int FogOfWarMinZValue = -56;
    public int FogOfWarMaxZValue = 56;
    public int FogOfWarMinXValue = -60;
    public int FogOfWarMaxXValue = 56;
    private bool showFPS = true;
    [Header("Used when generating levels")]
    public Transform floorParent;
    public Transform wallToRepeat;
    public Transform wallParent;
    public GameObject floor;
    private float fps;


    public void CreateFogOfWar(int xMin, int xMax, int zMin, int zMax)
    {

        GameObject fowParent = new GameObject();
        for (int z = zMin; z < zMax; z += 2)
        {
            for (int x = xMin; x < xMax; x += 2)
            {
                GameObject cube = CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetParent(fowParent.gameObject.transform);
                cube.gameObject.GetComponent<Renderer>().receiveShadows = false;
                cube.gameObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
                cube.transform.position = new Vector3(x, 5.0f, z);
                cube.transform.localScale = new Vector3(2, 0.1f, 2);
                cube.layer = LayerMask.NameToLayer("FOW");
                cube.name = "FOW";
                cube.GetComponent<Renderer>().material.color = Color.black;

            }
        }

    }

    void Awake()
    {
        //CreateFogOfWar(FogOfWarMinXValue, FogOfWarMaxXValue, FogOfWarMinZValue, FogOfWarMaxZValue);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!boss)
        {
           boss = FindGameObjectsWithTag("Boss").FirstOrDefault()?.GetComponent<DamagableObject>();
        }
        player = FindAnyObjectByType<Player>();
        statsText = GetComponentInChildren<TextMeshProUGUI>();
        Transform canavas = GetComponentInChildren<Canvas>().transform;
        healthBar = canavas.Find("PlayerHealthBar").gameObject.GetComponentInChildren<Slider>();
        healthBar.maxValue = player.maxHealth;
        if (boss)
        {
            bossHealthBar = canavas.Find("BossHealthBar").gameObject.GetComponentInChildren<Slider>();
            bossHealthBar.maxValue = boss.maxHealth;
            bossHealthBar.gameObject.SetActive(true);
        }
        InvokeRepeating("Tick", 0, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            showFPS = !showFPS;
        }

        if (showFPS)
        {
            timeleft -= Time.deltaTime;
            timeAccumulated += Time.timeScale / Time.deltaTime;
            ++framesDrawn;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                fps = timeAccumulated / framesDrawn;
                timeleft = updateInterval;
                timeAccumulated = 0.0F;
                framesDrawn = 0;
            }
        }
    }

    // Called twice per second with invoke repeating
    void Tick()
    {

        statsText.SetText($"{player.Keys} Gold: {player.gold}  {(showFPS ? $@"{fps:F1} FPS" : "")} ");
        healthBar.value = (float)player.hp;


        if (bossHealthBar)
        {
            bossHealthBar.value = (float)boss.hp;
            if (boss.hp <= 0)
            {
                Destroy(bossHealthBar.gameObject);
            }

        }
    }
}



//DrawLvl2Walls();


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

//}

//private void DrawLevel1Walls()
//{
////for (var x = -60; x < 64; x += 4)
////{
////    Instantiate(wallToRepeat, new Vector3(x, 0, 2), Quaternion.identity, wallParent);
////    Instantiate(wallToRepeat, new Vector3(x, 0, -120), Quaternion.identity, wallParent);
////    Instantiate(wallToRepeat, new Vector3(60, 0, -60 + x), Quaternion.identity, wallParent);
////    Instantiate(wallToRepeat, new Vector3(-60, 0, -60 + x), Quaternion.identity, wallParent);
////}
//}


//private void DrawLvl2Walls()
//{
//    // Spawn 33 north 
//    const int WALL_SIZE = 4;
//    for (var wallNr = 0; wallNr < 31; wallNr++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(-60 + wallNr * WALL_SIZE, 0, 52), Quaternion.Euler(0, 90, 0), wallParent)).gameObject
//            .name = $"{wallToRepeat.gameObject.name}-NORTH";
//    }


//    // Spawn 33 south

//    for (var wallNr = 0; wallNr < 31; wallNr++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(-60 + wallNr * WALL_SIZE, 0, -60), Quaternion.Euler(0, 90, 0), wallParent))
//            .gameObject.name = $"{wallToRepeat.gameObject.name}-SOUTH270";
//    }

//    // West
//    for (var wallNr = 0; wallNr < 28; wallNr++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(-62, 0, -58 + wallNr * WALL_SIZE), Quaternion.Euler(0, 180, 0), wallParent)).gameObject
//            .name = $"{wallToRepeat.gameObject.name}-WEST";
//    }

//    // East
//    for (var wallNr = 0; wallNr < 28; wallNr++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(62, 0, -58 + wallNr * WALL_SIZE), Quaternion.Euler(0, 180, 0), wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-EAST";
//    }

//    return;

//    int rowNr = 1;
//    // ROW 1
//    (Instantiate(wallToRepeat, new Vector3(7 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";


//    // ROW 2
//    rowNr = 2;
//    (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R3 
//    rowNr = 3;
//    (Instantiate(wallToRepeat, new Vector3(5 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(9 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(29 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(30 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(31 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R4
//    rowNr = 4;
//    (Instantiate(wallToRepeat, new Vector3(4 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(8 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R5
//    rowNr = 5;
//    (Instantiate(wallToRepeat, new Vector3(3 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(7 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(11 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(12 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(14 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(15 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(18 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(19 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";


//    (Instantiate(wallToRepeat, new Vector3(22 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(23 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(24 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(24 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(26 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(27 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R6
//    rowNr = 6;
//    (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(19 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R7
//    rowNr = 7;
//    (Instantiate(wallToRepeat, new Vector3(1 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(5 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(09 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(29 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(31 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R8
//    rowNr = 8;
//    (Instantiate(wallToRepeat, new Vector3(08 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    // R9
//    rowNr = 9;
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(07 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    for (int x = 11; x < 25; x++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    }

//(Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//    wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R10
//    rowNr = 10;
//    (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(11 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    // R11
//    rowNr = 11;
//    (Instantiate(wallToRepeat, new Vector3(1 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(2 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    (Instantiate(wallToRepeat, new Vector3(4 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(5 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R12
//    rowNr = 12;
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R13
//    rowNr = 13;
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(16 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R14
//    rowNr = 14;
//    (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    for (int x = 13; x < 19; x++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    }

//(Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//    wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(26 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(27 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R15
//    rowNr = 15;
//    (Instantiate(wallToRepeat, new Vector3(6 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R16
//    rowNr = 16;
//    for (int x = 6; x < 11; x++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    }

//(Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//    wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R17
//    rowNr = 17;
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R18
//    rowNr = 18;
//    (Instantiate(wallToRepeat, new Vector3(02 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(04 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(13 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(25 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";


//    // R19
//    rowNr = 19;
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    for (int x = 13; x < 25; x++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    }

//(Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//    wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R20
//    rowNr = 20;
//    (Instantiate(wallToRepeat, new Vector3(03 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R21
//    rowNr = 21;
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(28 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R22
//    rowNr = 22;
//    (Instantiate(wallToRepeat, new Vector3(01 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(05 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(17 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R23
//    rowNr = 23;
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    for (int x = 10; x < 28; x++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    }

//    // R24
//    rowNr = 24;
//    (Instantiate(wallToRepeat, new Vector3(03 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(10 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R25
//    rowNr = 25;
//    (Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";

//    // R26
//    rowNr = 26;

//    for (int x = 06; x < 18; x++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    }

//    for (int x = 20; x < 31; x++)
//    {
//        (Instantiate(wallToRepeat, new Vector3(x * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//            wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    }


//(Instantiate(wallToRepeat, new Vector3(06 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//    wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//    (Instantiate(wallToRepeat, new Vector3(20 * WALL_SIZE, 0 * WALL_SIZE, rowNr * WALL_SIZE), Quaternion.identity,
//        wallParent)).gameObject.name = $"{wallToRepeat.gameObject.name}-R{rowNr}";
//}
