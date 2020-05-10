using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullPlatform : MonoBehaviour
{
    [SerializeField]
    private int torchesToLit = 3;
    [SerializeField]
    private int torchesLit = 0;
    [SerializeField]
    private AudioClip activationSound;
    [SerializeField]
    private List<GameObject> objectsToActivate;

    public GameObject exitStone;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTorchLit(GameObject other)
    {
        torchesLit++;
        Debug.Log(torchesLit);
        if (torchesLit == torchesToLit)
        {

            foreach (var o in objectsToActivate)
            {
                o.SetActive(true);
            }
            AudioSource.PlayClipAtPoint(activationSound, exitStone.transform.position);
            exitStone.SetActive(false);
        }
    }
}
