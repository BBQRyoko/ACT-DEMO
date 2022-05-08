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

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        WeaponInfoUpdate();
    }

    void WeaponInfoUpdate() 
    {
        foreach (GameObject weapons in weaponPrefabs) 
        {
            weapons.SetActive(false);
        }
        weaponItemInfo = weaponList[weaponInfoIndex];
        weaponPrefabs[weaponInfoIndex].SetActive(true);
    }

    public override void Interact()
    {
        base.Interact();

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
}
