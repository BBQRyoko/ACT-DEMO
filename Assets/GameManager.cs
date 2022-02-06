using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TutorialSystem tutorialSystem;

    public void Tutorial(TutorialScriptableObject tutorial) 
    {
        tutorialSystem.OpenTutorial(tutorial);
        GamePause();
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
