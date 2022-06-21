using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] bossRoomFogWall;
    [SerializeField] PlayerManager playerManager;
    public bool playerEntered;
    [SerializeField] EnemyStats bossStats;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] StaminaBar bossStaminaBar;

    public bool katanaRequired;
    [SerializeField] GameObject weaponSwitch_guide;
    [SerializeField] AudioClip combatClip;
    [SerializeField] BGMManager combatBGM;
    bool bgmChanged;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        bossStaminaBar = bossHealthBar.GetComponentInChildren<StaminaBar>();
        combatBGM = FindObjectOfType<BGMManager>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject fogwall in bossRoomFogWall)
        {
            if (playerEntered)
            {
                fogwall.SetActive(true);
            }
            else
            {
                fogwall.SetActive(false);
            }
        }

        if (playerEntered)
        {
            bossHealthBar.GetComponent<HealthBar>().SetCurrentHealth(bossStats.currHealth);
            bossStaminaBar.SetCurrentStamina(bossStats.currStamina);
        }

        if (playerManager.isDead)
        {
            playerEntered = false;
            bossHealthBar.SetActive(false);
            playerManager.enteredBossRoom = false;
            CombatBGMStop();
        }

        if (bossStats.currHealth <= 0)
        {
            bossHealthBar.SetActive(false);
            if (!katanaRequired)
            {
                playerEntered = false;
                playerManager.enteredBossRoom = false;
                if (combatClip) combatBGM.BGMStop();
                Destroy(this.gameObject);
            }
            else
            {
                if (playerManager.GetComponent<PlayerInventory>().curEquippedWeaponItem.Id == 1)
                {
                    playerEntered = false;
                    weaponSwitch_guide.SetActive(true);
                    Destroy(this.gameObject);
                }
                else
                {
                    weaponSwitch_guide.SetActive(false);
                }
            }
        }
        if (bossStats.GetComponent<EnemyManager>().isPhaseChaging && !bgmChanged) 
        {
            combatClip = bossStats.GetComponent<EnemyManager>().secondPhaseBGM;
            bgmChanged = true;
            combatBGM.BGMPlay(combatClip);
        }
    }

    void CombatBGMStop() 
    {
        combatBGM.BGMStop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
            bossHealthBar.SetActive(true);
            bossHealthBar.GetComponent<HealthBar>().SetMaxHealth(bossStats.maxHealth);
            bossStaminaBar.SetMaxStamina(bossStats.maxStamina);
            combatBGM.BGMPlay(combatClip);
            playerManager.enteredBossRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == bossStats.gameObject)
        {
            if (!bossStats.GetComponent<EnemyManager>().isTaijied && !playerEntered) bossStats.GetComponent<EnemyManager>().EnemyRestartReset();
        }

        if (other.CompareTag("Player")) 
        {
            playerEntered = false;
            playerManager.enteredBossRoom = false;
            bossHealthBar.SetActive(false);
            CombatBGMStop();
        }
    }
}
