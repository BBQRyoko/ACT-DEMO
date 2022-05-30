using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : InteractSystem
{
    PlayerManager playerManager;
    GameManager gameManager;
    [SerializeField] Transform respawnPos;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    public override void Interact()
    {
        base.Interact();
        ProgressSave();
    }

    void ProgressSave() 
    {
        gameManager.curCheckPoint = respawnPos;
        playerManager.Rest();
    }
}
