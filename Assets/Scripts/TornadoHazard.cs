using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoHazard : MonoBehaviour
{
    FlyingObj deflectFlyingObj;
    [SerializeField] Transform shootPos;
    [SerializeField] ProjectileDamager curDamager;
    [SerializeField] float defaultCoverDuration = 3f;
    float characterCoveredDuration;

    [Header("火旋风相关")]
    [SerializeField] bool isFireTornado;
    [SerializeField] FlyingObj fireTornado;
    [SerializeField] GameObject explosionPrefab;

    private void Awake()
    {
        if(!isFireTornado) curDamager = GetComponent<ProjectileDamager>();
    }
    private void Update()
    {
        if (!isFireTornado) 
        {
            if (curDamager.coveredPlayer)
            {
                if (characterCoveredDuration > 0)
                {
                    characterCoveredDuration -= Time.deltaTime;
                }
                else
                {
                    curDamager.ProjectileDestroy();
                }
            }
        }
    }
    public void TornadoLifeCheck()
    {
        characterCoveredDuration += defaultCoverDuration;
    }
    void DefelectFlyingObj(FlyingObj obj) 
    {
        if (!deflectFlyingObj && obj.GetComponentInChildren<ProjectileDamager>() && (!obj.GetComponentInParent<PlayerManager>() || !obj.GetComponentInParent<EnemyManager>())) 
        {
            deflectFlyingObj = obj;
            Debug.Log(deflectFlyingObj.gameObject.name);
            var defelectObj = Instantiate(deflectFlyingObj, shootPos, false);
            defelectObj.transform.SetParent(null);
            defelectObj.gameObject.SetActive(true);
            defelectObj.StartFlyingObj(deflectFlyingObj.shooterPos, false, deflectFlyingObj.shooterPos, true);
            ProjectileDamager projectileDamager = defelectObj.GetComponentInChildren<ProjectileDamager>();
            if(projectileDamager.curProjectilType == ProjectileDamager.ProjectilType.regular) defelectObj.GetComponent<Rigidbody>().useGravity = false;
            defelectObj.m_LifeTime += 1.2f;
            if (projectileDamager.isPlayerDamage)
            {
                projectileDamager.isPlayerDamage = false;
            }
            else
            {
                projectileDamager.isPlayerDamage = true;
            }
            deflectFlyingObj = null;
        }
    }
    void GenerateFireTornado() 
    {
        ProjectileDamager projectileDamager = GetComponent<ProjectileDamager>();
        if (projectileDamager.coveredPlayer)
        {
            CharacterManager characterManager = null;
            if (projectileDamager.coveredPlayer.CompareTag("Player"))
            {
                characterManager = projectileDamager.coveredPlayer.GetComponent<CharacterManager>();
            }
            else
            {
                characterManager = projectileDamager.coveredPlayer.GetComponentInChildren<CharacterManager>();
            }
            characterManager.isToronadoCovered = false;
            projectileDamager.coveredPlayer.parent = null;
            projectileDamager.coveredPlayer = null;
        }
        var obj = Instantiate(fireTornado, transform, false);
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        obj.StartFlyingObj(GetComponent<FlyingObj>().m_TraceTarget, true, null, true);
        Destroy(this.gameObject);
    }
    public void FireTornadoExplosion() 
    {
        var obj = Instantiate(explosionPrefab, transform, false);
        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isFireTornado)
        {
            if (other.CompareTag("DarkKnight"))
            {
                other.GetComponentInChildren<EnemyAnimatorManager>().tornadoSlashEnhance = true;
                Destroy(transform.gameObject);
            }

            if (other.GetComponentInParent<FlyingObj>() && (!other.GetComponent<PlayerManager>() || !other.GetComponentInParent<PlayerManager>() || !other.GetComponentInChildren<PlayerManager>())
                && (!other.GetComponent<EnemyManager>() || !other.GetComponentInParent<EnemyManager>()) && !other.GetComponent<TornadoHazard>()) //接触飞行道具, 非龙卷类
            {
                //如果是火球的话, 变成火龙卷
                if (other.GetComponentInParent<FlyingObj>().isFireBall)
                {
                    GenerateFireTornado();
                    Destroy(other.transform.parent.gameObject);
                }
                else
                {
                    if (other.GetComponentInParent<FlyingObj>() && other.GetComponent<ProjectileDamager>()
                        && (!other.GetComponentInParent<PlayerManager>() || !other.GetComponentInParent<EnemyManager>())) //所有不与玩家绑定的飞行道具
                    {
                        if (other.GetComponent<ProjectileDamager>().curProjectilType == ProjectileDamager.ProjectilType.regular) 
                        {
                            DefelectFlyingObj(other.GetComponentInParent<FlyingObj>());
                            Destroy(other.gameObject);
                        }
                    }
                }
            }
        }
        else
        {
            if (other.gameObject.layer == 8) //碰到场景障碍后触发爆炸
            {
                FireTornadoExplosion();
            }

            if (other.GetComponent<FlyingObj>() && !other.CompareTag("Player") && !other.CompareTag("Enemy")) //接触飞行道具
            {
                Destroy(other.gameObject);
            }
            else if (other.GetComponentInParent<FlyingObj>() && !other.CompareTag("Player") && !other.CompareTag("Enemy")) //接触其它龙卷
            {
                Destroy(other.gameObject);
            }
            else if (other.GetComponent<EnemyManager>() || other.GetComponent<PlayerManager>())
            {
                FireTornadoExplosion();
            }
        }
    }
}
