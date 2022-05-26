using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoHazard : MonoBehaviour
{
    FlyingObj reflectFlyingObj;
    [SerializeField] Transform shootPos;

    //可以反弹攻击
    void DefelectFlyingObj(FlyingObj obj) 
    {
    
    }

    //可以吸收火焰
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DarkKnight"))
        {
            Debug.Log("123");
            other.GetComponentInChildren<EnemyAnimatorManager>().tornadoSlashEnhance = true;
            Destroy(transform.gameObject);
        }

        if (other.GetComponentInParent<FlyingObj>()) 
        {
            DefelectFlyingObj(other.GetComponentInParent<FlyingObj>());
            Destroy(other.gameObject);
        }
    }
}
