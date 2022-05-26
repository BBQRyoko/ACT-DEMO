using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamager : MonoBehaviour
{
    [SerializeField] bool isToronado;
    FlyingObj curFlyingObj;
    [SerializeField] Transform coveredPlayer;
    public bool isHeavy;
    public bool isPlayerDamage;
    public int curDamage = 10;
    public int staminaDamage;
    public float energyRestoreAmount = 20;
    public float chargeAmount;
    [SerializeField] float hitFactor;

    private void Awake()
    {
        if (!transform.parent)
        {
            curFlyingObj = GetComponent<FlyingObj>();
        }
        else 
        {
            curFlyingObj = GetComponentInParent<FlyingObj>();
        }
    }
    private void Update()
    {
        if (coveredPlayer) //存在包裹的单位
        {
            curFlyingObj.m_TraceTime = 0;
        }
    }
    void ToronadoCheck(Transform coveredCharacter) 
    {
        if (!coveredPlayer)
        {
            PlayerManager playerManager = coveredCharacter.GetComponent<PlayerManager>();
            PlayerLocmotion playerLocmotion = coveredCharacter.GetComponent<PlayerLocmotion>();
            playerManager.isStunned = true;
            playerManager.isToronadoCovered = true;
            coveredPlayer = coveredCharacter;
            coveredCharacter.transform.position = transform.position;
            coveredCharacter.transform.rotation = transform.rotation;
            playerLocmotion.rig.velocity = new Vector3(0f, playerLocmotion.rig.velocity.y, 0f);
            coveredCharacter.transform.parent = transform;
        }
        else //如果风里面已经有了一个角色，就直接停下风
        {
            ProjectileDestroy();
        }
    }
    void ProjectileDestroy() 
    {
        if (coveredPlayer) //如果玩家在其中就将玩家弹出
        {
            PlayerManager playerManager = coveredPlayer.GetComponent<PlayerManager>();
            playerManager.isToronadoCovered = false;
            coveredPlayer.parent = null;
            coveredPlayer = null;
            Destroy(gameObject);
        }
        else 
        {
            if (transform.parent)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(transform.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) //碰到场景障碍后消失
        {
            ProjectileDestroy();
        }

        if (!isPlayerDamage) //敌人伤害时
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            ParryCollider parryCollider = other.GetComponent<ParryCollider>();

            if (isToronado && playerStats) //龙卷check, 如果是龙卷碰到玩家就会无视防御
            {
                ToronadoCheck(playerStats.transform);
            }
            else 
            {
                if (playerStats != null)
                {
                    Vector3 hitDirection = transform.position - playerStats.transform.position;
                    hitDirection.y = 0;
                    hitDirection.Normalize();

                    playerStats.TakeDamage(curDamage, hitDirection * hitFactor, isHeavy);
                    ProjectileDestroy();
                }
                else if (parryCollider != null)
                {
                    PlayerManager playerManager = parryCollider.GetComponentInParent<PlayerManager>();
                    AudioSource attackAudioSource = playerManager.GetComponentInChildren<AudioSource>();
                    Sample_SFX sample_SFX_Source = playerManager.GetComponentInChildren<Sample_SFX>();
                    attackAudioSource.volume = 0.2f;
                    int i = sample_SFX_Source.blockedSFX_List.Length;
                    int random = Random.Range(0, i - 1);
                    attackAudioSource.clip = sample_SFX_Source.blockedSFX_List[random];
                    attackAudioSource.Play();
                    playerManager.HandleParryingCheck(curDamage);
                    Destroy(this.gameObject);
                }
            }
        }
        else //玩家伤害 
        {
            Vector3 hitDirection = new Vector3(0, 0, 0);
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            PlayerManager playerManager = playerStats.GetComponent<PlayerManager>();
            AnimatorManager animatorManager = playerStats.GetComponentInChildren<AnimatorManager>();
            if (enemyStats != null) 
            {
                enemyStats.TakeDamage(curDamage, staminaDamage, hitDirection * hitFactor);
                enemyStats.GetComponent<EnemyManager>().curTarget = playerStats;
                playerManager.GetComponent<PlayerAttacker>().chargeValue += chargeAmount;
                playerManager.GetComponent<BaGuaManager>().YinYangChargeUp(energyRestoreAmount);
                //火球击中音效
                //animatorManager.generalAudio.volume = 0.1f;
                //animatorManager.generalAudio.clip = animatorManager.sample_SFX.Bagua_SFX_List[3];
                //animatorManager.generalAudio.Play();

                Destroy(transform.parent.gameObject);
            }
            else
            {
                if (other.CompareTag("FireBlocker"))
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
