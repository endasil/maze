using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {

        Debug.Log("Trigger enter door" + other.gameObject.tag);
        // Todo make player object pick up instead
        if (other.gameObject.tag == "Player")
        {
            //AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Debug.Log("Player found");

            var player = other.gameObject.GetComponent<Player>();
            if (player.Keys > 0)
            {
                Debug.Log("Player has keys");
                player.Keys--;
                leftDoor.Rotate(Vector3.up, 90);
                rightDoor.Rotate(Vector3.up, -90);
                Destroy(GetComponent<NavMeshObstacle>());
                Destroy(GetComponent<BoxCollider>());
            }
            
        }
    }
}
