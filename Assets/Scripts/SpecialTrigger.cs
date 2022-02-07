using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTrigger : MonoBehaviour
{
    //当前只触发教程
    GameManager gameManager;
    [SerializeField] TutorialScriptableObject tutorial;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            gameManager.Tutorial(tutorial);
            Destroy(gameObject);
        }
    }
}
