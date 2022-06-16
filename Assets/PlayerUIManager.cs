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
    public GameObject gsChargeBar;
    public Image gsChargeBarSlider;
    [SerializeField] Image weaponImage;
    [SerializeField] Image switchAttackTimer;

    //prompt
    public GameObject promptInfo;
    public TextMeshProUGUI promptString;
    public bool promptActive;
    float promptTimer; //temp

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerInventory = GetComponent<PlayerInventory>();
        switchAttackTimer.fillAmount = playerManager.transAttackTimer / 1.75f;
    }

    private void Update()
    {
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
}
