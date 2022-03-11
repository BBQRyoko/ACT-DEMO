using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] bossRoomFogWall;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] bool playerEntered;
    [SerializeField] EnemyStats bossStats;
    [SerializeField] GameObject bossHealthBar;
    [SerializeField] CameraManager cameraManager;

    public bool katanaRequired;
    [SerializeField] GameObject weaponSwitch_guide;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
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
        }

        if (playerManager.isDead)
        {
            playerEntered = false;
            bossHealthBar.SetActive(false);
        }

        if (bossStats.currHealth <= 0) 
        {
            bossHealthBar.SetActive(false);
            if (!katanaRequired)
            {
                playerEntered = false;
                Destroy(this.gameObject);
            }
            else 
            {
                if (playerManager.katanaUnlock)
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerEntered = true;
            bossHealthBar.SetActive(true);
            bossHealthBar.GetComponent<HealthBar>().SetMaxHealth(bossStats.maxHealth);
        }
    }

}
