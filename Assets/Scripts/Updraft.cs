using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updraft : InteractSystem
{
    [SerializeField] float maxJumpHeight = 14f;
    [SerializeField] float maxJumpTime = 2f;
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
        playerLocmotion.HandleJumping(maxJumpHeight, maxJumpTime);
    }
}
