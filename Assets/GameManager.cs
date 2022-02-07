using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerManager playerManager;
    [SerializeField] TutorialSystem tutorialSystem;
    public Transform curCheckPoint;
    [SerializeField] EnemyManager[] enemiesWillSpawn;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        enemiesWillSpawn = FindObjectsOfType<EnemyManager>();
    }
    public void Tutorial(TutorialScriptableObject tutorial) 
    {
        tutorialSystem.OpenTutorial(tutorial);
        GamePause();
    }

    public void Restart()
    {
        //黑屏那些
        playerManager.transform.position = curCheckPoint.position;
        playerManager.Respawn();
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
