using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TutorialPanelController : MonoBehaviour
{
    [SerializeField] GameObject tutorialRect;
    [SerializeField] RectTransform tutorialPos;

    bool tutorialPopup = false;
    private void Update()
    {
        if (!tutorialPopup) 
        {
            tutorialRect.SetActive(false);
        }
    }
    public void ActiveTutorialPanel() 
    {
        if (!tutorialPopup) 
        {
            tutorialPopup = true;
            tutorialRect.SetActive(true);
        }
    }
}
