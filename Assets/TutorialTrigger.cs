using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    TutorialPanelController tutorialPanelController;

    private void Start()
    {
        tutorialPanelController = FindObjectOfType<TutorialPanelController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            tutorialPanelController.ActiveTutorialPanel();
        }
    }
}
