using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [SerializeField] CharacterManager characterManager;
    public EnemyManager enemyManager;
    public PlayerManager playerManager;
    public AudioSource attackAudio;
    Collider damageCollider;

    public bool parryWindow_isOpen;

    public Sample_SFX sample_SFX;

    int curDamage = 10;

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
            PlayerManager playerManager = parryCollider.GetComponentInParent<PlayerManager>();

            if (enemyManager != null) // 敌人的格挡
            {
                EnemyStats enemyStats = enemyManager.GetComponent<EnemyStats>();
                float staminaDamage = (weaponWeightRatio+1) * curDamage;
                //普通格挡
                //音效处理
                attackAudio.volume = 0.2f;
                int i = sample_SFX.blockedSFX_List.Length;
                int random = Random.Range(0, i - 1);
                attackAudio.clip = sample_SFX.blockedSFX_List[random];
                attackAudio.Play();

                enemyManager.HandleParryingCheck(staminaDamage);
                HitPause(duration);
                damageCollider.enabled = false;
            }
            else //玩家的格挡
            {
            
            }

            //if (parryCollider != null) 
            //{
            //    if (parryCollider.isPerfect)
            //    {
            //        enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimation("GetHit_Up", true, true);
            //        playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("WeaponAbility_01(Success)", true, true);
            //        playerManager.PerfectBlock();
            //    }
            //    else
            //    {
            //        playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("WeaponAbility_01(Broken)", true, true);
            //    }
            //    DisableDamageCollider();
            //}
        }
        else if (collision.tag == "Player")
        {
            Vector3 hitDirection = transform.position - playerManager.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            PlayerStats playerStats = collision.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                if (!playerStats.GetComponent<PlayerManager>().damageAvoid && !playerStats.GetComponent<PlayerManager>().isPerfect)
                {
                    playerStats.TakeDamage(curDamage, hitDirection, true);
                }
                else if (playerStats.GetComponent<PlayerManager>().isPerfect) 
                {
                    enemyManager.GetComponentInChildren<EnemyAnimatorManager>().PlayTargetAnimation("GetHit_Up", true, true);
                    playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("WeaponAbility_01(Success)", true, true);
                    playerManager.PerfectBlock();
                }
            }
        }
        else if (collision.tag == "Enemy")
        {
            Vector3 hitDirection = transform.position - collision.transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();

            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            if (enemyStats != null && enemyStats.currHealth != 0 && !enemyStats.GetComponent<EnemyManager>().isDodging && !enemyStats.GetComponent<EnemyManager>().isBlocking)
            {
                attackAudio.volume = 0.15f;
                int i = sample_SFX.hittedSFX_List.Length;
                int random = Random.Range(0, i - 1);
                attackAudio.clip = sample_SFX.hittedSFX_List[random];
                attackAudio.Play();
                enemyStats.TakeDamage(curDamage, hitDirection, playerManager.GetComponent<PlayerStats>());
                HitPause(duration);
                playerManager.isHitting = true;
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
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1f;
    }
}
