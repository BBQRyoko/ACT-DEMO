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
    }
}
