using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSale : MonoBehaviour
{
    public int price = 0;
    public GameObject shopDisplayItem;
    private GameObject shopDisplayItemClone;

    public GameObject itemOnSale;

    public Vector3 displayItemOffset;
    public TextMeshPro statsText;
    [SerializeField]
    private AudioClip purchaseSound;

    // Start is called before the first frame update
    protected void Start()
    {
        shopDisplayItemClone = Instantiate(shopDisplayItem, transform.position + displayItemOffset, Quaternion.identity);
        var distance = shopDisplayItemClone.transform.position - transform.position;
        var collider = gameObject.GetComponent<BoxCollider>();
        collider.size = Vector3.Scale(shopDisplayItemClone.GetComponent<BoxCollider>().size, shopDisplayItem.transform.localScale);
        collider.center = Vector3.Scale(shopDisplayItemClone.GetComponent<BoxCollider>().center, shopDisplayItem.transform.localScale);
        collider.center += distance;
        InvokeRepeating("Tick", 0, 0.5f);

    }

    void Tick()
    {
        statsText.SetText($"X {price}");
        shopDisplayItemClone.transform.position = transform.position + displayItemOffset;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            var player = other.gameObject.GetComponent<Player>();
            if (player.PayMoney(price))
            {
                Instantiate(itemOnSale, transform.position + displayItemOffset, Quaternion.identity);
                Destroy(shopDisplayItemClone);
                Destroy(gameObject);
                if (purchaseSound)
                {
                    AudioSource.PlayClipAtPoint(purchaseSound, transform.position);
                }
            }
        }

    }
}
