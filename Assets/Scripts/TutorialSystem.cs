using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialSystem : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] TextMeshProUGUI tutorialTitle;
    [SerializeField] Image tutorialImage;
    [SerializeField] TextMeshProUGUI tutorialDes;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public void OpenTutorial(TutorialScriptableObject tutorial) 
    {
        tutorialTitle.text = tutorial.title;
        //tutorialImage = tutorial.image;
        tutorialDes.text = tutorial.description;
        gameObject.SetActive(true);
    }

    public void CloseTutorial() 
    {
        gameObject.SetActive(false);
        gameManager.Resume();
    }
}
