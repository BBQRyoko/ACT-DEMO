using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBossRoom : InteractSystem
{
    PlayerManager playerManager;
    [SerializeField] Transform telePos;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    public override void Interact()
    {
        base.Interact();
        playerManager.transform.position = telePos.position;
    }
}
