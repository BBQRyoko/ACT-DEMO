using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoFieldStunTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            other.GetComponent<PlayerManager>().isStunned = true;
            other.GetComponent<PlayerManager>().stunTimer = -3f;
            this.gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
