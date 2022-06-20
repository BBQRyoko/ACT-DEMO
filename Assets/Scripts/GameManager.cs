using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public EventSystem eventSystem;
    PlayerManager playerManager;
    InputManager inputManager;
    [SerializeField] TutorialSystem tutorialSystem;
    public Transform curCheckPoint;
    [SerializeField] EnemyManager[] enemyList;

    [SerializeField] GameObject titlePage;
    [SerializeField] GameObject levelStartMenu;
    [SerializeField] bool gameStartManager;

    bool restartMenuOn;
    public GameObject restartMenu;
    public GameObject pauseMenu;
    public bool gamePaused;
    [SerializeField] Transform[] differentCheckPositions;

    //MenuRelated
    [SerializeField] GameObject pauseFirstButon;
    [SerializeField] GameObject deadFirstButon;
    public GameObject endFirstButon;

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        playerManager = FindObjectOfType<PlayerManager>();
        inputManager = FindObjectOfType<InputManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (gameStartManager) 
        {
            titlePage.SetActive(true);
            playerManager.gameStart = true;
            gamePaused = true;
        }

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
    public void GameStart(int index = 0) 
    {
        titlePage.SetActive(false);
        levelStartMenu.SetActive(true);
        gamePaused = false;
        gameStartManager = false;
        if (index == 0)
        {
            playerManager.transform.position = differentCheckPositions[0].position;
            curCheckPoint = differentCheckPositions[0];
            playerManager.StartFromLevel1();
        }
        else if (index == 1) 
        {
            playerManager.transform.position = differentCheckPositions[1].position;
            curCheckPoint = differentCheckPositions[1];
            playerManager.StartFromLevel2();
        }
        else if (index == 2)
        {
            playerManager.transform.position = differentCheckPositions[2].position;
            curCheckPoint = differentCheckPositions[2];
            playerManager.StartFromLevel3();
        }
    }
    public void PlayerDead() 
    {
        if (!restartMenuOn)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            restartMenu.SetActive(true);
            restartMenuOn = true;
            eventSystem.SetSelectedGameObject(deadFirstButon);
        }
    }

    public void GameExit() 
    {
        Application.Quit();
    }
    public void Restart()
    {
        //黑屏那些
        restartMenu.SetActive(false);
        pauseMenu.SetActive(false);
        BossRoomTrigger[] bossRoomTriggers = FindObjectsOfType<BossRoomTrigger>();
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

    public void GameSlowDown(float slowRate = 0.45f)  
    {
        Time.timeScale = slowRate;
    }

    void GamePause() 
    {
        Time.timeScale = 0.000000001f;
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
            eventSystem.SetSelectedGameObject(pauseFirstButon);
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
