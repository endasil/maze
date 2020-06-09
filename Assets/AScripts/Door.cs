using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Transform leftDoor;
    [SerializeField]
    private Transform rightDoor;
    [SerializeField]
    private AudioClip openSound;
    
    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {

            if (openSound)
            {
                AudioSource.PlayClipAtPoint(openSound, transform.position);
            }

            Debug.Log("Player found");

            var player = other.gameObject.GetComponent<Player>();
            if (player.UseKey())
            {
                Debug.Log("Player has keys");
                
                leftDoor.Rotate(Vector3.up, 90);
                rightDoor.Rotate(Vector3.up, -90);
                Destroy(GetComponent<NavMeshObstacle>());
                Destroy(GetComponent<BoxCollider>());
            }
            
        }
    }
}
