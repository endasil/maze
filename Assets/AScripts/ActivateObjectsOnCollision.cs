using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnActivatedEvent : UnityEvent<GameObject>
{
}


public class ActivateObjectsOnCollision : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> objectsToActivate;
    public OnActivatedEvent notifyWhenActivated;

    public bool activated = false;
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

        if (other.gameObject.tag == "Player" || other.gameObject.tag == "PlayerProjectile")
        {
            activated = true;

            foreach (var o in objectsToActivate)
            {
                o.SetActive(true);
            }

            notifyWhenActivated.Invoke(this.gameObject);
        }
    }

}
