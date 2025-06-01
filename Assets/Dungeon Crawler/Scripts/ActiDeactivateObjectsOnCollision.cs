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
    [SerializeField]
    private OnActivatedEvent notifyWhenActivated;
    [SerializeField]
    private bool activateOnProjectile = true;
    [SerializeField]
    private bool activated = false;
    [SerializeField]
    private AudioClip activationSound;

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
                Debug.Log($"activated {o.name}");
            }

            foreach (var o in objectsToDeactivate)
            {
                o.SetActive(false);
                Debug.Log($"deactivated {o.name}");
            }

            notifyWhenActivated.Invoke(this.gameObject);

            if (activationSound)
            {
                AudioSource.PlayClipAtPoint(activationSound, gameObject.transform.position);
            }
        }
    }

}
