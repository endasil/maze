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
        if (other.gameObject.CompareTag("Player"))
        {

            if (openSound)
            {
                AudioSource.PlayClipAtPoint(openSound, transform.position);
            }

            var player = other.gameObject.GetComponent<Player>();
            if (player.UseKey())
            {
                leftDoor.Rotate(Vector3.up, 90);
                rightDoor.Rotate(Vector3.up, -90);
                Destroy(GetComponent<NavMeshObstacle>());
                Destroy(GetComponent<BoxCollider>());
            }
            
        }
    }
}
