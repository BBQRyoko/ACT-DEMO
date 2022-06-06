using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRequiredDoor : InteractSystem
{
    PlayerUIManager playerUIManager; //prompt info 
    PlayerManager playerManager;
    [SerializeField] string infoText;

    [SerializeField] bool isSpecialFireDoor;
    public InteractPromptUI fireUi;
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

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(isSpecialFireDoor) fireUi = FindObjectOfType<InteractPromptUI>();
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (isSpecialFireDoor) fireUi = null;
    }
}
