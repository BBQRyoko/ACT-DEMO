using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Execution : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (gameObject.GetComponentInParent<EnemyManager>().canBeExecuted)
            {
                other.GetComponent<PlayerAttacker>().executionTarget = transform.GetComponentInParent<EnemyManager>();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerAttacker>().executionTarget = null;
        }
    }
}
