using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    //还是要设置攻击的类型
    public bool isHeavyAttack;
    [SerializeField] CharacterManager characterManager;
    public EnemyManager enemyManager;
    public PlayerManager playerManager;
    public AudioSource attackAudio;
    [SerializeField]Collider damageCollider;
    bool isPlayer;

    public bool parryWindow_isOpen;
    public Sample_SFX sample_SFX;

    public float curDamage = 10;
    public float staminaDamage;
    public float energyRestoreAmount = 20;
    public float chargeAmount;

    public float weaponWeightRatio;
    public int duration;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }
    private void Start()
    {
        characterManager = GetComponentInParent<CharacterManager>();
        enemyManager = GetComponentInParent<EnemyManager>();
        attackAudio = characterManager.GetComponentInChildren<AudioSource>();
        sample_SFX = characterManager.GetComponentInChildren<Sample_SFX>();
        if (transform.GetComponentInParent<PlayerManager>())
        {
            isPlayer = true;
        }
    }
    public void EnableDamageCollider() 
    {
        damageCollider.enabled = true;
    }
    public void DisableDamageCollider() 
    {
        damageCollider.enabled = false;
        playerManager.isHitting = false;
    }
    public void weaponColliderDamageModifier(int damage) 
    {
        curDamage = damage;
    }
    public void parryWindow_Open() 
    {
        parryWindow_isOpen = true;
    }
    public void parryWindow_Close()
    {
        parryWindow_isOpen = false;
    }
    private void OnTriggerEnter(Collider collision)
    {   
        if (collision.tag == "Parry")
        {
            ParryCollider parryCollider = collision.GetComponent<ParryCollider>();
            EnemyManager enemyManager = parryCollider.GetComponentInParent<EnemyManager>();
            PlayerManager playerManager1 = parryCollider.GetComponentInParent<PlayerManager>(); //玩家格挡的情况

            if (enemyManager != null && isPlayer) // 敌人的格挡
            {
                if (curDamage >= 5)
                {
                    attackAudio.volume = 0.2f;
                    int i = sample_SFX.blockedSFX_List.Length;
                    int random = Random.Range(0, i - 1);
                    attackAudio.clip = sample_SFX.blockedSFX_List[random];
                    attackAudio.Play();
                    damageCollider.enabled = false;
                    playerManager.GetComponent<PlayerAttacker>().chargeValue += chargeAmount * 0.8f;
                    playerManager.GetComponent<BaGuaManager>().YinYangChargeUp(energyRestoreAmount / 2);
                    if (playerManager.GetComponent<BaGuaManager>().isSwitchAttack)
                    {
                        playerManager.GetComponent<BaGuaManager>().curEnergyCharge += 50f;
                        playerManager.GetComponent<BaGuaManager>().isSwitchAttack = false;
                    }
                    enemyManager.HandleParryingCheck(curDamage);
                    HitPause(duration);
                }
            }
            else if (playerManager1 != null && !isPlayer) //玩家的格挡
            {
                AudioSource attackAudioSource = playerManager1.GetComponentInChildren<AudioSource>();
                Sample_SFX sample_SFX_Source = playerManager1.GetComponentInChildren<Sample_SFX>();
                attackAudioSource.volume = 0.2f;
                int i = sample_SFX_Source.blockedSFX_List.Length;
                int random = Random.Range(0, i - 1);
                attackAudioSource.clip = sample_SFX_Source.blockedSFX_List[random];
                attackAudioSource.Play();

                damageCollider.enabled = false;
                playerManager1.HandleParryingCheck(curDamage);
            }
        }
        else if (collision.tag == "Player" && !isPlayer)
        {
            Vector3 hitDirection = enemyManager.transform.position - playerManager.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            PlayerStats playerStats = collision.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (playerStats.GetComponent<PlayerManager>().isPerfect) 
                {
                    EnemyStats enemyStats = enemyManager.GetComponent<EnemyStats>();
                    damageCollider.enabled = false;
                    enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimation("GetHit_Up", true, true);
                    playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Defend(Counter)", true, true);
                    playerManager.GetComponentInChildren<AnimatorManager>().generalAudio.volume = 0.15f;
                    playerManager.GetComponentInChildren<AnimatorManager>().generalAudio.clip = playerManager.GetComponentInChildren<AnimatorManager>().sample_SFX.Bagua_SFX_List[0];
                    playerManager.GetComponentInChildren<AnimatorManager>().generalAudio.Play();
                    enemyStats.TakeStaminaDamage(120f);
                }
                else if (!playerStats.GetComponent<PlayerManager>().damageAvoid && !playerStats.GetComponent<PlayerManager>().isPerfect)
                {
                    attackAudio.volume = 0.08f;
                    int i = sample_SFX.hittedSFX_List.Length;
                    int random = Random.Range(0, i - 1);
                    attackAudio.clip = sample_SFX.hittedSFX_List[random];
                    attackAudio.Play();
                    playerStats.TakeDamage(curDamage, hitDirection, isHeavyAttack);
                }
            }
        }
        else if (collision.tag == "Enemy" && isPlayer)
        {
            Vector3 hitDirection = transform.position - collision.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            if (!enemyStats.GetComponent<EnemyManager>().isParrying && enemyStats != null && enemyStats.currHealth != 0 && !enemyStats.GetComponent<EnemyManager>().isDodging && !enemyStats.GetComponent<EnemyManager>().isBlocking)
            {
                attackAudio.volume = 0.15f;
                int i = sample_SFX.hittedSFX_List.Length;
                int random = Random.Range(0, i - 1);
                attackAudio.clip = sample_SFX.hittedSFX_List[random];
                attackAudio.Play();
                enemyStats.TakeDamage(curDamage, staminaDamage, hitDirection, playerManager.GetComponent<PlayerStats>());
                HitPause(duration);
                playerManager.GetComponent<PlayerAttacker>().chargeValue += chargeAmount;
                playerManager.GetComponent<BaGuaManager>().YinYangChargeUp(energyRestoreAmount);
                if (playerManager.GetComponent<BaGuaManager>().isSwitchAttack)
                {
                    if (playerManager.GetComponent<BaGuaManager>().switchAttackTutorial != null) 
                    {
                        playerManager.GetComponent<BaGuaManager>().switchAttackTutorial.switchAttackNum += 1;
                    } 
                    playerManager.GetComponent<BaGuaManager>().curEnergyCharge += 75f;
                    playerManager.GetComponent<BaGuaManager>().isSwitchAttack = false;
                }
                playerManager.isHitting = false;

            }
        }
        else if (collision.tag == "DestructibleObject")
        {
            DestructibleObject destructibleObject = collision.GetComponent<DestructibleObject>();

            if (destructibleObject != null)
            {
                destructibleObject.ObjectDestroy();
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
                playerManager.isHitting = false;
            }
        }
    }
    public void HitPause(int dur) 
    {
        StartCoroutine(Hitted(dur));
    }
    IEnumerator Hitted(int dur) 
    {
        float pauseTime = dur / 60f;
        Time.timeScale = 0.6f;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1f;
    }
}
