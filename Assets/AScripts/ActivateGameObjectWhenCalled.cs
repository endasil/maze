using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ActivateGameObjectWhenCalled : MonoBehaviour
{
    [SerializeField]
    private int callsBeforeActivate =1;

    public AudioClip activationSound;
    [SerializeField]
    private int callsMade = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ObjectActivated(GameObject other)
    {

        callsMade++;
        Debug.Log("Method called!!!!");
        if (callsBeforeActivate < callsMade)
            return;

        gameObject.SetActive(true);
        if (activationSound)
        {
            AudioSource.PlayClipAtPoint(activationSound, transform.position);
        }
    }


}
