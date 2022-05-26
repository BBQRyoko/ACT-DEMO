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
        //获得释放者的位置信息，然后以那个为目标发射
        //飞行道具本身生成时检测是否有playerManager或者enemyManager
        //然后获取其的TargetPos位置
        //反射过的无法再触发反射

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
