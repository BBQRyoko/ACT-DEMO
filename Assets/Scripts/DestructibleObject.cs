using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject itemContained;

    public GameObject brokenPieces;
    public void ObjectDestroy() 
    {
        Instantiate(brokenPieces, transform.position, transform.rotation);
        Instantiate(itemContained, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

}
