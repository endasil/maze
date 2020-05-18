using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnActivatedEvent : UnityEvent<GameObject>
{
}


public class ActiDeactivateObjectsOnCollision : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> objectsToActivate;
    [SerializeField]
    protected List<GameObject> objectsToDeactivate;
    public OnActivatedEvent notifyWhenActivated;
    public bool activateOnProjectile = true;
    public bool activated = false;
    public AudioClip activationSound;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if (other.gameObject.tag == "Player" || (other.gameObject.tag == "PlayerProjectile" && activateOnProjectile))
        {
            activated = true;

            foreach (var o in objectsToActivate)
            {
                o.SetActive(true);
            }

            foreach (var o in objectsToDeactivate)
            {
                o.SetActive(false);
            }

            notifyWhenActivated.Invoke(this.gameObject);

            if (activationSound)
            {
                AudioSource.PlayClipAtPoint(activationSound, gameObject.transform.position);
            }
        }
    }

}
