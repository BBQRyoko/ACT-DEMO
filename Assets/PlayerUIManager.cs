using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerUIManager : MonoBehaviour
{
    PlayerManager playerManager;
    PlayerInventory playerInventory;
    public GameObject aimingCorsshair;
    public GameObject powerArrowUsingIcon;
    [SerializeField] TextMeshProUGUI powerArrowNum;

    //prompt
    public GameObject promptInfo;
    public TextMeshProUGUI promptString;
    public bool promptActive;
    float promptTimer; //temp

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        powerArrowNum.text = playerInventory.powerArrowNum.ToString();
        if (playerManager.GetComponent<PlayerAttacker>().isUsingPowerArrow)
        {
            powerArrowUsingIcon.SetActive(true);
        }
        else 
        {
            powerArrowUsingIcon.SetActive(false);
        }

        if (promptActive)
        {
            if (promptTimer <= 1)
            {
                promptTimer += Time.deltaTime;
            }
            else
            {
                if (playerManager.GetComponent<InputManager>().interact_Input)
                {
                    promptInfo.SetActive(false);
                    promptString.text = null;
                    promptActive = false;
                }
            }
        }
        else 
        {
            promptInfo.SetActive(false);
        }
    }
    public void PromptInfoActive(string infoText) 
    {
        if (!promptActive) 
        {
            promptTimer = 0;
            promptInfo.SetActive(true);
            promptString.text = infoText;
            promptActive = true;
        }
    }
}
