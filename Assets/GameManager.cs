using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    PlayerManager playerManager;
    [SerializeField] TutorialSystem tutorialSystem;
    public Transform curCheckPoint;
    [SerializeField] EnemyManager[] enemiesWillSpawn;

    public GameObject restartMenu;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        //enemiesWillSpawn = FindObjectsOfType<EnemyManager>();
    }
    public void Tutorial(TutorialScriptableObject tutorial) 
    {
        tutorialSystem.OpenTutorial(tutorial);
        GamePause();
    }
    public void PlayerDead() 
    {
        restartMenu.SetActive(true);
    }
    public void Restart()
    {
        //黑屏那些
        restartMenu.SetActive(false);
        playerManager.transform.position = curCheckPoint.position;
        playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Respawn",true,true);
        playerManager.GetComponentInChildren<WeaponSlotManager>().mainWeapon_Unequipped.gameObject.SetActive(true);
        playerManager.GetComponentInChildren<WeaponSlotManager>().mainArmedWeapon.SetActive(false);
        playerManager.isWeaponEquipped = false;
        playerManager.Rest();
    }
    void GamePause() 
    {
        Time.timeScale = 0;
    }
    public void Resume() 
    {
        Time.timeScale = 1;
    }


}
