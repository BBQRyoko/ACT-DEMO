using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updraft : InteractSystem
{
    PlayerLocmotion playerLocmotion;
    [SerializeField] bool drafted;

    void Start()
    {
        playerLocmotion = FindObjectOfType<PlayerLocmotion>();
    }
    public override void Interact()
    {
        base.Interact();
        playerLocmotion.transform.position = gameObject.transform.position;
        playerLocmotion.HandleJumping();
    }
}
