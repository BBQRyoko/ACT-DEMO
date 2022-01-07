using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : Interactable
{
    public Item item;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    void PickUpItem(PlayerManager playerManager) 
    {
        PlayerInventory playerInventory;
        PlayerLocmotion playerLocmotion;
        AnimatorManager animatorManager;
        playerInventory = playerManager.GetComponent<PlayerInventory>();
        playerLocmotion = playerManager.GetComponent<PlayerLocmotion>();
        animatorManager = playerManager.GetComponentInChildren<AnimatorManager>();

        playerLocmotion.rig.velocity = Vector3.zero;
        animatorManager.PlayTargetAnimation("Pick_Floor", true);
        playerInventory.AddItem(item, 1);
        Debug.Log(interactableText);
        Destroy(this.gameObject);
    }
}
