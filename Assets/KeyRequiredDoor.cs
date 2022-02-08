using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRequiredDoor : InteractSystem
{
    PlayerManager playerManager;
    Animator animator;
    public bool doorIsOpened;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        animator = GetComponent<Animator>();
    }
    public override void Interact() 
    {
        base.Interact();
        if (playerManager.keyNum >= 1 && !doorIsOpened)
        {
            playerManager.keyNum -= 1;
            animator.SetTrigger("DoorOpen");
            gameObject.GetComponent<BoxCollider>().enabled = false;
            doorIsOpened = true;
        }
        else 
        {
            Debug.Log("KeyRequired");
        }
    }
}
