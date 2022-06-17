using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    PlayerManager playerManager;
    InputManager inputManager;
    [SerializeField] TutorialSystem tutorialSystem;
    public Transform curCheckPoint;
    [SerializeField] EnemyManager[] enemyList;

    bool restartMenuOn;
    public GameObject restartMenu;
    public GameObject pauseMenu;
    public bool gamePaused;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        inputManager = FindObjectOfType<InputManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (gamePaused)
        {
            GamePause();
        }
        else
        {
            if(!inputManager.baGua_Input && !restartMenuOn) Resume();
        }
    }

    public void Tutorial(TutorialScriptableObject tutorial) 
    {
        tutorialSystem.OpenTutorial(tutorial);
        GamePause();
    }
    public void PlayerDead() 
    {
        if (!restartMenuOn)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            restartMenu.SetActive(true);
            restartMenuOn = true;
        }
    }
    public void Restart()
    {
        //黑屏那些
        restartMenu.SetActive(false);
        pauseMenu.SetActive(false);
        playerManager.transform.position = curCheckPoint.position;
        playerManager.GetComponentInChildren<AnimatorManager>().animator.SetBool("isInteracting", false);
        playerManager.GetComponentInChildren<AnimatorManager>().animator.SetBool("isUsingRootMotion", false);
        playerManager.GetComponentInChildren<AnimatorManager>().animator.SetBool("isDefending", false);
        playerManager.GetComponentInChildren<AnimatorManager>().animator.SetBool("isHolding", false);
        playerManager.GetComponentInChildren<AnimatorManager>().animator.SetBool("isGettingDamage", false);
        playerManager.GetComponentInChildren<AnimatorManager>().animator.SetBool("cantBeInterrupted", false);
        playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Respawn", true, true);
        playerManager.GetComponentInChildren<WeaponSlotManager>().mainWeapon_Unequipped.gameObject.SetActive(true);
        playerManager.GetComponentInChildren<WeaponSlotManager>().mainArmedWeapon.SetActive(false);
        playerManager.Rest();
        EnemiesReset();
        if (gamePaused) gamePaused = false;
        if (restartMenuOn) restartMenuOn = false;
    }

    public void GameSlowDown(float slowRate = 0.65f)  
    {
        Time.timeScale = slowRate;
    }

    void GamePause() 
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Resume() 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void GamePausMenu() 
    {
        if (!gamePaused)
        {
            pauseMenu.SetActive(true);
            gamePaused = true;
        }
        else 
        {
            gamePaused = false;
            pauseMenu.SetActive(false);
        }
    }
    void EnemiesReset() 
    {
        enemyList = FindObjectsOfType<EnemyManager>();
        foreach (EnemyManager enemy in enemyList) 
        {
            if (!enemy.isDead) enemy.EnemyRestartReset();
        }
    }
}
