using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoHazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DarkKnight")) 
        {
            Debug.Log("123");
            other.GetComponentInChildren<EnemyAnimatorManager>().tornadoSlashEnhance = true;
            Destroy(transform.gameObject);
        }
    }
}
