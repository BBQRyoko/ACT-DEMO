using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerUIManager : MonoBehaviour
{
    GameManager gameManager;
    PlayerManager playerManager;
    PlayerInventory playerInventory;
    public GameObject aimingCorsshair;
    public GameObject gsChargeBar;
    public Image gsChargeBarSlider;
    [SerializeField] Image weaponImage;
    [SerializeField] Image switchAttackTimer;

    //prompt
    public GameObject promptInfo;
    public TextMeshProUGUI promptString;
    public bool promptActive;
    float promptTimer; //temp

    //tutorial
    public GameObject tutorialPopup;
    public GameObject bowTutorial;
    bool bowTutorialUp;
    bool bowTaught;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
        switchAttackTimer.fillAmount = playerManager.transAttackTimer / 1.75f;
    }

    private void Update()
    {
        TutorialPopupController();
        BowTutorialPopup();
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
        weaponImage.sprite = playerInventory.curEquippedWeaponItem.weaponImage;
        switchAttackTimer.fillAmount = playerManager.transAttackTimer / 1.75f;
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

    void TutorialPopupController() 
    {
        if (tutorialPopup != null) 
        {
            if (playerManager.GetComponent<InputManager>().interact_Input)
            {
                tutorialPopup.SetActive(false);
                tutorialPopup = null;
            }
        }
    }

    void BowTutorialPopup()
    {
        if (playerInventory.curEquippedWeaponItem.Id == 2 && !bowTutorialUp && !bowTaught) 
        {
            bowTutorial.SetActive(true);
            bowTaught = true;
            bowTutorialUp = true;
            gameManager.gamePaused = true;
        }
        if (bowTutorialUp)
        {
            if (playerManager.GetComponent<InputManager>().interact_Input)
            {
                bowTutorial.SetActive(false);
                bowTutorialUp = false;
                gameManager.gamePaused = false;
            }
        }
    }
}
