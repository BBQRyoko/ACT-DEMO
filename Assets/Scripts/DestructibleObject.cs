using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject brokenPieces;
    [SerializeField] GameObject[] containedItem;
    [SerializeField] bool containUpdraft;
    [SerializeField] GameObject updraft;
    public void ObjectDestroy() 
    {
        foreach (GameObject curItem in containedItem)
        {
            if (!containUpdraft)
            {
                GameObject item = Instantiate(curItem, transform.position, transform.rotation);
                ItemPickUp pickUp = item.GetComponentInChildren<ItemPickUp>();
                if (!pickUp)
                {
                    float angel = Random.Range(-15f, 15f);
                    item.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.down * 4f;
                }
                else 
                {
                    if (!item.GetComponentInChildren<ItemPickUp>().isBuff)
                    {
                        item.transform.eulerAngles = Quaternion.Euler(-180, 0, 0) * transform.up;
                        float angel = Random.Range(-15f, 15f);
                        item.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.down * 4f;
                    }
                    else 
                    {
                        float angel = Random.Range(-15f, 15f);
                        item.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.down * 4f;
                    }
                }
            }
            else 
            {
                Instantiate(updraft, transform.position, transform.rotation);
            }
        }
        Instantiate(brokenPieces, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
