using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassToHide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager.isCrouching)
            {
                playerManager.isInGrass = true;
            }
            else
            {
                playerManager.isInGrass = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager.isCrouching)
            {
                playerManager.isInGrass = true;
            }
            else 
            {
                playerManager.isInGrass = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            playerManager.isInGrass = false;
        }
    }
}
