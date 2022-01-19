using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkKnight_PullZone : MonoBehaviour
{
    [SerializeField] float pullingForce;
    [SerializeField] Transform pullingPos;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Vector3 dir = new Vector3(other.transform.position.x - pullingPos.position.x, other.transform.position.y - pullingPos.position.y, other.transform.position.z - pullingPos.position.z);
            dir.Normalize();
            other.GetComponent<Rigidbody>().AddForce(dir * pullingForce, ForceMode.VelocityChange);
        }
    }
}
