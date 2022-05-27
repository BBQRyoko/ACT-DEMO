using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoHazard : MonoBehaviour
{
    FlyingObj deflectFlyingObj;
    [SerializeField] Transform shootPos;
    ProjectileDamager curDamager;
    [SerializeField] float defaultCoverDuration = 1.5f;
    float coveredDuration;

    private void Awake()
    {
        curDamager = GetComponent<ProjectileDamager>();
    }
    private void Update()
    {
        if (curDamager.coveredPlayer) 
        {
            if (coveredDuration > 0)
            {
                coveredDuration -= Time.deltaTime;
            }
            else
            {
                curDamager.ProjectileDestroy();
            }
        }
    }
    //可以反弹攻击
    void DefelectFlyingObj(FlyingObj obj) 
    {
        //if (!deflectFlyingObj) 
        //{
        //    deflectFlyingObj = obj;
        //    var defelectObj = Instantiate(deflectFlyingObj, shootPos, false);
        //    defelectObj.transform.SetParent(null);
        //    defelectObj.gameObject.SetActive(true);
        //    defelectObj.StartFlyingObj(deflectFlyingObj.shooterPos, false, deflectFlyingObj.shooterPos, true);
        //    ProjectileDamager projectileDamager = defelectObj.GetComponentInChildren<ProjectileDamager>();
        //    if (projectileDamager.isPlayerDamage)
        //    {
        //        projectileDamager.isPlayerDamage = false;
        //    }
        //    else
        //    {
        //        projectileDamager.isPlayerDamage = true;
        //    }
        //    deflectFlyingObj = null;
        //}
       
        //获得释放者的位置信息，然后以那个为目标发射
        //飞行道具本身生成时检测是否有playerManager或者enemyManager
        //然后获取其的TargetPos位置
        //反射过的无法再触发反射
    }
    public void TornadoLifeCheck() 
    {
        coveredDuration += defaultCoverDuration;
    }
    //可以吸收火焰
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DarkKnight"))
        {
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
