using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRequiredDoor : InteractSystem
{
    PlayerUIManager playerUIManager; //prompt info 
    PlayerManager playerManager;
    [SerializeField] string infoText;
    // Start is called before the first frame update
    void Start()
    {
        playerUIManager = FindObjectOfType<PlayerUIManager>();
        playerManager = FindObjectOfType<PlayerManager>();
    }
    public override void Interact()
    {
        base.Interact();
        playerUIManager.PromptInfoActive(infoText);
    }
}
