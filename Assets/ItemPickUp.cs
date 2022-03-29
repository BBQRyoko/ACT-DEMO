using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : InteractSystem
{
    PlayerManager playerManager;
    public enum ItemType {key, katana, newGreatSword};
    public ItemType curItemType;

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    public override void Interact()
    {
        base.Interact();
        if (curItemType == ItemType.key)
        {
            playerManager.keyNum += 1;
            Destroy(gameObject.transform.parent.gameObject);
        }
        else if (curItemType == ItemType.katana) 
        {
            playerManager.katanaUnlock = true;
            playerManager.GetComponent<PlayerInventory>().UnlockKatana();
            playerManager.GetComponentInChildren<WeaponSlotManager>().mainArmedWeapon.SetActive(false);
            playerManager.GetComponentInChildren<WeaponSlotManager>().mainWeapon_Unequipped.gameObject.SetActive(true);
            playerManager.GetComponentInChildren<WeaponSlotManager>().WeaponSwitchAnimatorController();
            playerManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("WeaponSwitch(Equip)", true, true);
            playerManager.inInteractTrigger = false;
            playerManager.GetComponent<InputManager>().interact_Input = false;
            Destroy(gameObject.transform.parent.gameObject);
        }
        else if (curItemType == ItemType.newGreatSword)
        {
            //最后一关时写
        }
    }
}
