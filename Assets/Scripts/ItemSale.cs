using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSale : MonoBehaviour
{
    public int price = 0;

    public GameObject shopDisplayItem;
    public GameObject itemOnSale;

    public Vector3 displayItemOffset;
    public TextMeshPro statsText;

    // Start is called before the first frame update
    void Start()
    {
        shopDisplayItem = Instantiate(shopDisplayItem, transform.position + displayItemOffset, Quaternion.identity);
        var distance = shopDisplayItem.transform.position - transform.position;
        var collider = gameObject.GetComponent<BoxCollider>();
        collider.size = Vector3.Scale(shopDisplayItem.GetComponent<BoxCollider>().size, shopDisplayItem.transform.localScale);
        collider.center = Vector3.Scale(shopDisplayItem.GetComponent<BoxCollider>().center, shopDisplayItem.transform.localScale);
        collider.center += distance;

    }

    // Update is called once per frame
    void Update()
    {
        statsText.SetText($"X {price}");
        shopDisplayItem.transform.position = transform.position + displayItemOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            var player = other.gameObject.GetComponent<Player>();
            if (player.Gold != price)
            {
                player.Gold -= price;
                Instantiate(itemOnSale, transform.position + displayItemOffset, Quaternion.identity);
                Destroy(shopDisplayItem);
                Destroy(gameObject);
            }
        }

    }
}
