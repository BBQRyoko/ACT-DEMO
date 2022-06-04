using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHangingRope : InteractSystem
{
    PlayerManager playerManager;
    [SerializeField] Transform startPos;
    [SerializeField] Transform destinationHangingPos;
    Vector3 hangingMoveDirection;

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        hangingMoveDirection = new Vector3(destinationHangingPos.position.x - startPos.position.x, destinationHangingPos.position.y - startPos.position.y, destinationHangingPos.position.z - transform.position.z);
        hangingMoveDirection.Normalize();
    }

    public override void Interact()
    {
        base.Interact();
        if (playerManager.isHanging)
        {
            playerManager.HangingController();
            playerManager.isHanging = false;
            playerManager.hangDirection = Vector3.zero;
        }
        else 
        {
            if (playerManager.isCrouching) playerManager.isCrouching = false;
            playerManager.transform.position = transform.position;
            playerManager.transform.rotation = transform.rotation;
            playerManager.GetComponent<PlayerLocmotion>().movementVelocity.y = 0;
            playerManager.hangDirection = hangingMoveDirection;
            playerManager.HangingController();
        }
    }
}
