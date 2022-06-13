using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamager : MonoBehaviour
{
    //dot伤害
    public enum ProjectilType {regular, magic}; //regular考虑重力, magic不考虑重力
    public ProjectilType curProjectilType;
    [SerializeField] TornadoHazard tornadoHazard;
    FlyingObj curFlyingObj;
    public Transform coveredPlayer;
    public bool isPlayerDamage;
    public bool isHeavy;
    public float curDamage = 10;
    public float staminaDamage;
    public float energyRestoreAmount = 20;
    public float chargeAmount;

    public bool isSwitchAttack;

    [SerializeField] AudioClip hitAudio;

    private void Awake()
    {
        tornadoHazard = GetComponent<TornadoHazard>();
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
            EnemyManager enemyManager = coveredCharacter.GetComponent<EnemyManager>();
            if (playerManager)
            {
                PlayerLocmotion playerLocmotion = coveredCharacter.GetComponent<PlayerLocmotion>();
                if (playerManager.isFalling && !playerManager.isGround) 
                {
                    playerManager.isFalling = false;
                    playerManager.isGround = true;
                }
                playerManager.isStunned = true;
                playerManager.isToronadoCovered = true;
                tornadoHazard.TornadoLifeCheck();
                coveredPlayer = coveredCharacter;
                coveredCharacter.transform.position = transform.position;
                coveredCharacter.transform.rotation = transform.rotation;
                playerLocmotion.rig.velocity = new Vector3(0f, 0f, 0f);
                coveredCharacter.transform.parent = transform;
            }
            else if (enemyManager)
            {
                EnemyLocomotion enemyLocomotion = coveredCharacter.GetComponent<EnemyLocomotion>();
                enemyManager.GetComponentInChildren<EnemyAnimatorManager>().animator.SetBool("isStunned", true);
                enemyManager.isToronadoCovered = true;
                tornadoHazard.TornadoLifeCheck();
                coveredPlayer = coveredCharacter;
                coveredCharacter.transform.position = transform.position;
                coveredCharacter.transform.rotation = transform.rotation;
                enemyLocomotion.GetComponent<Rigidbody>().velocity = new Vector3(0f, enemyLocomotion.GetComponent<Rigidbody>().velocity.y, 0f);
                coveredPlayer.parent = transform;
            }
        }
        else //如果风里面已经有了一个角色，就直接停下风
        {
            ProjectileDestroy(coveredCharacter);
        }
    }
    public void ProjectileDestroy(Transform hittedCharacter = null) 
    {
        if (coveredPlayer) //如果玩家在其中就将玩家弹出
        {
            CharacterManager characterManager = null; 
            if (coveredPlayer.CompareTag("Player"))
            {
                characterManager = coveredPlayer.GetComponent<CharacterManager>();
                characterManager.isToronadoCovered = false;
                coveredPlayer.parent = null;
            }
            else //Enemy
            {
                characterManager = coveredPlayer.GetComponentInChildren<CharacterManager>();
                characterManager.isToronadoCovered = false;
                coveredPlayer.parent = characterManager.GetComponent<EnemyManager>().originalParent;
            }
            if (characterManager.GetComponent<EnemyManager>()) 
            coveredPlayer = null;
            if (hittedCharacter)
            {
                if (hittedCharacter.GetComponentInChildren<EnemyAnimatorManager>()) hittedCharacter.GetComponentInChildren<EnemyAnimatorManager>().animator.SetBool("isStunned", true);
                else if (hittedCharacter.GetComponent<PlayerManager>()) hittedCharacter.GetComponent<PlayerManager>().isStunned = true;
            }
            gameObject.SetActive(false);
            Destroy(gameObject,5f);
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
        if (other.gameObject.layer == 8 && !other.CompareTag("DestructibleObject")) //碰到场景障碍后消失
        {
            ProjectileDestroy();
        }

        CharacterStats characterStats = other.GetComponent<CharacterStats>();

        if (tornadoHazard) //龙卷check, 如果是龙卷碰到玩家就会无视防御
        {
            if (characterStats)
            {
                ToronadoCheck(characterStats.transform);
            }
            else 
            {
                if (other.CompareTag("DestructibleObject"))
                {
                    DestructibleObject destructibleObject = other.GetComponent<DestructibleObject>();

                    if (destructibleObject != null)
                    {
                        destructibleObject.ObjectDestroy();
                    }
                }
            }
        }
        else 
        {
            if (!isPlayerDamage) //敌人伤害时
            {
                PlayerStats playerStats = other.GetComponent<PlayerStats>();
                ParryCollider parryCollider = other.GetComponent<ParryCollider>();

                if (playerStats != null)
                {
                    Vector3 hitDirection = transform.position - playerStats.transform.position;
                    hitDirection.y = 0;
                    hitDirection.Normalize();

                    playerStats.TakeDamage(curDamage, hitDirection, isHeavy);
                    if (hitAudio)
                    {
                        playerStats.GetComponentInChildren<AnimatorManager>().generalAudio.clip = hitAudio;
                        playerStats.GetComponentInChildren<AnimatorManager>().generalAudio.Play();
                    }
                    ProjectileDestroy();
                    if (!playerStats.GetComponent<PlayerManager>().cameraManager.currentLockOnTarget)
                    {
                        if (GetComponentInParent<FlyingObj>())
                        {
                            Debug.Log("123");
                            playerStats.GetComponent<PlayerManager>().cameraManager.currentLockOnTarget = GetComponentInParent<FlyingObj>().shooterPos.GetComponentInParent<EnemyManager>().lockOnTransform;

                            playerStats.GetComponent<InputManager>().lockOn_Flag = true;
                        }
                        else 
                        {
                            if (GetComponent<FlyingObj>())
                            {
                                playerStats.GetComponent<PlayerManager>().cameraManager.currentLockOnTarget = GetComponent<FlyingObj>().shooterPos.GetComponentInParent<EnemyManager>().lockOnTransform;
                                playerStats.GetComponent<InputManager>().lockOn_Flag = true;
                            }
                        }
                    }
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
            else //玩家伤害 
            {
                EnemyStats enemyStats = other.GetComponent<EnemyStats>();
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                PlayerManager playerManager = playerStats.GetComponent<PlayerManager>();
                AnimatorManager animatorManager = playerStats.GetComponentInChildren<AnimatorManager>();
                ParryCollider parryCollider = other.GetComponent<ParryCollider>();

                if (enemyStats != null)
                {
                    Vector3 hitDirection = transform.position - enemyStats.transform.position;
                    hitDirection.y = 0;
                    hitDirection.Normalize();
                    enemyStats.TakeDamage(curDamage, staminaDamage, hitDirection);
                    enemyStats.GetComponent<EnemyManager>().curTarget = playerStats;
                    playerManager.GetComponent<PlayerAttacker>().chargeValue += chargeAmount;
                    playerManager.GetComponent<BaGuaManager>().YinYangChargeUp(energyRestoreAmount);
                    if (playerManager.GetComponent<BaGuaManager>().isSwitchAttack)
                    {
                        playerManager.GetComponent<BaGuaManager>().curEnergyCharge += 60f;
                        playerManager.GetComponent<BaGuaManager>().isSwitchAttack = false;
                    }
                    if (hitAudio) 
                    {
                        animatorManager.generalAudio.clip = hitAudio;
                        animatorManager.generalAudio.Play();
                    }
                    Destroy(transform.parent.gameObject);
                }
                else if(parryCollider != null && curProjectilType == ProjectilType.regular) 
                {
                    //普通箭
                    EnemyManager enemyManager = parryCollider.GetComponentInParent<EnemyManager>();
                    enemyManager.GetComponentInChildren<AudioSource>().volume = 0.2f;
                    int i = enemyManager.GetComponentInChildren<Sample_SFX>().blockedSFX_List.Length;
                    int random = Random.Range(0, i - 1);
                    enemyManager.GetComponentInChildren<AudioSource>().clip = enemyManager.GetComponentInChildren<Sample_SFX>().blockedSFX_List[random];
                    enemyManager.GetComponentInChildren<AudioSource>().Play();
                    playerManager.GetComponent<PlayerAttacker>().chargeValue += chargeAmount * 0.8f;
                    playerManager.GetComponent<BaGuaManager>().YinYangChargeUp(energyRestoreAmount / 2);
                    if (playerManager.GetComponent<BaGuaManager>().isSwitchAttack)
                    {
                        playerManager.GetComponent<BaGuaManager>().curEnergyCharge += 50f;
                        playerManager.GetComponent<BaGuaManager>().isSwitchAttack = false;
                    }
                    enemyManager.HandleParryingCheck(curDamage);
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    if (other.CompareTag("DestructibleObject"))
                    {
                        DestructibleObject destructibleObject = other.GetComponent<DestructibleObject>();

                        if (destructibleObject != null)
                        {
                            destructibleObject.ObjectDestroy();
                        }
                    }
                    else if (other.CompareTag("FireBlocker")) //是个隐患，之后可以改一下
                    {
                        Destroy(other.gameObject);
                        Destroy(other.GetComponentInChildren<FireRequiredDoor>().fireUi.gameObject);
                    }
                }
            }
        }
    }
}
