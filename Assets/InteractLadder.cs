using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractLadder : InteractSystem
{
    PlayerManager playerManager;
    [Header("0 = Bot, 1 = TOP")]
    [SerializeField] int index; // 0 - Bot, 1 - Top
    [SerializeField] Transform startPos;
    [SerializeField] Transform destinationHangingPos;
    Vector3 ClimbingDirection;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        ClimbingDirection = new Vector3(destinationHangingPos.position.x - startPos.position.x, destinationHangingPos.position.y - startPos.position.y, destinationHangingPos.position.z - transform.position.z);
        ClimbingDirection.Normalize();
    }
    public override void Interact()
    {
        base.Interact();
        if (playerManager.isClimbing)
        {
            ClimbOffLadder();
        }
        else
        {
            ClimbOnLadder();
        }
    }
    void ClimbOnLadder() 
    {
        playerManager.transform.position = transform.position;
        playerManager.transform.rotation = transform.rotation;
        playerManager.GetComponent<PlayerLocmotion>().movementVelocity.y = 0;
        playerManager.climbDirection = ClimbingDirection;
        playerManager.ClimbingController();
    }
    void ClimbOffLadder() 
    {
        if (index == 0)
        {
            playerManager.isClimbing = false;
            playerManager.hangDirection = Vector3.zero;
        }
        else 
        {
            if (playerManager.isCrouching) playerManager.isCrouching = false;
            playerManager.transform.position = transform.position;
            playerManager.transform.rotation = transform.rotation;
            playerManager.ClimbingController();
            playerManager.hangDirection = Vector3.zero;
        }

    }
    public override void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            if (index == 0)
            {
                if (!playerManager.isClimbing)
                {
                    other.GetComponent<PlayerManager>().inInteractTrigger = true;
                    HandleInteractUI(this);
                }
                else
                {
                    ClimbOffLadder();
                }
            }
            else 
            {
                if (playerManager.isClimbing)
                {
                    ClimbOffLadder();
                }
            }
        }
    }
}
