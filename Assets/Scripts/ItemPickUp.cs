using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : InteractSystem
{
    PlayerManager playerManager;

    [SerializeField] int weaponInfoIndex;

    public WeaponItem weaponItemInfo;
    [SerializeField] WeaponItem[] weaponList;
    [SerializeField] GameObject[] weaponPrefabs;

    public bool isBuff;
    [SerializeField] int buff_Index; // 0 - attack, 1 - movement, 2 - def
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        if (!isBuff) 
        {
            WeaponInfoUpdate();
        }
    }

    void WeaponInfoUpdate() 
    {
        foreach (GameObject weapons in weaponPrefabs) 
        {
            weapons.SetActive(false);
        }
        if (weaponInfoIndex == 0)
        {
            interactText = "大剑(装备)";
        }
        else if (weaponInfoIndex == 1) 
        {
            interactText = "太刀(装备)";
        }
        else if (weaponInfoIndex == 2)
        {
            interactText = "弓(装备)";
        }
        weaponItemInfo = weaponList[weaponInfoIndex];
        weaponPrefabs[weaponInfoIndex].SetActive(true);
    }

    public override void Interact()
    {
        base.Interact();

        if (!isBuff)
        {
            PlayerInventory playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerManager.inInteractTrigger = false;
            playerManager.GetComponent<InputManager>().interact_Input = false;
            if (playerInventory.unequippedWeaponItems[1] == null) //没有有副武器
            {
                playerInventory.PickUpWeapon(weaponItemInfo);
                Destroy(gameObject.transform.parent.gameObject);
            }
            else //有副武器
            {
                int curIndex = playerInventory.curEquippedWeaponItem.Id;
                playerInventory.PickUpWeapon(weaponItemInfo);
                weaponInfoIndex = curIndex;
                WeaponInfoUpdate();
            }
        }
        else 
        {
            Instantiate(playerManager.GetComponent<BaGuaManager>().buffList[buff_Index], playerManager.transform,false);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
